using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using trAInr.Application.DTOs.AI;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Application.Interfaces.Services.AI;
using trAInr.Domain.Entities;

namespace trAInr.Application.Services.AI;

public class AiProgramGenerationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AiProgramGenerationService> _logger;
    private Task? _executingTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public AiProgramGenerationService(
        IServiceProvider serviceProvider,
        ILogger<AiProgramGenerationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_cancellationTokenSource.Token);
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            _cancellationTokenSource?.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AI Program Generation Background Service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Create a scope for this iteration to get scoped services
                using var scope = _serviceProvider.CreateScope();
                var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                var aiProgramGeneratorService = scope.ServiceProvider.GetRequiredService<IAiProgramGeneratorService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Get next pending job
                var job = await jobRepository.GetNextPendingJobAsync();

                if (job != null)
                {
                    _logger.LogInformation("Processing job {JobId}", job.Id);
                    await ProcessJobAsync(job, jobRepository, aiProgramGeneratorService, unitOfWork, stoppingToken);
                }
                else
                {
                    // No jobs available - wait before checking again
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background job processing loop");
                // Wait before retrying to avoid tight error loop
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("AI Program Generation Background Service stopped.");
    }

    private async Task ProcessJobAsync(
        AiGenerationJob job,
        IJobRepository jobRepository,
        IAiProgramGeneratorService aiProgramGeneratorService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        try
        {
            // Update job status to Processing
            job.Status = "Processing";
            job.UpdatedAt = DateTime.UtcNow;
            jobRepository.Update(job);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Deserialize request
            var request = JsonSerializer.Deserialize<GenerateProgamRequest>(job.RequestData)
                ?? throw new InvalidOperationException("Failed to deserialize job request data");

            // Generate program using the existing service
            var result = await aiProgramGeneratorService.GenerateProgramAsync(request, cancellationToken);

            // Update job with result
            job.Status = "Completed";
            job.ProgramTemplateId = result.Id;
            job.ResultData = JsonSerializer.Serialize(result);
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            jobRepository.Update(job);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Job {JobId} completed successfully. ProgramTemplateId: {ProgramTemplateId}", 
                job.Id, result.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing job {JobId}", job.Id);

            // Update job with error
            job.Status = "Failed";
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            jobRepository.Update(job);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception saveEx)
            {
                _logger.LogError(saveEx, "Failed to save error status for job {JobId}", job.Id);
            }
        }
    }
}

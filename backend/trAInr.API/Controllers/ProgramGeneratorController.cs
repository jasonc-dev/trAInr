using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs.AI;
using trAInr.Application.DTOs.ProgramTemplate;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Entities;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgramGeneratorController : ControllerBase
{
    private readonly IJobRepository _jobRepository;
    private readonly IUnitOfWork _unitOfWork;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    public ProgramGeneratorController(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    {
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobResponse>> GenerateProgram([FromBody] GenerateProgamRequest request, CancellationToken cancellationToken)
    {
        // Create a new job
        var job = new AiGenerationJob
        {
            Id = Guid.NewGuid(),
            Status = "Pending",
            RequestData = JsonSerializer.Serialize(request, JsonOptions),
            CreatedAt = DateTime.UtcNow
        };

        await _jobRepository.AddAsync(job);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return immediately with job ID
        return Accepted(new JobResponse
        {
            JobId = job.Id,
            Status = job.Status,
            CreatedAt = job.CreatedAt
        });
    }

    [HttpGet("jobs/{jobId}")]
    [ProducesResponseType(typeof(JobStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobStatusResponse>> GetJobStatus(Guid jobId)
    {
        var job = await _jobRepository.GetByIdAsync(jobId);

        if (job == null)
        {
            return NotFound(new { message = $"Job with id {jobId} not found." });
        }

        ProgramTemplateResponse? result = null;
        if (job.Status == "Completed" && !string.IsNullOrEmpty(job.ResultData))
        {
            try
            {
                result = JsonSerializer.Deserialize<ProgramTemplateResponse>(job.ResultData, JsonOptions);
            }
            catch
            {
                // If deserialization fails, result remains null
            }
        }

        return Ok(new JobStatusResponse
        {
            JobId = job.Id,
            Status = job.Status,
            Result = result,
            ErrorMessage = job.ErrorMessage,
            CreatedAt = job.CreatedAt,
            CompletedAt = job.CompletedAt
        });
    }
}

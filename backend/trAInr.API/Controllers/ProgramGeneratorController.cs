using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using trAInr.Application.DTOs.AI;
using trAInr.Application.DTOs.ProgramTemplate;
using trAInr.Application.Interfaces;
using trAInr.Application.Interfaces.Repositories;
using trAInr.Domain.Entities;

namespace trAInr.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProgramGeneratorController(IJobRepository jobRepository, IUnitOfWork unitOfWork) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

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

        await jobRepository.AddAsync(job);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
        var job = await jobRepository.GetByIdAsync(jobId);

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

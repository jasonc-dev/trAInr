using Microsoft.AspNetCore.Mvc;
using trAInr.Application.Interfaces.Services.AI;

namespace trAInr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(IEmbeddingService embeddingService)
    : ControllerBase
{
    [HttpPost("admin/generate-embeddings")]
    public async Task<IActionResult> GenerateEmbeddings()
    {
        await embeddingService.GenerateExerciseEmbeddingsAsync();
        return Ok(new { Message = "Embeddings generation completed" });
    }
}
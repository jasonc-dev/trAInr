using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using trAInr.Application.Interfaces.Services.AI;

namespace trAInr.Infrastructure.Api;

public class OpenAiClient : IOpenAiClient
{

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateProgramTemplate(string prompt, CancellationToken ct = default)
    {
        var apiKey = _configuration["OpenAi:ApiKey"] ?? throw new InvalidOperationException("OpenAi:ApiKey is not configured");

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
          {
        new { role = "system", content = "You are a professional fitness trainer and program designer." },
        new { role = "user", content = prompt }
      }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = JsonContent.Create(requestBody),
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var responseCotent = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<OpenAiResponse>(responseCotent);

        return result?.Choices?.FirstOrDefault()?.Message?.Content ?? throw new InvalidOperationException("No content in OpenAI response.");
    }
    private class OpenAiResponse
    {
        [JsonPropertyName("choices")]
        public List<OpenAiChoice>? Choices { get; set; }
    }

    private class OpenAiChoice
    {
        [JsonPropertyName("message")]
        public OpenAiMessage? Message { get; set; }
    }

    private class OpenAiMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
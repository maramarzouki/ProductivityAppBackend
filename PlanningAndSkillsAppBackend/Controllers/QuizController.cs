using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using DotnetGeminiSDK.Client.Interfaces;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Model.GeminiModels;
using static System.Net.WebRequestMethods;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [Route("api/quiz")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly string _apiKey;
        private const string Model = "gemini-2.0-flash";  // or whatever your ListModels showed
        public QuizController(IConfiguration configuration)
        {
            _apiKey = configuration.GetSection("Gemini")["ApiKey"];
        }

        [HttpPost("generate-quiz-question")]
        public async Task<ActionResult<string>> GenerateQuizQuestion([FromBody] GeminiRequest req)
        {
            // 1. Build the prompt exactly as before
            var historyBlock = string.Join("\n\n", req.History.Select((h, i) =>
            {
                var optsJson = JsonSerializer.Serialize(h.Options);
                return $@"Question {i + 1}: {h.Question}
                    Options: {optsJson}
                    User picked: {h.UserChoice}";
            }));

            var categoriesBlock = req.Categories.Any()
                ? $"Categories to cover: {string.Join(", ", req.Categories)}\n\n"
                : string.Empty;

            var prompt = $@"
                    You are a professional quiz-master designing personality archetype questions.  
                    You have these categories (traits) to cover: {string.Join(", ", req.Categories)}.  
                    You’ve already asked the following (in order):

                    {historyBlock}

                    Now, generate only **one** new quiz question **specifically** targeting an uncovered trait from that list.  

                    **Output** **strictly** as JSON with exactly these fields—no extra keys, no explanatory text:
                    ```json
                    {{
                      ""question"": ""<A single, clear, situation-based question>"",
                      ""options"": [""<Answer A>"", ""<Answer B>"", ""<Answer C>"", ""<Answer D>""],
                      ""trait"": ""<One of the categories exactly as given>""
                    }}".Trim();

            using var http = new HttpClient();
            // Build the request URL with your key as a query parameter
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            // For v1beta2 endpoint (generateText) :contentReference[oaicite:0]{index=0}
            //var payload = new
            //{
            //    prompt = new { text = prompt }
            //};
            var payload = new
            {
                contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


            using var response = await http.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, err);
            }

            // 3. Return raw JSON response (you can parse out .candidates[0].output if you like)
            var resultJson = await response.Content.ReadAsStringAsync();
            return Ok(resultJson);
        }

        [HttpPost("generate-quiz-report")]
        public async Task<ActionResult<string>> GenerateQuizReport([FromBody] GeminiRequest req)
        {
            // 1. Build the prompt exactly as before
            var historyBlock = string.Join("\n\n", req.History.Select((h, i) =>
            {
                var optsJson = JsonSerializer.Serialize(h.Options);
                return $@"Question {i + 1}: {h.Question}
                    Options: {optsJson}
                    User picked: {h.UserChoice}";
            }));

            var categoriesBlock = req.Categories.Any()
                ? $"Categories to cover: {string.Join(", ", req.Categories)}\n\n"
                : string.Empty;

            // Build a JSON-schema snippet for each trait:
            var analysisEntries = string.Join(",\n", req.Categories.Select(trait =>
                                $@"    ""{trait}"": {{
                      ""insight"": ""<1–2 sentence summary of what their answers reveal about {trait}>"",
                      ""tip"":     ""<A practical, specific suggestion to develop or balance {trait}>""
                    }}"));

            // Wrap it in the outer analysis object:
            var analysisSchema = $@"
                    ```json
                    {{
                      ""analysis"": {{
                    {analysisEntries}
                      }}
                    }}
                    ```";

            var prompt = $@"
                    You are an expert psychologist and professional quiz-master.
                    {categoriesBlock}
                    The user has just completed a quiz. Here are the questions they answered and their choices, in order:

                    {historyBlock}

                    Based on their answers, generate a personalized personality report.
                    Output strictly as valid JSON—no extra prose—using exactly this schema:

                    {analysisSchema}

                    Also include a top-level field \""userLevels\"" which is an object mapping each trait name (from the categories) to a numeric level between 0 and 1 indicating the user's score in that category.

                    Respond strictly with valid JSON—no code fences, no markdown, no explanatory prose.".Trim();

            //var prompt = $@"
            //            You are an expert psychologist and professional quiz-master.
            //            {categoriesBlock}
            //            The user has just completed a quiz. Here are the questions they answered and their choices, in order:

            //            {historyBlock}

            //            Based on their answers, generate a personalized personality report.
            //            Output strictly as valid JSON—no extra prose—using exactly this schema:

            //            {analysisSchema}

            //              Also include a top-level field \""areasToDevelop\"" which is an array of trait names (from the categories) {categoriesBlock} where the user showed the lowest scores.

            //              Respond strictly with valid JSON—no code fences, no markdown, no explanatory prose.
            //            ".Trim();

            using var http = new HttpClient();
            // Build the request URL with your key as a query parameter
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            // For v1beta2 endpoint (generateText) :contentReference[oaicite:0]{index=0}
            //var payload = new
            //{
            //    prompt = new { text = prompt }
            //};
            var payload = new
            {
                contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");


            using var response = await http.PostAsync(url, content);
            //response.EnsureSuccessStatusCode();

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, err);
            }

            // 3. Return raw JSON response (you can parse out .candidates[0].output if you like)
            var resultJson = await response.Content.ReadAsStringAsync();
            return Ok(resultJson);
        }
    }
    //    private readonly IGeminiClient _gemini;
    //    public QuizController(IGeminiClient gemini)
    //    {
    //        _gemini = gemini;
    //    }
    //    [HttpPost("generate-quiz-question")]
    //    public async Task<ActionResult<string>> GenerateQuizQuestion([FromBody] GeminiRequest req)
    //    {
    //        // 1. Build history string with question, options, and choice
    //        var historyBlock = string.Join("\n\n", req.History.Select((h, i) =>
    //        {
    //            var optsJson = JsonSerializer.Serialize(h.Options);
    //            return $@"Question {i + 1}: {h.Question}
    //                        Options: {optsJson}
    //                        User picked: {h.UserChoice}";
    //        }));

    //        // 2. (Optional) categories
    //        var categoriesBlock = req.Categories.Any()
    //            ? $"Categories to cover: {string.Join(", ", req.Categories)}\n\n"
    //            : string.Empty;

    //        // 3. Interpolate into template
    //        var prompt = $@"You are a quiz-master helping users discover their personality archetype.
    //                    We are on question #{req.NextQuestionNumber} of 10. We have already asked:
    //                    {historyBlock}

    //                    {categoriesBlock}Now generate:
    //                    1) A JSON object with:
    //                       - `question`: une question concise.
    //                       - `options`: un tableau de 4 réponses.
    //                       - `trait`: un label interne pour le trait mesuré.

    //                    Respond ONLY in JSON. No extra text.
    //                    ".Trim();

    //        // 4. Send to Gemini

    //        var result = await _gemini.TextPrompt(prompt);

    //        return Ok(new { res = result });
    //    }
    //}
}

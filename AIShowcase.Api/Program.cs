using AiShowcase.Api.Services;
using AIShowcase.Api.Services;

namespace AIShowcase.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger - display only, no Try It Out in production
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                // Dev - wide open
                options.AddPolicy("DevPolicy", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

                // Prod - locked to portfolio domain

                //options.AddPolicy("ProdPolicy", policy =>
                //    policy.SetIsOriginAllowed(origin =>
                //    origin == "https://kind-wave-087ca330f2.azurestaticapps.net" ||
                //    origin == "https://mbruno-projects.com" ||
                //    origin == "https://www.mbruno-projects.com"
                //)
                //.AllowAnyMethod()
                //.AllowAnyHeader());



                //options.AddPolicy("ProdPolicy", policy =>
                //    policy.WithOrigins(
                //        "https://mbruno-projects.com",
                //        "https://www.mbruno-projects.com",
                //        "https://kind-wave-087ca330f2.azurestaticapps.net",
                //        "https://kind-wave-087ca330f2.azurestaticapps.net/"
                //    )
                //    .AllowAnyMethod()
                //    .AllowAnyHeader());
                options.AddPolicy("ProdPolicy", policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            });

            var endpoint = builder.Configuration["DocumentIntelligence:Endpoint"];
            var apiKey = builder.Configuration["DocumentIntelligence:ApiKey"];
            builder.Services.AddSingleton(new DocumentIntelligenceService(endpoint, apiKey));

            var langEndpoint = builder.Configuration["LanguageService:Endpoint"];
            var langApiKey = builder.Configuration["LanguageService:ApiKey"];
            builder.Services.AddSingleton(new LanguageService(langEndpoint, langApiKey));

            var searchEndpoint = builder.Configuration["AiSearch:Endpoint"];
            var searchApiKey = builder.Configuration["AiSearch:ApiKey"];
            builder.Services.AddSingleton(new SearchService(searchEndpoint, searchApiKey));

            var openAiEndpoint = builder.Configuration["AzureOpenAI:Endpoint"];
            var openAiKey = builder.Configuration["AzureOpenAI:ApiKey"];
            var openAiDeployment = builder.Configuration["AzureOpenAI:DeploymentName"];
            builder.Services.AddSingleton(new ChatService(openAiEndpoint, openAiKey, openAiDeployment));

            // Speech SDK requires native binaries - wrap to prevent startup crash
            // if native libs are unavailable on the host platform (e.g. Free tier / 32-bit)
            try
            {
                var speechKey = builder.Configuration["SpeechService:ApiKey"];
                var speechRegion = builder.Configuration["SpeechService:Region"];
                builder.Services.AddSingleton(new SpeechService(speechKey, speechRegion));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Startup] Speech service unavailable on this platform: {ex.Message}");
            }

            var visionEndpoint = builder.Configuration["VisionService:Endpoint"];
            var visionApiKey = builder.Configuration["VisionService:ApiKey"];
            builder.Services.AddSingleton(new VisionService(visionEndpoint, visionApiKey));

            builder.Services.AddSingleton<ContentSafetyService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                // Dev - full Swagger UI with Try It Out enabled
                app.UseSwaggerUI();
            }
            else
            {
                app.UseSwagger();
                // Prod - Swagger UI visible but Try It Out disabled
                app.UseSwaggerUI(c =>
                {
                    c.SupportedSubmitMethods(); // empty = no submit buttons anywhere
                });
            }

            app.UseHttpsRedirection();

            // Use DevPolicy locally, ProdPolicy in Azure
            app.UseCors(app.Environment.IsDevelopment() ? "DevPolicy" : "ProdPolicy");

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
using ChatApi;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddFastEndpoints()
   .SwaggerDocument();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddHttpClient();

builder.Services.AddSemanticKernelWithChatCompletionsAndEmbeddingGeneration(builder.Configuration);
builder.Services.AddKernelMemory(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();

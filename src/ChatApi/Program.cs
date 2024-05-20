using ChatApi;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddFastEndpoints()
   .SwaggerDocument();

builder.Services.AddSemanticKernelWithChatCompletionsAndEmbeddingGeneration();
builder.Services.AddKernelMemory(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();

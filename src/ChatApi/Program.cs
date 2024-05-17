using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
   .AddFastEndpoints()
   .SwaggerDocument();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints()
   .UseSwaggerGen();

app.Run();

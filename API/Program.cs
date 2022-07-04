using API.Rabbit;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API", Version = "v1" });
    c.IncludeXmlComments(string.Format(@"{0}\API.xml", AppDomain.CurrentDomain.BaseDirectory));
});
builder.Services.AddMediatR(new[] { Assembly.GetExecutingAssembly() } );
builder.Services.AddSingleton<IRabbitQueue, RabbitQueue>();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        (HttpStatusCode code, string responseContent) = exceptionHandlerPathFeature?.Error switch
        {
            InvalidOperationException e => (HttpStatusCode.BadRequest, $"Invalid operation: {e.Message}"),
            Exception e => (HttpStatusCode.InternalServerError, $"{e.Message}"),
            _ => (HttpStatusCode.InternalServerError, "Unknown error"),
        };

        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "text/plain";

        await context.Response.WriteAsync(responseContent);
    });
    errorApp.UseHsts();
});

app.UseAuthorization();

app.MapControllers();

app.Run();

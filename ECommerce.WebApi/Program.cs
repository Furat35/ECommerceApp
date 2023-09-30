using ECommerce.Core.Entities;
using ECommerce.Core.Exceptions;
using ECommerce.Core.Extensions;
using ECommerce.Data.Extensions;
using ECommerce.Service.Extensions;
using ECommerce.WebApi.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataLayerServices(builder.Configuration);
builder.Services.AddServiceLayerServices();
builder.Services.AddApiLayerServices(builder, builder.Configuration);
builder.Services.AddCoreLayerServices();
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Web API v1");
    });
}

//app.UseSerilogRequestLogging();
app.UseExceptionHandler(
    options =>
    {
        options.Run(async context =>
        {
            context.Response.ContentType = "application/json";
            var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();

            if (exceptionObject != null)
            {
                context.Response.StatusCode = exceptionObject.Error switch
                {
                    BadRequestException exception => StatusCodes.Status400BadRequest,
                    NotFoundException exception => StatusCodes.Status404NotFound,
                    _ => 433
                };
                var errorMessage = $"{exceptionObject.Error.Message}";
                await context.Response
                    .WriteAsync(JsonSerializer.Serialize(new HttpResponseBody(context.Response.StatusCode, exceptionObject.Error.Message)))
                    .ConfigureAwait(false);
            }
        });
    }
);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCaching();
app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

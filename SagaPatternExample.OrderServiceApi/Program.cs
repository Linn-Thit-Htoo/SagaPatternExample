using Microsoft.AspNetCore.Mvc;
using SagaPatternExample.OrderServiceApi.Extensions;
using SagaPatternExample.Utils;
using SagaPatternExample.Utils.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = actionContext =>
                    {
                        var validationErrors = actionContext.ModelState.SelectMany(x => x.Value!.Errors).Select(y => y.ErrorMessage).ToList();
                        return new OkObjectResult(new Result<List<string>>()
                        {
                            Data = validationErrors,
                            IsSuccess = false,
                            Message = "Fail",
                            StatusCode = EnumHttpStatusCode.Success
                        });
                    };
                }).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHealthChecks("/health");

app.UseAuthorization();

app.MapControllers();

app.Run();

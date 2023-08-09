using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Wrappers;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var unitOfWork = context.RequestServices.GetService<IUnitOfWork<int>>() ?? throw new ArgumentNullException();
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = await Result<string>.FailAsync(error.Message);
                switch (error)
                {
                    case ApiException e:
                        //custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case UnauthorizedException e:
                        // custom authorization error
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case ValidationException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        responseModel.Messages = e.Errors;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ForbiddenException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    default:
                        // unhanled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
                await unitOfWork.Rollback();
                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Convert to camelCase
                };
                var result = JsonSerializer.Serialize(responseModel,jsonSerializerOptions);
                await response.WriteAsync(result);
            }
        }
    }
}
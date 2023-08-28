using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomValidationFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Check if the response is a validation error response
        if (context.Result is BadRequestObjectResult badRequestResult)
        {
            // Get the validation errors
            var error = badRequestResult.Value as SerializableError;
            if (error != null)
            {
                string[] errors = error["error"] as string[];
                context.Result = new BadRequestObjectResult(Result<string>.Fail(errors[0]));
            }
            else if (badRequestResult.Value is ValidationProblemDetails validationProblemDetails)
            {
                var errors = validationProblemDetails.Errors.SelectMany(x => x.Value).ToList();
                context.Result = new BadRequestObjectResult(Result<string>.Fail(errors[0]));
            }
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // No action needed
    }
}

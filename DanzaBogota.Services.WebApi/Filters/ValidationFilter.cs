using DanzaBogota.Transversal.Common.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DanzaBogota.Services.WebApi.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var validationErrors = new List<string>();

            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.Value.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator == null) continue;

                var validationContext = new ValidationContext<object>(argument.Value);
                var result = await validator.ValidateAsync(validationContext);

                if (!result.IsValid)
                {
                    validationErrors.AddRange(result.Errors.Select(e => e.ErrorMessage));
                }
            }

            var errors = validationErrors;

            if (errors.Count == 0)
            {
                errors = new List<string>();

                foreach (var entry in context.ModelState)
                {
                    foreach (var modelError in entry.Value.Errors)
                    {
                        if (!string.IsNullOrWhiteSpace(modelError.ErrorMessage))
                        {
                            errors.Add(modelError.ErrorMessage);
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                context.Result = new BadRequestObjectResult(ApiResponse<string>.Fail(string.Join(" | ", errors.Distinct())));
                return;
            }

            await next();
        }
    }
}

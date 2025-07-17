using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CompanyPriceTracker.API.Filters {
    public class ValidationFilter<T> : IEndpointFilter {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is null)
                return await next(context);

            var model = context.Arguments.OfType<T>().FirstOrDefault();
            if (model is null)
                return Results.BadRequest("Invalid request body.");

            var result = await validator.ValidateAsync(model);
            if (!result.IsValid) {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                return Results.ValidationProblem(errors);
            }

            return await next(context);
        }
    }
}

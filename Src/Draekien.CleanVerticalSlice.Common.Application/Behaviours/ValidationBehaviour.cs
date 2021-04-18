using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace Draekien.CleanVerticalSlice.Common.Application.Behaviours
{
    /// <summary>
    /// Applies all registered validators before proceeding with request handling
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <inheritdoc />
        /// <exception cref="ValidationException">One or more validation failures occured</exception>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);
            IEnumerable<Task<ValidationResult>> validationTasks = _validators.Select(async v => await v.ValidateAsync(context, cancellationToken));
            ValidationResult[] validationResults = await Task.WhenAll(validationTasks);
            List<ValidationFailure> failures = validationResults.SelectMany(r => r.Errors)
                                                                .Where(f => f is not null)
                                                                .ToList();

            if (failures.Count > 0) throw new ValidationException(failures);

            return await next();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace Common.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <inheritdoc />
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

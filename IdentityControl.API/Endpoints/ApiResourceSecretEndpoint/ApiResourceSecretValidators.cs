using System;
using FluentValidation;
using IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Insert;
using IdentityControl.API.Endpoints.ApiResourceSecretEndpoint.Update;

namespace IdentityControl.API.Endpoints.ApiResourceSecretEndpoint
{
    public class ApiResourceSecretValidators
    {
        public class InsertApiResourceSecretRequestValidator : AbstractValidator<InsertApiResourceSecretRequest>
        {
            public InsertApiResourceSecretRequestValidator()
            {
                RuleFor(s => s.Description).MaximumLength(100);
                RuleFor(s => s.Value).NotEmpty().MaximumLength(100);
                RuleFor(s => s.Type).NotNull();
                RuleFor(s => s.Expiration).GreaterThan(DateTime.UtcNow).When(s => s.Expiration.HasValue);
            }
        }

        public class UpdateApiResourceSecretRequestValidator : AbstractValidator<UpdateApiResourceSecretRequest>
        {
            public UpdateApiResourceSecretRequestValidator()
            {
                RuleFor(s => s.Description).MaximumLength(100);
                RuleFor(s => s.ExpiresAt).GreaterThan(DateTime.UtcNow).When(s => s.ExpiresAt.HasValue);
            }
        }

        public class RegenerateApiResourceSecretRequestValidator : AbstractValidator<RegenerateApiResourceSecretRequest>
        {
            public RegenerateApiResourceSecretRequestValidator()
            {
                RuleFor(s => s.Id).NotEmpty();
                RuleFor(s => s.Value).NotEmpty().MaximumLength(100);
            }
        }
    }
}
using System;
using FluentValidation;
using IdentityControl.API.Endpoints.ClientSecretEndpoint.Insert;
using IdentityControl.API.Endpoints.ClientSecretEndpoint.Update;

namespace IdentityControl.API.Endpoints.ClientSecretEndpoint
{
    public class ClientSecretValidators
    {
        public class InsertRequestValidator : AbstractValidator<InsertClientSecretRequest>
        {
            public InsertRequestValidator()
            {
                RuleFor(s => s.Description).MaximumLength(100);
                RuleFor(s => s.Value).NotEmpty().MaximumLength(100);
                RuleFor(s => s.Type).NotNull();
                RuleFor(s => s.Expiration).GreaterThan(DateTime.UtcNow).When(s => s.Expiration.HasValue);
            }
        }

        public class UpdateRequestValidator : AbstractValidator<UpdateClientSecretRequest>
        {
            public UpdateRequestValidator()
            {
                RuleFor(s => s.Description).MaximumLength(100);
                RuleFor(s => s.ExpiresAt).GreaterThan(DateTime.UtcNow).When(s => s.ExpiresAt.HasValue);
            }
        }

        public class RegenerateRequestValidator : AbstractValidator<RegenerateClientSecretRequest>
        {
            public RegenerateRequestValidator()
            {
                RuleFor(s => s.Value).NotEmpty().MaximumLength(100);
            }
        }
    }
}
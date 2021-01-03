using FluentValidation;
using IdentityControl.API.Endpoints.ApiScopeEndpoint.Insert;
using IdentityControl.API.Endpoints.ApiScopeEndpoint.Update;

namespace IdentityControl.API.Endpoints.ApiScopeEndpoint
{
    public class ApiScopeValidators
    {
        public class InsertApiScopeValidator : AbstractValidator<InsertApiScopeRequest>
        {
            public InsertApiScopeValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Description).MaximumLength(500);
            }
        }

        public class UpdateApiScopeValidator : AbstractValidator<UpdateApiScopeRequest>
        {
            public UpdateApiScopeValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Description).MaximumLength(500);
            }
        }
    }
}
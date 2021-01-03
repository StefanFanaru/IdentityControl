using FluentValidation;
using IdentityControl.API.Endpoints.ApiResourceEndpoint.Insert;
using IdentityControl.API.Endpoints.ApiResourceEndpoint.Update;

namespace IdentityControl.API.Endpoints.ApiResourceEndpoint
{
    public class ApiResourceValidators
    {
        public class InsertApiResourceValidator : AbstractValidator<InsertApiResourceRequest>
        {
            public InsertApiResourceValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Description).MaximumLength(500);
            }
        }

        public class UpdateApiResourceValidator : AbstractValidator<UpdateApiResourceRequest>
        {
            public UpdateApiResourceValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Description).MaximumLength(500);
            }
        }
    }
}
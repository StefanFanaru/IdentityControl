using FluentValidation;
using IdentityControl.API.Asp;
using IdentityControl.API.Endpoints.ClientEndpoint.ClientChildren;
using IdentityControl.API.Endpoints.ClientEndpoint.Dtos;
using IdentityControl.API.Endpoints.ClientEndpoint.Insert;
using IdentityControl.API.Endpoints.ClientEndpoint.Update;

namespace IdentityControl.API.Endpoints.ClientEndpoint
{
    public class ClientValidators
    {
        public class InsertClientValidator : AbstractValidator<InsertClientRequest>
        {
            public InsertClientValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Description).MaximumLength(500);
                RuleFor(x => x.ClientUri).MustBeLink();
            }
        }

        public class UpdateClientValidator : AbstractValidator<UpdateClientRequest>
        {
            public UpdateClientValidator()
            {
                RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
                RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Description).MaximumLength(500);
                RuleFor(x => x.ClientUri).MustBeLink();
            }
        }

        public class ClientChildAssignmentValidator : AbstractValidator<ClientChildAssignmentRequest>
        {
            public ClientChildAssignmentValidator()
            {
                RuleFor(x => x.Value).NotEmpty().MaximumLength(100);
                RuleFor(x => x.Value).MaximumLength(100).MustBeLink().When(x => x.Type == ClientChildType.CorsOrigin);
                RuleFor(x => x.Value).MaximumLength(100).MustBeLink()
                    .When(x => x.Type == ClientChildType.RedirectUri || x.Type == ClientChildType.LogoutRedirectUri);
            }
        }
    }
}
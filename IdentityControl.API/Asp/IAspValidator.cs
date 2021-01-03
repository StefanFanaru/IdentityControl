using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IdentityControl.API.Services.ToasterEvents;

namespace IdentityControl.API.Asp
{
    public interface IAspValidator

    {
        Task<ValidatorResult<TResponse>> ValidateAsync<TRequest, TResponse, TFluentValidator>(TRequest request,
            IToasterEvent toasterEvent, CancellationToken cancellationToken = default)
            where TRequest : new()
            where TResponse : BaseResponse, new()
            where TFluentValidator : AbstractValidator<TRequest>, new();
    }
}
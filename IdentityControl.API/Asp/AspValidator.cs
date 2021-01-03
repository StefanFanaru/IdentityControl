using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IdentityControl.API.Services.SignalR;
using IdentityControl.API.Services.ToasterEvents;
using Microsoft.AspNetCore.Mvc;

namespace IdentityControl.API.Asp
{
    public class AspValidator : IAspValidator

    {
        private readonly IEventSender _eventSender;

        public AspValidator(IEventSender eventSender)
        {
            _eventSender = eventSender;
        }

        public async Task<ValidatorResult<TResponse>> ValidateAsync<TRequest, TResponse, TFluentValidator>(TRequest request,
            IToasterEvent toasterEvent, CancellationToken cancellationToken = default)
            where TRequest : new()
            where TResponse : BaseResponse, new()
            where TFluentValidator : AbstractValidator<TRequest>, new()
        {
            var response = new TResponse();
            var fluentValidator = new TFluentValidator();
            var result = new ValidatorResult<TResponse>();

            var validation = await fluentValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                response.Succeeded = false;

                foreach (var failure in validation.Errors) response.Errors.Add(failure.ErrorMessage);

                await _eventSender.SendAsync(toasterEvent.TransformInFailure());

                result.Failed = true;
                result.Response = new BadRequestObjectResult(response);
                return result;
            }

            response.Succeeded = true;
            result.Response = new OkObjectResult(response);
            return result;
        }
    }
}
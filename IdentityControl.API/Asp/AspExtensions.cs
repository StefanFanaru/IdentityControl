using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace IdentityControl.API.Asp
{
    public static class AspExtensions
    {
        public static BadRequestObjectResult GetBadRequestWithError<TResponse>(string errorMessage)
            where TResponse : BaseResponse, new()
        {
            return new BadRequestObjectResult(new TResponse
            {
                Succeeded = false,
                Errors = new List<string> {errorMessage}
            });
        }
    }
}
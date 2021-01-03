using Microsoft.AspNetCore.Mvc;

namespace IdentityControl.API.Asp
{
    public class ValidatorResult<TResponse>
    {
        public bool Failed { get; set; }
        public ActionResult<TResponse> Response { get; set; }
    }
}
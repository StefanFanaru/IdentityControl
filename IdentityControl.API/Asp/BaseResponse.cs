using System.Collections.Generic;

namespace IdentityControl.API.Asp
{
    public class BaseResponse
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
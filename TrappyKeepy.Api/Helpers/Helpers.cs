using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace TrappyKeepy.Api
{
    public class Helpers
    {
        public string ParseToken(IHeaderDictionary headers)
        {
            return headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
        }
    }
}

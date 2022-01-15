using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MovieNight.Common.Results
{
    /// <summary>
    /// An internal server result for returning objects in 
    /// an HttpContext response.
    /// </summary>
    public class InternalServerErrorObjectResult :
        IActionResult
    {
        private readonly object _payload;

        public InternalServerErrorObjectResult(object payload)
        {
            _payload = payload;
        }

        public object Value => _payload;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var obj = new ObjectResult(_payload)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            await obj.ExecuteResultAsync(context);
        }
    }
}

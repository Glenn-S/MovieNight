using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MovieNight.Common.Results
{
    /// <summary>
    /// A server error result for HttpContext responses.
    /// </summary>
    public class InternalServerErrorResult :
        IActionResult
    {
        public async Task ExecuteResultAsync(ActionContext context)
        {
            var obj = new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            await obj.ExecuteResultAsync(context);
        }
    }
}

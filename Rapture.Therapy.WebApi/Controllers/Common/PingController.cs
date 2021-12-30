using Eadent.Common.WebApi.DataTransferObjects.Common;
using Eadent.Common.WebApi.Definitions;
using Eadent.Common.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Rapture.Therapy.WebApi.Controllers.Common
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {

        [HttpGet]
        public ActionResult<PingResponseDto> Get([FromHeader(Name = EadentHeaders.SessionTokenName)] string? sessionToken)
        {
            var stopWatch = Stopwatch.StartNew();

            ActionResult<PingResponseDto> response;

            int httpStatusCode = StatusCodes.Status400BadRequest;

            var responseDto = new PingResponseDto()
            {
                RemoteIpAddress = HttpHelper.GetRemoteAddress(Request),
                SoftwareVersion = "V0.0"
            };

            if (sessionToken == null)
            {
                httpStatusCode = StatusCodes.Status200OK;
            }
            else
            {
                // TODO: VALIDATE: Signed In Session.
                responseDto.LocalIpAddress = HttpHelper.GetLocalAddress(Request);

                httpStatusCode = StatusCodes.Status200OK;
            }

            stopWatch.Stop();

            responseDto.GeneratedInMs = stopWatch.ElapsedMilliseconds;

            response = StatusCode(httpStatusCode, responseDto);

            return response;
        }
    }
}

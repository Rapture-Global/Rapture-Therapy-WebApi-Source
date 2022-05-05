using Eadent.Common.WebApi.DataTransferObjects.Common;
using Eadent.Common.WebApi.Definitions;
using Eadent.Common.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;

namespace Rapture.Therapy.WebApi.Controllers.Common
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private ILogger<PingController> Logger { get; }

        public PingController(ILogger<PingController> logger)
        {
            Logger = logger;
        }

        [HttpGet]
        public ActionResult<PingResponseDto> Ping([FromHeader(Name = EadentHeaders.SessionTokenName)] string? sessionToken)
        {
            var stopWatch = Stopwatch.StartNew();

            Logger.LogInformation($"{nameof(PingController)}: sessionToken = {sessionToken}");

            ActionResult<PingResponseDto> response;

            int httpStatusCode = StatusCodes.Status400BadRequest;

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var fileVersion = fileVersionInfo.FileVersion;

            var responseDto = new PingResponseDto()
            {
                RemoteIpAddress = HttpHelper.GetRemoteIpAddress(Request),
                SoftwareVersion = $"V{fileVersion}"
            };

            if (sessionToken == null)
            {
                responseDto.SetSuccess();

                httpStatusCode = StatusCodes.Status200OK;
            }
            else
            {
                // TODO: VALIDATE: Signed In Session.
                responseDto.LocalIpAddress = HttpHelper.GetLocalIpAddress(Request);
                responseDto.SetSuccess();

                httpStatusCode = StatusCodes.Status200OK;
            }

            stopWatch.Stop();

            responseDto.GeneratedInMs = stopWatch.ElapsedMilliseconds;

            response = StatusCode(httpStatusCode, responseDto);

            return response;
        }
    }
}

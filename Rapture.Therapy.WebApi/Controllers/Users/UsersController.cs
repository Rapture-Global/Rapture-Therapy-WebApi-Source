using Eadent.Common.WebApi.DataTransferObjects.Sessions.Users;
using Eadent.Common.WebApi.Definitions;
using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Rapture.Therapy.WebApi.Controllers.Users
{
    [Route("Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private ILogger<UsersController> Logger { get; }

        private IEadentWebApiUserIdentity UserIdentity { get; }

        public UsersController(ILogger<UsersController> logger, IEadentWebApiUserIdentity userIdentity)
        {
            Logger = logger;
            UserIdentity = userIdentity;
        }

        [HttpPost]
        public ActionResult<UserRegisterResponseDto> SignIn([FromHeader(Name = EadentHeaders.SessionTokenName)] string? sessionToken, [FromBody] UserRegisterRequestDto requestDto)
        {
            var stopWatch = Stopwatch.StartNew();

            Logger.LogInformation($"{nameof(UsersController)}: sessionToken = {sessionToken}");

            ActionResult<UserRegisterResponseDto> response;

            int httpStatusCode = StatusCodes.Status400BadRequest;

            UserRegisterResponseDto responseDto;

            if (requestDto == null)
            {
                responseDto = new UserRegisterResponseDto();
                responseDto.Set((long)CommonDeveloperCode.MissingRequestBody, "Missing Request Body.");
            }
            else if (string.IsNullOrWhiteSpace(requestDto.DisplayName))
            {
                responseDto = new UserRegisterResponseDto();
                responseDto.Set((long)CommonDeveloperCode.InvalidDisplayName, "Invalid Display Name.");
            }
            else if (string.IsNullOrWhiteSpace(requestDto.EMailAddress))
            {
                responseDto = new UserRegisterResponseDto();
                responseDto.Set((long)CommonDeveloperCode.InvalidEMailAddress, "Invalid E-Mail Address.");
            }
            else if (string.IsNullOrWhiteSpace(requestDto.PlainTextPassword))
            {
                responseDto = new UserRegisterResponseDto();
                responseDto.Set((long)CommonDeveloperCode.InvalidPassword, "Invalid Password.");
            }
            else
            {
                responseDto = UserIdentity.RegisterUser(sessionToken, requestDto, HttpHelper.GetRemoteIpAddress(Request));

                if (responseDto.DeveloperCode == (long)CommonDeveloperCode.Success)
                    httpStatusCode = StatusCodes.Status201Created;
                else
                    httpStatusCode = StatusCodes.Status400BadRequest;
            }

            stopWatch.Stop();

            responseDto.GeneratedInMs = stopWatch.ElapsedMilliseconds;

            response = StatusCode(httpStatusCode, responseDto);

            return response;
        }
    }
}

using Eadent.Common.WebApi.DataTransferObjects.Sessions.Users;
using Eadent.Common.WebApi.Definitions;
using Eadent.Common.WebApi.Helpers;
using Eadent.Identity.Access;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Rapture.Therapy.WebApi.Controllers.Users
{
    [Route("Users/Sessions")]
    [ApiController]
    public class UserSessionsController : ControllerBase
    {
        private ILogger<UserSessionsController> Logger { get; }

        private IEadentWebApiUserIdentity UserIdentity { get; }

        public UserSessionsController(ILogger<UserSessionsController> logger, IEadentWebApiUserIdentity userIdentity)
        {
            Logger = logger;
            UserIdentity = userIdentity;
        }

        [HttpPost]
        public ActionResult<UserSessionSignInResponseDto> SignIn([FromBody] UserSessionSignInRequestDto requestDto)
        {
            var stopWatch = Stopwatch.StartNew();

            ActionResult<UserSessionSignInResponseDto> response;

            int httpStatusCode = StatusCodes.Status400BadRequest;

            UserSessionSignInResponseDto responseDto;

            if (requestDto == null)
            {
                responseDto = new UserSessionSignInResponseDto();
                responseDto.Set((long)CommonDeveloperCode.MissingRequestBody, "Missing Request Body.");
            }
            else if (string.IsNullOrWhiteSpace(requestDto.EMailAddress))
            {
                responseDto = new UserSessionSignInResponseDto();
                responseDto.Set((long)CommonDeveloperCode.InvalidEMailAddress, "Invalid E-Mail Address.");
            }
            else if (string.IsNullOrWhiteSpace(requestDto.PlainTextPassword))
            {
                responseDto = new UserSessionSignInResponseDto();
                responseDto.Set((long)CommonDeveloperCode.InvalidPassword, "Invalid Password.");
            }
            else
            {
                responseDto = UserIdentity.SignInUser(requestDto, HttpHelper.GetRemoteIpAddress(Request));

                if ((responseDto.DeveloperCode == (long)CommonDeveloperCode.Success) || (responseDto.DeveloperCode == (long)CommonDeveloperCode.SuccessUserMustChangePassword))
                    httpStatusCode = StatusCodes.Status201Created;
                else
                    httpStatusCode = StatusCodes.Status400BadRequest;
            }

            stopWatch.Stop();

            responseDto.GeneratedInMs = stopWatch.ElapsedMilliseconds;

            response = StatusCode(httpStatusCode, responseDto);

            return response;
        }

        [HttpDelete]
        public ActionResult<UserSessionSignOutResponseDto> SignOut([FromHeader(Name = EadentHeaders.SessionTokenName)] string? sessionToken)
        {
            var stopWatch = Stopwatch.StartNew();

            Logger.LogInformation($"{nameof(UserSessionsController)}: sessionToken = {sessionToken}");

            ActionResult<UserSessionSignOutResponseDto> response;

            int httpStatusCode = StatusCodes.Status400BadRequest;

            UserSessionSignOutResponseDto responseDto;

            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                responseDto = new UserSessionSignOutResponseDto();
                responseDto.Set((long)CommonDeveloperCode.Unauthorised, "Unauthorised.");
                httpStatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                responseDto = UserIdentity.SignOutUser(sessionToken, HttpHelper.GetRemoteIpAddress(Request));

                if (responseDto.DeveloperCode == (long)CommonDeveloperCode.Success)
                    httpStatusCode = StatusCodes.Status200OK;
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

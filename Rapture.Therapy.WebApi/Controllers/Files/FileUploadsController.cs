using Eadent.Common.WebApi.DataTransferObjects.Files;
using Microsoft.AspNetCore.Mvc;

namespace Rapture.Therapy.WebApi.Controllers.Files
{
    [Route("Files/Uploads")]
    [ApiController]
    public class FileUploadsController : ControllerBase
    {
        private ILogger<FileUploadsController> Logger { get; }

        public FileUploadsController(ILogger<FileUploadsController> logger)
        {
            Logger = logger;
        }

        [HttpPost]
        public HttpResponseMessage UploadFile([FromBody] FileUploadRequestDto requestDto)
        {
            HttpResponseMessage response = null;
#if false
            SessionData.GeneratedInStopWatch = Stopwatch.StartNew();

            ServiceResponse fileResponse = null;

            if (requestDto == null)
            {
                fileResponse = new ServiceResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    ServiceCode = 2000,
                    Message = "Invalid Payload.",
                    MoreInfo = null,
                };
            }
            else if ((requestDto.UserId != 1) || (requestDto.Password != "Nagp4Me"))
            {
                fileResponse = new ServiceResponse()
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    ServiceCode = 3000,
                    Message = "Unauthorised.",
                    MoreInfo = null,
                };
            }
            else
            {
                try
                {
                    long serviceCode = 1000;

                    var serverPath = System.Web.Hosting.HostingEnvironment.MapPath("/App_Data");

                    var fileName = Path.Combine(serverPath, requestDto.RelativePath, requestDto.FileName);

                    bool createFile = false;

                    FileInfo fileInfo = new FileInfo(fileName);

                    if (requestDto.BlockIndex == 0) // First Block. May be overwriting existing file.
                    {
                        createFile = true;
                    }
                    else if (fileInfo.Exists)
                    {
                        if (fileInfo.Length != requestDto.SizeBeforeThisBlock)
                        {
                            serviceCode = 4000;

                            fileResponse = new ServiceResponse()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                ServiceCode = serviceCode,
                                Message = "Before Update: File Size != SizeBeforeThisBlock",
                                MoreInfo = null
                            };
                        }

                        createFile = false;
                    }
                    else
                    {
                        createFile = true;
                    }

                    byte[] blockBytes = null;

                    if (serviceCode == 1000)
                    {
                        blockBytes = Convert.FromBase64String(requestDto.BlockBase64);

                        if (requestDto.BlockHashType == "SHA-512")
                        {
                            SHA512 sha = new SHA512Cng();
                            var referenceHashBytes = sha.ComputeHash(blockBytes);
                            var referenceHashBase64 = Convert.ToBase64String(referenceHashBytes);

                            if (referenceHashBase64 != requestDto.BlockHashBase64)
                            {
                                serviceCode = 4001;

                                fileResponse = new ServiceResponse()
                                {
                                    StatusCode = (int)HttpStatusCode.BadRequest,
                                    ServiceCode = serviceCode,
                                    Message = "Hashes do not match.",
                                    MoreInfo = null
                                };
                            }
                        }
                        else
                        {
                            serviceCode = 4002;

                            fileResponse = new ServiceResponse()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                ServiceCode = serviceCode,
                                Message = "Unsupported Block Hash Type.",
                                MoreInfo = null
                            };
                        }
                    }

                    if (serviceCode == 1000)
                    {
                        if (createFile)
                        {
                            File.WriteAllBytes(fileName, blockBytes);
                        }
                        else
                        {
                            using (var stream = new FileStream(fileName, FileMode.Append))
                            {
                                stream.Write(blockBytes, 0, blockBytes.Length);
                            }
                        }

                        fileInfo = new FileInfo(fileName);

                        if (fileInfo.Length != requestDto.SizeAfterThisBlock)
                        {
                            serviceCode = 4003;

                            fileResponse = new ServiceResponse()
                            {
                                StatusCode = (int)HttpStatusCode.BadRequest,
                                ServiceCode = serviceCode,
                                Message = "After Update: File Size != SizeAfterThisBlock",
                                MoreInfo = null
                            };
                        }
                    }

                    //if (IsDebugEnabled)
                    //    Log.Debug("File Upload Request: File Name = " + request.FileName + ", Block Index = " + request.BlockIndex + ", Service Code = " + serviceCode);

                    if (serviceCode == 1000)
                    {
                        fileResponse = new ServiceResponse()
                        {
                            StatusCode = (int)HttpStatusCode.OK,
                            ServiceCode = serviceCode,
                            Message = "Success.",
                            MoreInfo = null
                        };
                    }
                }
                catch (Exception exception)
                {
                    if (IsDebugEnabled)
                        Log.Debug("Exception.", exception);

                    fileResponse = new ServiceResponse()
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        ServiceCode = 6000,
                        Message = "Exception.",
                        MoreInfo = null
                    };
                }
            }

            SessionData.GeneratedInStopWatch.Stop();

            var difference = SessionData.GeneratedInStopWatch.ElapsedMilliseconds.ToString("#,0");

            fileResponse.GeneratedIn = difference + "ms";

            var formatter = new JsonMediaTypeFormatter()
            {
                Indent = indent
            };

            if (IsDebugEnabled)
                Log.Debug("Generated In = " + fileResponse.GeneratedIn);

            //Response = Request.CreateResponse(HttpStatusCode.OK, PingResponse, "application/json");
            response = Request.CreateResponse((HttpStatusCode)fileResponse.StatusCode, fileResponse, formatter);
#endif

            return response;
        }
    }
}

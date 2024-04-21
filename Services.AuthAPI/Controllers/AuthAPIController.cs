using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebUI.Models;
using WebUI.Service.IService;
using WebUI.Utility;

namespace Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : Controller
    {
        private readonly IAuthService _authService;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegRequestDto model)
        {
            try
            {
                if(ModelState.IsValid)
                {

                //Encrypting the password
                model.Password = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(model.Password));
                ResponseDto response = await _authService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.POST,
                    Data = model,
                    Url = SD.DataProcessAPI + "/api/dataprocess/Register",
                    ContentType = SD.ContentType.MultipartFormData
                });
                if (response != null)
                {
                    if (response.IsSuccess)
                    {
                        //Token

                        return Ok(response);

                    }
                    else
                    {
                        return BadRequest(response);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
                }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
                }
            }
            catch {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                ResponseDto response = await _authService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.GET,
                    Data = null,
                    Url = SD.DataProcessAPI + $"/api/dataprocess/{model.UserName}"
                });
                if (response != null)
                {
                    if (response.IsSuccess)
                    {
                        string pwd = (string)response.Result;

                        if(model.Password == ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(pwd)))
                        {
                            //Token
                            response.Result = new AuthResponseDto()
                            {
                                Token = ""
                            };
                           

                            return Ok(response);
                        }
                        response.IsSuccess = false;
                        response.Message = "Password is incorrect";
                        return BadRequest(response);
                    }
                    else
                    {
                        return BadRequest(response);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
            }
        }
    }
}

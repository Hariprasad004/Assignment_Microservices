using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dtos;
using Services.AuthAPI.Service.IService;
using System.Text;
using WebUI.Service.IService;
using WebUI.Utility;

namespace Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPassWordHash _passWordHash;

        public AuthAPIController(IAuthService authService, IJwtTokenGenerator jwtTokenGenerator, IPassWordHash passWordHash)
        {
            _authService = authService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passWordHash = passWordHash;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegRequestDto model)
        {
            try
            {
                if(ModelState.IsValid)
                {

                    //Encrypting the password
                    model.Password = _passWordHash.GetHashedPassword(model.Password); //Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(model.Password));
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
                        return Ok(JsonConvert.SerializeObject(response));

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
                        RegRequestDto userdto = JsonConvert.DeserializeObject<RegRequestDto>(Convert.ToString(response.Result)); 

                        if (_passWordHash.VerifyPassword(model.Password, userdto.Password))
                        {
                            //Token
                            response = new ResponseDto()
                            {
                                Result = new AuthResponseDto()
                                {
                                    Token= _jwtTokenGenerator.GenerateToken(userdto)
                                }
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

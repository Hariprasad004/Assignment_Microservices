using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using Services.AuthAPI.Models;
using Services.AuthAPI.Models.Dtos;
using Services.AuthAPI.Service.IService;
using WebUI.Service.IService;
using WebUI.Utility;

namespace Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : Controller
    {
        private readonly IBaseService _authService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPassWordHash _passWordHash;

        public AuthAPIController(IBaseService authService, IJwtTokenGenerator jwtTokenGenerator, IPassWordHash passWordHash)
        {
            _authService = authService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passWordHash = passWordHash;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //Encrypting the password
                    model.Password = _passWordHash.GetHashedPassword(model.Password);
                    ResponseDto response = await _authService.SendAsync(new RequestDto()
                    {
                        ApiType = StaticDetails.ApiType.POST,
                        Data = model,
                        Url = StaticDetails.DataProcessAPI + "/api/dataprocess/Register",
                        ContentType = StaticDetails.ContentType.MultipartFormData
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
            catch(Exception ex)
            {
                Log.Error(ex.InnerException, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseDto response = await _authService.SendAsync(new RequestDto()
                    {
                        ApiType = StaticDetails.ApiType.GET,
                        Data = null,
                        Url = StaticDetails.DataProcessAPI + $"/api/dataprocess/{model.UserName}"
                    });
                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            RegisterRequestDto userdto = JsonConvert.DeserializeObject<RegisterRequestDto>(Convert.ToString(response.Result));
                            string UserPwd = "";
                            if(userdto != null)
                            {
                                UserPwd = userdto.Password;
                            }

                            if (_passWordHash.VerifyPassword(model.Password, UserPwd))
                            {
                                //Token
                                response = new ResponseDto()
                                {
                                    Result = new AuthResponseDto()
                                    {
                                        Token = _jwtTokenGenerator.GenerateToken(userdto)
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
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.InnerException, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error occured");
            }
        }
    }
}

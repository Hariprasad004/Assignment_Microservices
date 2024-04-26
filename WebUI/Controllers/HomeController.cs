using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebUI.Models;
using WebUI.Service.IService;
using WebUI.Utility;
using static WebUI.Utility.SD;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public HomeController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegRequestDto obj)
        {
            if (ModelState.IsValid)
            {
                ResponseDto result = await _authService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.POST,
                    Data = obj,
                    Url = SD.AuthAPI + "/api/auth/Register",
                    ContentType = ContentType.MultipartFormData
                });

                if (result != null)
                {
                    if (result.IsSuccess)
                    {
                        TempData["success"] = "Registration Successful";
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        TempData["error"] = result.Message;
                        return View(obj);
                    }
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            if (ModelState.IsValid)
            {
                ResponseDto responseDto = await _authService.SendAsync(new RequestDto()
                {
                    ApiType = SD.ApiType.POST,
                    Data = obj,
                    Url = SD.AuthAPI + "/api/auth/Login"
                });
                if (responseDto != null)
                {
                    if (responseDto.IsSuccess)
                    {
                        AuthResponseDto loginResponseDto =
                            JsonConvert.DeserializeObject<AuthResponseDto>(Convert.ToString(responseDto.Result));

                        await SignInUser(loginResponseDto);
                        _tokenProvider.SetToken(loginResponseDto.Token);
                        return RedirectToAction("LoginSuccessful", "Home");
                    }
                    else
                    {
                        TempData["error"] = responseDto.Message;
                        return View(obj);
                    }
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult LoginSuccessful()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
        private async Task SignInUser(AuthResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}

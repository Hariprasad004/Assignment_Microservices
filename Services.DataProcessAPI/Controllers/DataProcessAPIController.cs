using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DataProcessAPI.Data;
using Services.DataProcessAPI.Models;
using WebUI.Models;

namespace Services.DataProcessAPI.Controllers
{
    public class DataProcessAPIController : Controller
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private const string ErrMsg = "An error occurred while processing your request";

        public DataProcessAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        [Route("{email}")]
        public ResponseDto Get(string email)
        {
            try
            {
                User obj = _db.Users.FirstOrDefault(u => u.Email == email);
                _response.Result = _mapper.Map<UserDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ErrMsg;
            }
            return _response;
        }

        [HttpPost("Register")]
        public ResponseDto Register([FromBody]UserDto userdto)
        {
            try
            {
                User user = _db.Users.FirstOrDefault(u => u.Email == userdto.Email);
                if (user != null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Email id already exists";
                }
                else
                {
                     user = _mapper.Map<User>(userdto);
                    _db.Users.Add(user);
                    _db.SaveChanges();

                    if (userdto.Image != null)
                    {

                        string fileName = user.ID + Path.GetExtension(userdto.Image.FileName);
                        string filePath = @"wwwroot\UserImages\" + fileName;

                        //I have added the if condition to remove the any image with same name if that exist in the folder by any change
                        var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                        FileInfo file = new FileInfo(directoryLocation);
                        if (file.Exists)
                        {
                            file.Delete();
                        }

                        var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                        using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                        {
                            userdto.Image.CopyTo(fileStream);
                        }
                        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                        user.ImageUrl = baseUrl + "/UserImages/" + fileName;
                        user.ImageLocalPath = filePath;
                    }
                    else
                    {
                        user.ImageUrl = "https://placehold.co/600x400";
                    }
                    _db.Users.Update(user);
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<UserDto>(user);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ErrMsg;
            }
            return _response;
        }

    }
}

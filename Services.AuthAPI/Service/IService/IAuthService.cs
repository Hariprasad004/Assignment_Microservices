using Services.AuthAPI.Models.Dtos;

namespace WebUI.Service.IService
{
    public interface IAuthService
	{
		Task<ResponseDto?> SendAsync(RequestDto requestDto);
	}
}

using Services.AuthAPI.Models.Dtos;

namespace WebUI.Service.IService
{
    public interface IBaseService
	{
		Task<ResponseDto?> SendAsync(RequestDto requestDto);
	}
}

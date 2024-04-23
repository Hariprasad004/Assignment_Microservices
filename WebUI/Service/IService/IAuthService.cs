using WebUI.Models;

namespace WebUI.Service.IService
{
	public interface IAuthService
	{
		Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
	}
}

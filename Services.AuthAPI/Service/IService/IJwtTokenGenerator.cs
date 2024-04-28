using Services.AuthAPI.Models.Dtos;

namespace Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(RegisterRequestDto userDto);
    }
}

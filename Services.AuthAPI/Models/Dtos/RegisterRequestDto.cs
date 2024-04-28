using System.ComponentModel.DataAnnotations;

namespace Services.AuthAPI.Models.Dtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public int? Id { get; set; }
    }
}

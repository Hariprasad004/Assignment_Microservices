using System.ComponentModel.DataAnnotations;

namespace WebUI.Models
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
        //[MaxFileSize(1)]
        //[AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile? Image { get; set; }
    }
}

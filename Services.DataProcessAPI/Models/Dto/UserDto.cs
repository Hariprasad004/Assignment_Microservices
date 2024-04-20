namespace WebUI.Models
{
	public class UserDto
	{
		public string ID { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Services.DataProcessAPI.Models
{
	public class User
	{
        [Key]
        public int ID { get; set; }
        [Required]
		public string Email { get; set; }
		[Required]
		public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
		public DateTime DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
    }
}

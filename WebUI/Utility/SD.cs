namespace WebUI.Utility
{
	public class SD
	{
		public static string AuthAPI {  get; set; }
		public enum ApiType
		{
			GET,
			POST,
			PUT,
			DELETE
		}
		public enum ContentType
		{
			Json,
			MultipartFormData,
		}
	}
}

namespace WebUI.Utility
{
	public class StaticDetails
	{
		public static string AuthAPI {  get; set; }
        public const string TokenCookie = "JWTToken";
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

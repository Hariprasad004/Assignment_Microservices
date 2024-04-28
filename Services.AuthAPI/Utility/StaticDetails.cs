namespace WebUI.Utility
{
	public class StaticDetails
	{
		public static string DataProcessAPI {  get; set; }
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

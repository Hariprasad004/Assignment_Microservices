using Newtonsoft.Json;
using Serilog;
using Services.AuthAPI.Models.Dtos;
using System.Net;
using System.Text;
using WebUI.Service.IService;
using static WebUI.Utility.StaticDetails;

namespace WebUI.Service
{
    public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public BaseService(IHttpClientFactory httpClientFactory) 
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
		{
			try
			{
				HttpClient client = _httpClientFactory.CreateClient("WebAPI");
				HttpRequestMessage message = new();
				if (requestDto.ContentType == ContentType.MultipartFormData)
				{
					message.Headers.Add("Accept", "*/*");
				}
				else
				{
					message.Headers.Add("Accept", "application/json");
				}

				message.RequestUri = new Uri(requestDto.Url);

				if (requestDto.ContentType == ContentType.MultipartFormData)
				{
					var content = new MultipartFormDataContent();

					foreach (var prop in requestDto.Data.GetType().GetProperties())
					{
						var value = prop.GetValue(requestDto.Data);
						if (value is FormFile)
						{
							var file = (FormFile)value;
							if (file != null)
							{
								content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
							}
						}
						else
						{
							content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
						}
					}
					message.Content = content;
				}
				else
				{
					if (requestDto.Data != null)
					{
						message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
					}
				}

				HttpResponseMessage? apiResponse = null;

				switch (requestDto.ApiType)
				{
					case ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					case ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}

				apiResponse = await client.SendAsync(message);

				switch (apiResponse.StatusCode)
				{
					case HttpStatusCode.NotFound:
						return new() { IsSuccess = false, Message = "Not Found" };
					case HttpStatusCode.Forbidden:
						return new() { IsSuccess = false, Message = "Access Denied" };
					case HttpStatusCode.Unauthorized:
						return new() { IsSuccess = false, Message = "Unauthorized" };
					case HttpStatusCode.InternalServerError:
						return new() { IsSuccess = false, Message = "Internal Server Error" };
					default:
						var apiContent = await apiResponse.Content.ReadAsStringAsync();
						var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
						return apiResponseDto;
				}
			}
			catch (Exception ex)
			{
				var dto = new ResponseDto
				{
					Message = ex.Message.ToString(),
					IsSuccess = false
				};
                Log.Error(ex.InnerException, ex.Message);
                return dto;
			}
		}
	}
}

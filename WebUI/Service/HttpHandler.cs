using WebUI.Service.IService;

namespace WebUI.Service
{
    public class HttpHandler : IHttpHandler
    {
        public HttpClient client
        {
            get
            {
                return new HttpClient();
            }
        }
    }
}

using System.Net;

namespace MagicVilla.Models
{
    public class APIResponse
    {
        public HttpStatusCode statusCode {  get; set; }
        public bool isSuccess { get; set; } = true;
        public List<string> Errors { get; set; }
        public object Result { get; set; }

    }
}

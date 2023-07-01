
namespace webapplication.Models
{
    public class ResponseApi
    {
        public int success { get; set; }

        public string message { get; set; }

        public string? error { get; set; }

        public dynamic? data { get; set; }
        public dynamic? data2 { get; set; }
        public dynamic? data3 { get; set; }
        public dynamic? data4 { get; set; }
    }
}

using System.Data;

namespace webapplication.Models
{
    public class ResponseDB
    {
        public int success { get; set; }
        public string message { get; set; }
        public string? error { get; set; }
        public dynamic? data { get; set; }
        public DataTable? datatable { get; set; }
        public dynamic? data3 { get; set; }
        public dynamic? data4 { get; set; }
    }
}

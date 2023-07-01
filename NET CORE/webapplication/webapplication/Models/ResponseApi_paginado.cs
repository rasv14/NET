
namespace webapplication.Models
{
    public class ResponseApi_paginado
    {
        public int success { get; set; }

        public string message { get; set; }

        public string? error { get; set; }

        public dynamic? data { get; set; }
        public dynamic? data2 { get; set; }
        public dynamic? data3 { get; set; }
        public dynamic? data4 { get; set; }

        public clslinks links { get; set; }

        public clsmeta meta { get; set; }

        public class clslinks 
            
        {
            public string first { get; set; }
            public string previous { get; set; }
            public string next { get; set; }
            public string last { get; set; }
        }


        public class clsmeta

        {
            public string currentPage { get; set; }
            public string itemCount { get; set; }
            public string itemsPerPage { get; set; }
            public string totalItems { get; set; }

            public string totalPages { get; set; }
        }
    }
}




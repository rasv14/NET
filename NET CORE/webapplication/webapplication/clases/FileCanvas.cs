using System;
using System.Collections.Generic;
using System.Text;

namespace webapplication.clases
{
   public class FileCanvas
    {
        public int id { get; set; }
        public int? folder_id { get; set; }
        
        public string? display_name { get; set; }
        public string? filename { get; set; }
        public string? upload_status { get; set; }

       // public string content-type { get; set; }
        public string url { get; set; }
        public Int64? size { get; set; }
        public string? created_at { get; set; }
        public string? updated_at { get; set; }
        public string? mime_class { get; set; }

        public UserCanvas? user { get; set; }

}




}




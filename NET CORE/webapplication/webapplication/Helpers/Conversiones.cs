using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using webapplication.Handler;

namespace webapplication.Helpers
{
    public static class Conversiones
    {
        public static List<Dictionary<string, object>> DataTableToJson(this DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    if (row[col].ToString().StartsWith('{') || row[col].ToString().StartsWith('['))
                    {
                        dict[col.ColumnName] = JsonConvert.DeserializeObject(row[col].ToString());
                    }
                    else
                    {
                        dict[col.ColumnName] = row[col].ToString().Length == 0 ? "" : row[col];
                    }
                }
                list.Add(dict);
            }
            return list;
        }        

        public static string getTokenFromHeader(HeadersParameters header)
        {            
            string[] authorization = header.Authorization.Split(" ");
            return authorization[1];
        }
    }
}

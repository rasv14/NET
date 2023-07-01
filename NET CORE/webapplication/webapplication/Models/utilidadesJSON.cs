using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace webapplication.Models
{
    public class utilidadesJSON
    {
        public string DictionaryToString(Dictionary<string, string> P_DATA)
        {
            string separator = "¦";
            string subseparator = "»";
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in P_DATA)
            {
                sb.Append(separator);
                sb.Append(kvp.Key);
                sb.Append(subseparator);
                sb.Append(kvp.Value);
                // Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value)
            }

            return sb.ToString();
        }
    }
}

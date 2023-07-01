
using Microsoft.Extensions.Configuration;
namespace webapplication.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string? Ip { get; set; }
    }

  
}

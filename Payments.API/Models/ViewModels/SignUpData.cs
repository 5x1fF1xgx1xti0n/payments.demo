using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.ViewModels
{
    public class SignUpData
    {
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}

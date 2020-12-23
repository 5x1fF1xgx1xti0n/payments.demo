using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public AccountViewModel Account { get; set; }
        public string Role { get; set; }
    }
}

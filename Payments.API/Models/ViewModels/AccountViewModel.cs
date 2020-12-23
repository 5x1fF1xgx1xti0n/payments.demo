using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public decimal Sum { get; set; }
        public UserViewModel User { get; set; }
    }
}

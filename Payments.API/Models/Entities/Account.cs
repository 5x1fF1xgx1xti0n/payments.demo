using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string EnctyptedSum { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool Deleted { get; set; }
    }
}

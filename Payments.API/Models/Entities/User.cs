using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string Role { get; set; }
        public bool Deleted { get; set; }
    }
}

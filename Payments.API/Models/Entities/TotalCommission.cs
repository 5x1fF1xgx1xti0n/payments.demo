using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.Entities
{
    public class TotalCommission
    {
        public int Id { get; set; }
        public string EncryptedValue { get; set; }
    }
}

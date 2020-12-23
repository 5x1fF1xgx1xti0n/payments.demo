using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.Entities.EncryptedModels
{
    public class TransactionDetails
    {
        public DateTime Date { get; set; }
        public decimal RawSum { get; set; }
        public decimal CommissionSum { get; set; }
        public decimal TotalSum { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }
}

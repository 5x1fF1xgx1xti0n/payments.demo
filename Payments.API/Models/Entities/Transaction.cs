using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string EncryptedTransactionDetails { get; set; }
        public bool Deleted { get; set; }
    }
}

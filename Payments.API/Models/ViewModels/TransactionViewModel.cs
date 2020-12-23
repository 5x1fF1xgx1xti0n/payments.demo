using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.ViewModels
{
    public class TransactionViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal RawSum { get; set; }
        public decimal CommissionSum { get; set; }
        public decimal TotalSum { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
    }
}

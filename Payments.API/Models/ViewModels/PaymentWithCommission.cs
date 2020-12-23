using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payments.API.Models.ViewModels
{
    public class PaymentWithCommission
    {
        public PaymentWithCommission(decimal raw, decimal commission)
        {
            Raw = raw;
            Commission = commission;
        }

        public decimal Raw { get; set; }
        public decimal Commission { get; set; }
        public decimal Total => Raw + Commission;
    }
}

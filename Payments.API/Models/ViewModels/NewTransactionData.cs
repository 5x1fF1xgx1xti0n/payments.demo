namespace Payments.API.Models.ViewModels
{
    public class NewTransactionData
    {
        public decimal RawSum { get; set; }
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
    }
}

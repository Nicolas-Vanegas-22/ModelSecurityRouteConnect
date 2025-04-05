using System;

namespace Entity.Model
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Activity { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
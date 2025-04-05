using System;

namespace Entity.Model
{
    public class PaymentDTO
    {
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Activity { get; set; }
    }
}


using System;

namespace Entity.DTO
{
    public class PaymentDTO
    {
        public string PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Activity { get; set; }
        public string Description { get; set; }
    }
}


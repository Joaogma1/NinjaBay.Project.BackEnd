﻿using System;

namespace NinjaBay.Domain.CommandViewModels
{
    public class PaymentCardViewModel
    {
        public string Number { get; set; }

        public string Cvv { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
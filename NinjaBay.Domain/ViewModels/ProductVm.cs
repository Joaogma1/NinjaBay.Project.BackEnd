﻿using System;
using System.Collections.Generic;

namespace NinjaBay.Domain.ViewModels
{
    public class ProductVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<KeyWordVm> KeyWords { get; set; } = new List<KeyWordVm>();
        public IEnumerable<ProductImageVm> Links { get; set; } = new List<ProductImageVm>();
        public IEnumerable<ProductQAVm> QuestionAndAnswers { get; set; } = new List<ProductQAVm>();
    }
}
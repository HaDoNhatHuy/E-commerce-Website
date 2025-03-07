﻿using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class StatisticalChartModel
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

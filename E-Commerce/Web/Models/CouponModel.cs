﻿using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CouponModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên coupon")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả coupon")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số lượng coupon")]
        public int Quantity { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }
        public int Status { get; set; }
        public decimal Discount {  get; set; }
    }
}

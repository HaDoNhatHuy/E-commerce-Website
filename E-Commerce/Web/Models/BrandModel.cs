﻿using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        //[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên thương hiệu")]
        [Required(ErrorMessage = "Yêu cầu nhập tên thương hiệu")]
        public string Name { get; set; }
        public string Slug { get; set; }
        //[Required,MinLength(4,ErrorMessage ="Yêu cầu nhập mô tả thương hiệu")]
        [Required(ErrorMessage = "Yêu cầu nhập mô tả thương hiệu")]
        public string Description { get; set; }
        public int Status { get; set; }
    }
}

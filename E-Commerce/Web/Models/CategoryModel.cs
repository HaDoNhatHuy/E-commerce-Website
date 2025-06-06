﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        //[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên Danh mục")]
        [Required(ErrorMessage ="Yêu cầu nhập tên danh mục")]
        public string Name { get; set; }
        //[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Mô tả Danh mục")]
        [Required(ErrorMessage = "Yêu cầu nhập mô tả danh mục")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}

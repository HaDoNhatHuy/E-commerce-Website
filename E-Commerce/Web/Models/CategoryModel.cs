﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models
{
    [Table("Category")]
    public class CategoryModel
    {
        [Key]
        public Guid Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên Danh mục")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Mô tả Danh mục")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}

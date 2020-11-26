using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BAL.Models
{

    public class Category
    {
        [Key]
        public int CategoryRowId { get; set; }
        [Required(ErrorMessage= "Category Id is Must")]
        [StringLength(20,ErrorMessage = "Category Id can be max 20 characters")]
        public string CategoryId { get; set; }
        [Required(ErrorMessage = "Category Name is Must")]
        [StringLength(200, ErrorMessage = "Category Name can be max 200 characters")]
        public string CategoeyName { get; set; }
        [Required(ErrorMessage = "Base Price is Must")]
        public int BasePrice { get; set; }
        // Expected one-to-many relationship
        public ICollection<Product> Products { get; set; }
    }

    public class Product
    {
        [Key] // Promary Identity Key
        public int ProductRowId { get; set; }
        [Required(ErrorMessage = "Product Id is Must")]
        [StringLength(20, ErrorMessage = "Product Id can be max 20 characters")]
        public string ProductId { get; set; }
        [Required(ErrorMessage = "Product Name is Must")]
        [StringLength(200, ErrorMessage = "Product Name can be max 200 characters")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Manufacturer is Must")]
        [StringLength(20, ErrorMessage = "Manufacturer can be max 20 characters")]
        public string Manufacturer { get; set; }
        [Required]
        //  [NonNegativeValidator(ErrorMessage = "Value cannot be -ve")]
        public int Price { get; set; }
        [ForeignKey("CategoryRowId")] // Foreign Key
        public int CategoryRowId { get; set; }
        // referential Integrity
        public Category Category { get; set; }

    }
    public class NonNegativeValidatorAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (Convert.ToInt32(value) < 0) return false; // invalid
            return true; // valid
        }
    }
}

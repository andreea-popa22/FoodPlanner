using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodPlaner.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int ReceipeId { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public float Score { get; set; }
        [Required(ErrorMessage = "Continutul comentariului dumneavoastra este obligatoriu!")]
        public string Text { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
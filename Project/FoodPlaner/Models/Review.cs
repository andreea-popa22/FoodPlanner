using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodPlaner.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public int RecipeId { get; set; }
        public string UserId { get; set; }
        public float Score { get; set; }
        [Required(ErrorMessage = "The review content is mandatory!")]
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
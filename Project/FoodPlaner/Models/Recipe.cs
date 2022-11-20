using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodPlaner.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Ingredients { get; set; }
        public int Time { get; set; }
        public bool Intolerances { get; set; }
        public string Cuisine { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}
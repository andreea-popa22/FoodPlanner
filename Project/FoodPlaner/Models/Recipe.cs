using FoodPlaner.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodPlaner.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }
        [Required(ErrorMessage = "Please enter the recipe name.")]
        public string RecipeName { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter the ingredients")]
        public string Ingredients { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter the time")]
        public int Time { get; set; }
        public string Intolerances { get; set; }
        public List<string> IntolerancesList { get; set; }
        [Required(ErrorMessage = "Please enter the cusine or select none")]
        public Cusines Cuisine { get; set; }

        public byte[] Photo { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public Recipe() { }

        public Recipe(int id, string recipeName, string userId, string ingredients, string description, int time, string intolerances, Cusines cuisine, byte[] photo, List<string> intolerancesList = null)
        {
            RecipeId = id;
            RecipeName = recipeName;
            UserId = userId;
            Ingredients = ingredients;
            Description = description;
            Time = time;
            Intolerances = intolerances;
            Cuisine = cuisine;
            Photo = photo;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FoodPlaner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public int BodyIndex { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
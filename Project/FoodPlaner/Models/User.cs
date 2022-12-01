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
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
using FoodPlaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPlaner.Repositories
{
    public interface IRecipeRepository : IDisposable
    {
        IEnumerable<Recipe> GetRecipes();
        Recipe GetRecipeByID(int recipeId);
        ApplicationUser GetUserByRecipeID(string userId);
        List<Review> GetRecipeReviewsByID(int recipeId);
        void InsertRecipe(Recipe recipe);
        void DeleteRecipe(int recipeId);
        void UpdateRecipe(Recipe recipe);
        void Save();
    }
}
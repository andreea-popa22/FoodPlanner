using FoodPlaner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodPlaner.Repositories
{
    public class RecipeRepository : IRecipeRepository, IDisposable
    {
        private ApplicationDbContext context;
        public RecipeRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return context.Recipes.ToList();
        }

        public ApplicationUser GetUserByRecipeID(string id)
        {
            return context.Users.Find(id);
        }

        public List<Review> GetRecipeReviewsByID(int recipeId)
        {
            return context.Reviews.Where(r => r.RecipeId == recipeId).ToList();
        }

        public Recipe GetRecipeByID(int id)
        {
            return context.Recipes.Find(id);
        }

        public void InsertRecipe(Recipe recipe)
        {
            recipe.Intolerances = recipe.IntolerancesList == null ? null : string.Join(", ", recipe.IntolerancesList);
            context.Recipes.Add(recipe);
        }

        public void DeleteRecipe(int recipeId)
        {
            Recipe recipe = context.Recipes.Find(recipeId);
            context.Recipes.Remove(recipe);
        }

        public void UpdateRecipe(Recipe recipe)
        {
            context.Entry(recipe).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

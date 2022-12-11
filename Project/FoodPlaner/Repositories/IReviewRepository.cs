using FoodPlaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodPlaner.Repositories
{
    public interface IReviewRepository : IDisposable
    {
        IEnumerable<Review> GetReviews();
        Review GetReviewByID(int reviewId);
        ApplicationUser GetUserByReviewID(string userId);
        Recipe GetRecipeByReviewID(int recipeId);
        void InsertReview(Review review);
        void DeleteReview(int reviewId);
        void UpdateReview(Review review);
        void Save();
    }
}
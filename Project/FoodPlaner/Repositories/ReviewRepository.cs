using FoodPlaner.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FoodPlaner.Repositories
{
    public class ReviewRepository : IReviewRepository, IDisposable
    {
        private ApplicationDbContext context;
        public ReviewRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Review> GetReviews()
        {
            return context.Reviews.ToList();
        }

        public Recipe GetRecipeByReviewID(int id)
        {
            return context.Recipes.Find(id);
        }

        public ApplicationUser GetUserByReviewID(string id)
        {
            return context.Users.Find(id);
        }

        public Review GetReviewByID(int id)
        {
            return context.Reviews.Find(id);
        }

        public void InsertReview(Review review)
        {
            context.Reviews.Add(review);
        }

        public void DeleteReview(int reviewId)
        {
            Review review = context.Reviews.Find(reviewId);
            context.Reviews.Remove(review);
        }

        public void UpdateReview(Review review)
        {
            context.Entry(review).State = EntityState.Modified;
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
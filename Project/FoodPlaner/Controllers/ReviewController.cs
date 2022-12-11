using FoodPlaner.Models;
using FoodPlaner.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FoodPlaner.Controllers
{
    public class ReviewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        private IReviewRepository reviewRepository;

        public ReviewController()
        {
            this.reviewRepository = new ReviewRepository(new ApplicationDbContext());
        }
        public ReviewController(IReviewRepository reviewRepository)
        {
            this.reviewRepository = reviewRepository;
        }


        //GET
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        //POST
        [HttpPost]
        public ActionResult New(Review review, int recipeId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    review.UserId = User.Identity.GetUserId();
                    review.RecipeId = recipeId;
                  
                    review.Date = DateTime.Now;
                    reviewRepository.InsertReview(review);
                    reviewRepository.Save();
                    return Redirect("/Recipe/Show/" + recipeId);
                }
                else
                {
                    return Redirect("/Recipe/Show/" + recipeId);
                }

            }
            catch (Exception e)
            {
                return Redirect("/Recipe/Show/" + recipeId);
            }
        }

        // GET: Recipe
        public ActionResult Show(int reviewId)
        { 
            Review review = reviewRepository.GetReviewByID(reviewId);
            ApplicationUser user = reviewRepository.GetUserByReviewID(review.UserId);

            ViewBag.userName = user.Name + " " + user.Surname;
            return View(review);
        }

        [HttpDelete]
        public ActionResult Delete(int reviewId)
        {
            Review review = reviewRepository.GetReviewByID(reviewId);
            Recipe recipe = reviewRepository.GetRecipeByReviewID(review.RecipeId);
            reviewRepository.DeleteReview(reviewId);
            reviewRepository.Save();

            return Redirect("/Recipe/Show/" + recipe.RecipeId);
        }
        protected override void Dispose(bool disposing)
        {
            reviewRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
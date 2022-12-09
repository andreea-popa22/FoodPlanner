using FoodPlaner.Models;
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

        public ReviewController()
        {

        }
        public ReviewController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //GET
        [HttpGet]
        public ActionResult New()
        {
            var previousUrl = int.Parse(Request.UrlReferrer.AbsolutePath.Split('/').Select(x => x).Last());
            ViewBag.recipeId = previousUrl;
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
                    review.RecipeId = recipeId;
                    review.UserId = User.Identity.GetUserId();
                    review.Date = DateTime.Now;
                    db.Reviews.Add(review);
                    db.SaveChanges();
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
        public ActionResult Show(int id)
        {
            Review review = db.Reviews.Find(id);
            ApplicationUser user = db.Users.Find(review.UserId);
            ViewBag.userName = user.Name + " " + user.Surname;
            return View(review);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Review review = db.Reviews.Find(id);
            Recipe recipe = db.Recipes.Find(review.RecipeId);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return Redirect("/Recipe/Show/" + recipe.RecipeId);
        }
    }
}
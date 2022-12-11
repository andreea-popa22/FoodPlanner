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
            return View();
        }

        // GET: Recipe
        public ActionResult Show(int id)
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            reviewRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
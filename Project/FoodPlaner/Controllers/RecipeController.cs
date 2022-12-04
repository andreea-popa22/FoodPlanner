using FoodPlaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodPlaner.Controllers
{
    public class RecipeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Recipes
        public ActionResult Index()
        {
            return View(db.Recipes.ToList());
        }

        // GET: Recipe
        public ActionResult Show(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            User user = db.MyUsers.Find(recipe.UserId);
            ViewBag.userName = user.Name + " " + user.Surname;
            return View(recipe);
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return Redirect("/Recipe/Index");
        }
    }
}
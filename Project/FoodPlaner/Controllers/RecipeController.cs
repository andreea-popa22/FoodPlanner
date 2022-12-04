using FoodPlaner.Models;
using Microsoft.AspNet.Identity;
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

        //GET
        [HttpGet]
        public ActionResult New()
        { 
            return View();
        }

        //POST
        [HttpPost]
        public ActionResult New(Recipe recipe)
        {
            recipe.UserId = User.Identity.GetUserId();

            try
            {
                if (ModelState.IsValid)
                {
                    db.Recipes.Add(recipe);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(recipe);
                }

            }
            catch(Exception e)
            {
                return View(recipe);
            }
        }

        // GET: Recipe
        public ActionResult Show(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            ApplicationUser user = db.Users.Find(recipe.UserId);
            ViewBag.userName = user.Name + " " + user.Surname;
            return View(recipe);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Recipe recipe = db.Recipes.Find(id);

            return View(recipe);
        }

        [HttpPost]
        public ActionResult Edit(int id, Recipe requestRecipe)
        {
            try
            {
                Recipe recipe = db.Recipes.Find(id);
                if (TryUpdateModel(recipe))
                {
                    recipe.RecipeName = requestRecipe.RecipeName;
                    recipe.Ingredients = requestRecipe.Ingredients;
                    recipe.Intolerances = requestRecipe.Intolerances;
                    recipe.Time = requestRecipe.Time;
                    recipe.Cuisine = requestRecipe.Cuisine;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestRecipe);
                }
            }
            catch(Exception e)
            {
                return View(requestRecipe);
            }
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
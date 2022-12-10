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
        private int _perPage = 3;
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<ApplicationUser> _userManager;

        public RecipeController()
        {

        }
        public RecipeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: Recipes
        public ActionResult Index(string search, string sorted, string Chose)
        {
            ViewBag.sorted = sorted; 
            var recipes = db.Recipes.ToList();
            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                recipes = db.Recipes.Where(rp => rp.RecipeName.Contains(search))
                           .ToList();
            }

            var totalItems = recipes.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginateRecipes = sorted != "sorted" ? recipes.OrderBy(rp => rp.RecipeName)
                                  .Skip(offset)
                                  .Take(this._perPage)
                                  : recipes.OrderBy(rp => rp.Time)
                                  .Skip(offset)
                                  .Take(this._perPage);
            ViewBag.total = totalItems;
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Recipes = paginateRecipes;
            ViewBag.SearchString = search;

        return View();
        }

        public ActionResult SortByTime(string search)
        {
            if (TempData["sorted"] != "sorted")
            {
                TempData["sorted"] = "sorted";
            }
            else
            {
                TempData["sorted"] = "";
            }
            return RedirectToAction("Index");
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

        [HttpPost]
        public ActionResult Chose()
        {
            return RedirectToAction("Index");
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
                    recipe.UserId = User.Identity.GetUserId();

                    recipe.RecipeName = requestRecipe.RecipeName;
                    recipe.Ingredients = requestRecipe.Ingredients;
                    recipe.Description = requestRecipe.Description;
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
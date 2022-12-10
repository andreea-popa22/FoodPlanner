using FoodPlaner.Models;
using FoodPlaner.Repositories;
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
        //private ApplicationDbContext db = new ApplicationDbContext();
        private readonly UserManager<ApplicationUser> _userManager;
        private IRecipeRepository recipeRepository;


        public RecipeController()
        {
            this.recipeRepository = new RecipeRepository(new ApplicationDbContext());
        }
        public RecipeController(IRecipeRepository recipeRepository)
        {
            this.recipeRepository = recipeRepository;
        }
        public RecipeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: Recipes
        public ActionResult Index(string search, string sorted, string ddFilterOption)
        {
            ViewBag.sorted = sorted;
            ViewBag.ddlOption = ddFilterOption;
            var recipes = from r in recipeRepository.GetRecipes()
                          select r;
            if (search != null)
            {
                search = search.Trim();
                recipes = from r in recipeRepository.GetRecipes()
                          where r.RecipeName.Contains(search)
                          select r;
                //recipes = db.Recipes.Where(rp => rp.RecipeName.Contains(search))
                //           .ToList();
            }


            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            switch (ddFilterOption)
            {
                case "1":
                    recipes = recipes.Where(rp => rp.Time >= 15 && rp.Time < 30).ToList();
                    break;
                case "2":
                    recipes = recipes.Where(rp => rp.Time >= 30 && rp.Time < 60).ToList();
                    break;
                case "3":
                    recipes = recipes.Where(rp => rp.Time >= 60 && rp.Time < 90).ToList();
                    break;
                case "4":
                    recipes = recipes.Where(rp => rp.Time > 90).ToList();
                    break;
                default:
                    break;
            }

            var totalItems = recipes.Count();
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
                    recipeRepository.InsertRecipe(recipe);
                    recipeRepository.Save();
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
            Recipe recipe = recipeRepository.GetRecipeByID(id);
            ApplicationUser user = recipeRepository.GetUserByRecipeID(recipe.UserId);
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
            Recipe recipe = recipeRepository.GetRecipeByID(id);

            return View(recipe);
        }

        [HttpPost]
        public ActionResult Edit(int id, Recipe requestRecipe)
        {
            try
            {
                Recipe recipe = recipeRepository.GetRecipeByID(id);
                if (TryUpdateModel(recipe))
                {
                    recipe.UserId = User.Identity.GetUserId();

                    recipe.RecipeName = requestRecipe.RecipeName;
                    recipe.Ingredients = requestRecipe.Ingredients;
                    recipe.Description = requestRecipe.Description;
                    recipe.Intolerances = requestRecipe.Intolerances;
                    recipe.Time = requestRecipe.Time;
                    recipe.Cuisine = requestRecipe.Cuisine;
                    recipeRepository.UpdateRecipe(recipe);
                    recipeRepository.Save();
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
            Recipe recipe = recipeRepository.GetRecipeByID(id);
            recipeRepository.DeleteRecipe(id);
            recipeRepository.Save();
            return Redirect("/Recipe/Index");
        }
        protected override void Dispose(bool disposing)
        {
            recipeRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
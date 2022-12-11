using FoodPlaner.Models;
using FoodPlaner.Repositories;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public IEnumerable<Recipe> getFilteredRecipes(string searchText, IEnumerable<Recipe> recipes)
        {
            if(searchText == null)
            {
                return recipes;
            }
            return recipes.Where(r => r.RecipeName.Contains(searchText));
        }

        // GET: Recipes
        public async Task<ActionResult> Index(string search, string sorted, string ddFilterOption)
        {
            return View();
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
            catch (Exception e)
            {
                return View(recipe);
            }
        }

        // GET: Recipe
        public async Task<ActionResult> Show(int id)
        {
            Recipe recipe = recipeRepository.GetRecipeByID(id);
            ApplicationUser user = recipeRepository.GetUserByRecipeID(recipe.UserId);
            ViewBag.userName = user.Name + " " + user.Surname;
            ViewBag.loggedUserId = User.Identity.GetUserId();
            return View(recipe);
        }

        [HttpPost]
        public ActionResult Chose()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int recipeId)
        {
            Recipe recipe = recipeRepository.GetRecipeByID(recipeId);

            return View(recipe);
        }

        [HttpPost]
        public ActionResult Edit(int recipeId, Recipe requestRecipe)
        {
            try
            {
                Recipe recipeToBeEdited = recipeRepository.GetRecipeByID(recipeId);
                if (TryUpdateModel(recipeToBeEdited))
                {
                    recipeToBeEdited.UserId = User.Identity.GetUserId();
                    recipeToBeEdited.RecipeName = requestRecipe.RecipeName;
                    recipeToBeEdited.Description = requestRecipe.Description;
                    recipeToBeEdited.Time = requestRecipe.Time;
                    recipeToBeEdited.Cuisine = requestRecipe.Cuisine;
                    recipeToBeEdited.Ingredients = requestRecipe.Ingredients;
                    recipeToBeEdited.Intolerances = requestRecipe.Intolerances;
                    recipeRepository.UpdateRecipe(recipeToBeEdited);
                    recipeRepository.Save();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestRecipe);
                }
            }
            catch (Exception e)
            {
                return View(requestRecipe);
            }
        }

        [HttpDelete]
        public ActionResult Delete(int recipeId)
        {
            recipeRepository.DeleteRecipe(recipeId);
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
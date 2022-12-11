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
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Runtime.CompilerServices;

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

        public IEnumerable<Recipe> getFilteredRecipes(string search, IEnumerable<Recipe> recipes)
        {
            return recipes;
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
            return View();
        }

        // GET: Recipe
        public async Task<ActionResult> Show(int id)
        {
            return View();
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
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            recipeRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}
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
        private ApplicationDbContext db = new ApplicationDbContext();
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
            if (search == null)
            {
                return recipes;
            }
            search = search.Trim();
            search = search.ToLower();
            var filteredRecipes = recipes.Where(r => r.RecipeName.Contains(search));
            return filteredRecipes;
        }

        // GET: Recipes
        public async Task<ActionResult> Index(string search = "", string sorted = "", string ddFilterOption ="")
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Account/Login");
            }
                ViewBag.sorted = sorted;
            TempData["ddlOption"] = ddFilterOption == "" ? "0" : ddFilterOption;
            //var dbRecipes = from r in recipeRepository.GetRecipes()
            //              select r;

            var dbRecipes = db.Recipes.ToList();

            //UNCOMMENT THOSE 2 LINES FOR CALLING THE API
            List<Recipe> APIRecipes = await GetRecipesFromAPI();
            var recipes = dbRecipes.Concat((IEnumerable<Recipe>)APIRecipes);

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                recipes = recipes.Where(rp => rp.RecipeName.ToLower().Contains(search.ToLower()))
                           .ToList();
            }

            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            switch (ddFilterOption)
            {
                case "1":
                    recipes = recipes.Where(rp => rp.Time < 30).ToList();
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


            var paginateRecipes = sorted != "sorted" ? recipes.OrderByDescending(rp => rp.RecipeId)
                                  .Skip(offset)
                                  .Take(this._perPage)
                                  : recipes.OrderBy(rp => rp.RecipeName)
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
            catch (Exception e)
            {
                return View(recipe);
            }
        }

        // GET: Recipe
        public async Task<ActionResult> Show(int id)
        {
            Recipe recipe = recipeRepository.GetRecipeByID(id);
            if (recipe == null)
            {
                recipe = await getRecipeById(id);
                recipe.Reviews = recipeRepository.GetRecipeReviewsByID(id);
            }
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
            catch (Exception e)
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

        [HttpGet]
        public async Task<List<Recipe>> GetRecipesFromAPI()
        {
            List<Recipe> recipesList = new List<Recipe>();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/random?&number=20"),
                Headers = { { "X-RapidAPI-Key", getAPIKey() },
                            { "X-RapidAPI-Host", getAPIHost() }}
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                dynamic body = JsonConvert.DeserializeObject<dynamic>(result);
                foreach (dynamic obj in body.recipes)
                {
                    Recipe recipe = parseObjToRecipe(obj);
                    recipesList.Add(recipe);
                }
            }
            //ViewBag.Recipes = recipesList;
            return recipesList;
        }

        public Recipe parseObjToRecipe(dynamic obj)
        {
            int id = obj.id;
            string ingredients = "";
            foreach (dynamic ing in obj.extendedIngredients)
            {
                ingredients += ing.name;
                ingredients += ",";
            }
            ingredients = ingredients.Remove(ingredients.Length - 1);
            int time = obj.readyInMinutes;
            string description = obj.instructions;
            bool intolerances = obj.glutenFree;
            string name = obj.title;
            string cuisine = "Universal";
            if (obj.cuisines.Count > 0)
            {
                cuisine = obj.cuisines[0];
            }

            Recipe recipe = new Recipe(id, name, "default", ingredients, description, time, intolerances, cuisine);
            return recipe;
        }

        public async Task<Recipe> getRecipeById(int id)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/" + id + "/information"),
                Headers =
                {
                    { "X-RapidAPI-Key", getAPIKey() },
                    { "X-RapidAPI-Host", getAPIHost() },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(body);
                Recipe recipe = parseObjToRecipe(obj);
                return recipe;
            }
        }

        public string getAPIKey()
        {
            string workingDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var path = workingDirectory + @"\sensitive-data.json";
            using (StreamReader r = new StreamReader(path))
            {
                string readText = r.ReadToEnd();
                dynamic json = JObject.Parse(readText);
                return json.APIKey;
            }
        }

        public string getAPIHost()
        {
            string workingDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var path = workingDirectory + @"\sensitive-data.json";
            using (StreamReader r = new StreamReader(path))
            {
                string readText = System.IO.File.ReadAllText(path);
                dynamic json = JObject.Parse(readText);
                return json.APIHost;
            }
        }

        public async Task<ActionResult> Random()
        {
            List<Recipe> APIRecipes = await getRandomRecipes();
            ViewBag.Recipes = APIRecipes;
            return View();
        }

        public async Task<List<Recipe>> getRandomRecipes()
        {
            List<Recipe> recipesList = new List<Recipe>();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/recipes/random?&number=3"),
                Headers = { { "X-RapidAPI-Key", getAPIKey() },
                            { "X-RapidAPI-Host", getAPIHost() }}
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().Result;
                dynamic body = JsonConvert.DeserializeObject<dynamic>(result);
                foreach (dynamic obj in body.recipes)
                {
                    Recipe recipe = parseObjToRecipe(obj);
                    recipesList.Add(recipe);
                }
            }
            return recipesList;
        }
    }
}
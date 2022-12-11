﻿using FoodPlaner.Controllers;
using FoodPlaner.Models;
using FoodPlaner.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FoodPlaner.Tests.Controllers
{
    [TestClass]
    public class RecipeControllerTest
    {
        // for context
        private RecipeController recipeController;
        private Mock<IRecipeRepository> recipeRepository;
        private Mock<HttpRequestBase> request;

        private IEnumerable<Recipe> recipes;
        private const int defaultRecipeId = 1;
        private const string defaultUserId = "123";
        
        // constructor
        public RecipeControllerTest()
        {
            // test data
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.NameIdentifier, defaultUserId),
            }, "mock"));
            recipes = new List<Recipe>
            {
                new Recipe {RecipeId = 1, Ingredients = "Mazare, Pui, Sos de rosii", Time = 30, Intolerances = true, Cuisine = "Romania", RecipeName = "Tocanita de mazare", UserId = defaultUserId},
                new Recipe {RecipeId = 2, Ingredients = "Cartofi, Dovlecei, Sos de rosii", Time = 32, Intolerances = false, Cuisine = "Romania", RecipeName = "Tocanita de cartofi", UserId = defaultUserId},
                new Recipe {RecipeId = 3, Ingredients = "Vita, Sos de rosii, Cartofi, Morcovi", Time = 349, Intolerances = false, Cuisine = "Romania", RecipeName = "Gulas de vita", UserId = defaultUserId}
            };

            // mock httpContext
            request = new Mock<HttpRequestBase>();
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Request).Returns(request.Object);
            httpContext.Setup(c => c.User).Returns(user);

            // mock controllerContext
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(c => c.HttpContext).Returns(httpContext.Object);

            // mock recipeController
            recipeRepository = new Mock<IRecipeRepository>();
            recipeRepository.Setup(m => m.GetRecipes()).Returns(recipes);
            recipeController = new RecipeController(recipeRepository.Object);
            recipeController.ControllerContext = controllerContext.Object;
        }

        [TestMethod]
        public void RecipeController_ViewRecipes_ReturnsSuccess()
        {
            // mock request parameters
            var requestParams = new NameValueCollection { { "search", "" }, { "page", "1" } };
            request.Setup(r => r.Params).Returns(requestParams);

            string search = "";
            string filter = "0";
            string sorted = null;

            Task<ActionResult> result = recipeController.Index(search, filter, sorted) as Task<ActionResult>;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RecipeController_AddRecipe_ReturnsSuccess()
        {
            var recipe = new Recipe { RecipeId = defaultRecipeId, Ingredients = "Vinete, Sos de rosii", Time = 35, Intolerances = false, Cuisine = "Romania", RecipeName = "Tocanita de vinete" };
            RedirectToRouteResult result = recipeController.New(recipe) as RedirectToRouteResult;

            Assert.IsNotNull(result);
        }

        //[TestMethod]
        //public void RecipeController_AddRecipe_ReturnsFailure()
        //{
        //    var recipe = new Recipe {};
        //    RedirectToRouteResult result = recipeController.New(recipe) as RedirectToRouteResult;
        //    recipeRepository.Setup(m => m.InsertRecipe(recipe)).Raises()

        //    Assert.IsNull(result);
        //}

        [TestMethod]
        public void RecipeController_SearchRecipesByNull_ReturnsAll()
        {
            string searchText = null;
            var searcedRecipes = recipeController.getFilteredRecipes(searchText, recipes);

            Assert.AreEqual(searcedRecipes.Count(), recipes.Count());
        }

        [TestMethod]
        public void RecipeController_SearchRecipes_ReturnsNone()
        {
            int recipesToReturn = 0;
            string searchText = "nada";
            var searcedRecipes = recipeController.getFilteredRecipes(searchText, recipes);

            Assert.AreEqual(searcedRecipes.Count(), recipesToReturn);
        }

        [TestMethod]
        public void RecipeController_SearchRecipes_ReturnsFew()
        {
            int recipesToReturn = 1;
            string searchText = "Gulas";
            var searcedRecipes = recipeController.getFilteredRecipes(searchText, recipes);

            Assert.AreEqual(searcedRecipes.Count(), recipesToReturn);
        }

        [TestMethod]
        public void RecipeController_SearchRecipes_ReturnsAll()
        {
            int recipesToReturn = recipes.Count();
            string searchText = "";
            var searcedRecipes = recipeController.getFilteredRecipes(searchText, recipes);

            Assert.AreEqual(searcedRecipes.Count(), recipesToReturn);
        }

        [TestMethod]
        public void RecipeController_ShowRecipe_ReturnsSuccess()
        {

            var user = new ApplicationUser { Name = "Gheorghe", Surname = "Ion" };
            recipeRepository.Setup(m => m.GetRecipeByID(defaultRecipeId)).Returns(recipes.ElementAt(0));
            recipeRepository.Setup(m => m.GetUserByRecipeID(defaultUserId)).Returns(user);

            Task<ActionResult> result = recipeController.Show(defaultRecipeId) as Task<ActionResult>;

            Assert.IsNotNull(result);
        }

    }
}

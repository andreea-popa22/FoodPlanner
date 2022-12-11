using FoodPlaner.Controllers;
using FoodPlaner.Models;
using FoodPlaner.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FoodPlaner.Tests.Controllers
{
    [TestClass]
    public class RecipeControllerTest
    {
        private RecipeController _recipeController;
        private Mock<IRecipeRepository> _recipeRepository;
        private Mock<HttpRequestBase> _mockRequest;
        private IEnumerable<Recipe> _recipes;

        private const string userId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c";

        public RecipeControllerTest()
        {
            // Test date
            _recipes = new List<Recipe>
            {
                new Recipe {RecipeId = 27, Ingredients = "Varza, Mamaliga", Time = 55, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c"},
                new Recipe {RecipeId = 12, Ingredients = "Carne, Orez, Apa, Legume", Time = 77, Intolerances = true, Cuisine = "romania", RecipeName = "Ciorba de Perisoare", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c"},
                new Recipe {RecipeId = 23, Ingredients = "Varza, Mamaliga, Smantana", Time = 64, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale cu smantana", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c"},
                new Recipe {RecipeId = 28, Ingredients = "Varza, Mamaliga, Soia", Time = 85, Intolerances = false, Cuisine = "romania", RecipeName = "Sarmale de post", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c"},
                new Recipe {RecipeId = 51, Ingredients = "Leurda, Orez, Apa, Legume", Time = 77, Intolerances = true, Cuisine = "romania", RecipeName = "Ciorba de Leurda", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c"},
            };
            // Dependencies
            _recipeRepository = new Mock<IRecipeRepository>();
            _mockRequest = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();
            var mockControllerContext = new Mock<ControllerContext>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "test"),
                new Claim(ClaimTypes.NameIdentifier, userId),
            }, "mock"));
            mockHttpContext.Setup(c => c.Request).Returns(_mockRequest.Object);
            mockHttpContext.Setup(c => c.User).Returns(user);
            mockControllerContext.Setup(c => c.HttpContext).Returns(mockHttpContext.Object);

            // SUT
            _recipeController = new RecipeController(_recipeRepository.Object);
            _recipeController.ControllerContext = mockControllerContext.Object;
        }

        [TestMethod]
        public void RecipeController_Index_ReturnsSuccess()
        {

            // Arrange
            var requestParams = new NameValueCollection { { "search", "" }, { "page", "1" } };
            _mockRequest.Setup(r => r.Params).Returns(requestParams);

            _recipeRepository.Setup(m => m.GetRecipes()).Returns(_recipes);

            string search = "";
            string sorted = null;
            string ddFilterOption = "0";

            // Act
            Task<ActionResult> result = _recipeController.Index(search, sorted, ddFilterOption) as Task<ActionResult>;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RecipeController_FilterRecipes_ReturnsAll()
        {

            // Arrange
            _recipeRepository.Setup(m => m.GetRecipes()).Returns(_recipes);
            string search = "";

            // Act
            var newRecipes = _recipeController.getFilteredRecipes(search, _recipes);

            // Assert
            Assert.AreEqual(_recipes.Count(), newRecipes.Count());
            Assert.AreEqual(newRecipes.Where(r => r.RecipeName.Contains(search)).Count(), newRecipes.Count());
        }

        [TestMethod]
        public void RecipeController_FilterRecipes_ReturnsSome()
        {

            // Arrange
            _recipeRepository.Setup(m => m.GetRecipes()).Returns(_recipes);
            string search = "Ciorba";
            int expectedCount = 2;

            // Act
            var newRecipes = _recipeController.getFilteredRecipes(search, _recipes);

            // Assert
            Assert.AreEqual(expectedCount, newRecipes.Count());
            Assert.AreEqual(newRecipes.Where(r => r.RecipeName.Contains(search)).Count(), newRecipes.Count());
        }

        [TestMethod]
        public void RecipeController_FilterRecipes_ReturnsNone()
        {

            // Arrange
            _recipeRepository.Setup(m => m.GetRecipes()).Returns(_recipes);
            string search = "xczscz";
            int expectedCount = 0;

            // Act
            var newRecipes = _recipeController.getFilteredRecipes(search, _recipes);

            // Assert
            Assert.AreEqual(expectedCount, newRecipes.Count());
            Assert.AreEqual(newRecipes.Where(r => r.RecipeName.Contains(search)).Count(), newRecipes.Count());
        }

        [TestMethod]
        public void RecipeController_Show_ReturnsSuccess()
        {

            // Arrange
            var recipeId = 27;
            var recipes = new List<Recipe>
            {
                new Recipe {RecipeId = 27, Ingredients = "Varza, Mamaliga", Time = 55, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c"}
            };

            var recipeUserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c";
            var user = new ApplicationUser { Name = "Gheorghe", Surname = "Ion" };
            _recipeRepository.Setup(m => m.GetRecipeByID(recipeId)).Returns(recipes[0]);
            _recipeRepository.Setup(m => m.GetUserByRecipeID(recipeUserId)).Returns(user);


            // Act
            Task<ActionResult> result = _recipeController.Show(recipeId) as Task<ActionResult>;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RecipeController_New_ReturnsSuccess()
        {

            // Arrange
            var recipe = new Recipe { RecipeId = 27, Ingredients = "Varza, Mamaliga", Time = 55, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale" };

            // Act
            RedirectToRouteResult result = _recipeController.New(recipe) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RecipeController_Delete_ReturnsSuccess()
        {

            // Arrange
            var recipeId = 27;
            var recipe = new Recipe { RecipeId = 27, Ingredients = "Varza, Mamaliga", Time = 55, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale", UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c" };

            // Act
            RedirectResult result = _recipeController.Delete(recipeId) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RecipeController_Edit_ReturnsSuccess()
        {
            // Arange
            var recipeId = 27;
            _recipeRepository.Setup(m => m.GetRecipeByID(recipeId)).Returns(_recipes.ElementAt(0));

            // Act
            ViewResult result = _recipeController.Edit(recipeId) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
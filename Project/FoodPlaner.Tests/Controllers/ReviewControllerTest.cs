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
    public class ReviewControllerTest
    {
        private ReviewController recipeController;
        private Mock<IReviewRepository> reviewRepository;
        private Mock<HttpRequestBase> request;

        private const string defaultUserName = "User";
        private const string defaultUserSurName = "Test";
        private const string defaultUserId = "123";

        private const int defaultReviewId = 2;
        private const int defaultRecipeId = 1;
        private Recipe defaultRecipe = new Recipe { RecipeId = defaultRecipeId, Ingredients = "Mazare, Pui, Sos de rosii", Time = 30, Intolerances = true, Cuisine = "Romania", RecipeName = "Tocanita de mazare", UserId = defaultUserId };

        private IEnumerable<Review> reviews;

        public ReviewControllerTest()
        {
            // Test Data
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, defaultUserId),
                new Claim(ClaimTypes.Name, defaultUserName + " " + defaultUserSurName),
            }, "mock"));

            reviews = new List<Review>
            {
                new Review {ReviewId = defaultReviewId, RecipeId = defaultRecipeId, UserId = defaultUserId, Score = 1f, Text = "Nesatisfacator", Date = new System.DateTime()},
                new Review {ReviewId = defaultReviewId + 1, RecipeId = defaultRecipeId, UserId = defaultUserId, Score = 4f, Text = "O reteta foarte buna", Date = new System.DateTime()}
            };

            // Mock Http Context
            request = new Mock<HttpRequestBase>();
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Request).Returns(request.Object);
            httpContext.Setup(c => c.User).Returns(user);

            // Mock Controller Context
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(c => c.HttpContext).Returns(httpContext.Object);

            // Mock Recipe Controller
            reviewRepository = new Mock<IReviewRepository>();
            reviewRepository.Setup(m => m.GetReviewByID(defaultReviewId)).Returns(reviews.ElementAt(0));
            recipeController = new ReviewController(reviewRepository.Object)
            {
                ControllerContext = controllerContext.Object
            };
        }

        [TestMethod]
        public void ReviewController_AddReviewInitial_ReturnsSuccess()
        {
            ActionResult result = recipeController.New() as ActionResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReviewController_AddReview_ReturnsSuccess()
        {
            RedirectResult result = recipeController.New(reviews.ElementAt(0), defaultRecipeId) as RedirectResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReviewController_AddReview_FailsModelState()
        {
            recipeController.ModelState.AddModelError("key", "error message");
            RedirectResult result = recipeController.New(reviews.ElementAt(0), defaultRecipeId) as RedirectResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReviewController_ShowReview_ReturnsSuccess()
        {
            var user = new ApplicationUser { Name = defaultUserName, Surname = defaultUserSurName };
            reviewRepository.Setup(m => m.GetUserByReviewID(defaultUserId)).Returns(user);

            ActionResult result = recipeController.Show(defaultReviewId) as ActionResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReviewController_Delete_ReturnsSuccess()
        {
            reviewRepository.Setup(m => m.GetRecipeByReviewID(defaultRecipeId)).Returns(defaultRecipe);
            RedirectResult result = recipeController.Delete(defaultReviewId) as RedirectResult;
            Assert.IsNotNull(result);
        }
    }
}
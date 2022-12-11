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
        private ReviewController _recipeController;
        private Mock<IReviewRepository> _recipeRepository;
        private Mock<HttpRequestBase> _mockRequest;

        private const string userId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c";

        public ReviewControllerTest()
        {
            // Dependencies
            _recipeRepository = new Mock<IReviewRepository>();
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
            _recipeController = new ReviewController(_recipeRepository.Object);
            _recipeController.ControllerContext = mockControllerContext.Object;
        }


        [TestMethod]
        public void ReviewController_Show_ReturnsSuccess()
        {

            // Arrange
            var reviewId = 2;
            var reviews = new List<Review>
            {
                new Review {ReviewId = 2, RecipeId = 3, UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c", Score = 3.0f, Text = "Nu imi place", Date = new System.DateTime()}
            };

            var recipeUserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c";
            var user = new ApplicationUser { Name = "Gheorghe", Surname = "Ion" };
            _recipeRepository.Setup(m => m.GetReviewByID(reviewId)).Returns(reviews[0]);
            _recipeRepository.Setup(m => m.GetUserByReviewID(recipeUserId)).Returns(user);


            // Act
            ActionResult result = _recipeController.Show(reviewId) as ActionResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReviewController_New_ReturnsSuccess()
        {

            // Arrange
            var reviewId = 2;
            var recipeId = 27;
            var review = new Review { ReviewId = reviewId, RecipeId = recipeId, UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c", Score = 3.0f, Text = "Nu imi place", Date = new System.DateTime() };
            var recipe = new Recipe { RecipeId = recipeId, Ingredients = "Varza, Mamaliga", Time = 55, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale" };

            // Act
            RedirectResult result = _recipeController.New(review, recipeId) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ReviewController_Delete_ReturnsSuccess()
        {

            // Arrange
            var reviewId = 2;
            var recipeId = 27;
            var review = new Review { ReviewId = reviewId, RecipeId = recipeId, UserId = "8f353bc1-f788-4e26-aaec-fb202fb3d22c", Score = 3.0f, Text = "Nu imi place", Date = new System.DateTime() };
            var recipe = new Recipe { RecipeId = recipeId, Ingredients = "Varza, Mamaliga", Time = 55, Intolerances = true, Cuisine = "romania", RecipeName = "Sarmale" };
            _recipeRepository.Setup(m => m.GetRecipeByReviewID(recipeId)).Returns(recipe);
            _recipeRepository.Setup(m => m.GetReviewByID(reviewId)).Returns(review);

            // Act
            RedirectResult result = _recipeController.Delete(reviewId) as RedirectResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
using ApiLazyDoc.Controllers;
using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Models.User;
using ApiLazyDoc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ApiLazyDoc.XUnit
{
    public class UserControllerTest
    {
        public UserControllerTest()
        {
        }


        [Fact]
        public async Task Register_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var mockRequest = new Mock<RegistrationRequest>();
            var mockRepo = new Mock<IUserService>();
            mockRepo.Setup(repo => repo.Add(mockRequest.Object))
                    .ReturnsAsync(false);
            var controller = new UserController(mockRepo.Object, null);

            // Act
            var result = await controller.Register(model: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnOkObjectResult()
        {
            // Arrange
            var mockRequest = new Mock<AuthenticateRequest>();
            var mockRepo = new Mock<IUserService>();
            mockRepo.Setup(repo => repo.Authenticate(mockRequest.Object))
                .ReturnsAsync(GetAuthenticateResponse());

            var controller = new UserController(mockRepo.Object, null);

            // Act
            var result = await controller.Login(mockRequest.Object);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<AuthenticateResponse>(
                okResult.Value);
        }

        private AuthenticateResponse GetAuthenticateResponse()
        {
            return new AuthenticateResponse(new EntityUser(), "mysupertoken");
        }
    }
}

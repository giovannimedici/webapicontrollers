using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace TodoApi.Tests;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtSettings _jwtSettingsValue;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _jwtSettingsValue = new JwtSettings
        {
            SecretKey = "test_secret_key",
            Issuer = "test_issuer",
            Audience = "test_audience",
            ExpirationMinutes = 60
        };
        _jwtSettings = Options.Create(_jwtSettingsValue);
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["JwtSettings:SecretKey"]).Returns(_jwtSettingsValue.SecretKey);
        _configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns(_jwtSettingsValue.Issuer);
        _configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns(_jwtSettingsValue.Audience);
    }

    [Fact]
    public async Task CreateUser_WithValidUser_ReturnsCreatedAtAction()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@email.com", Password = "123" };
        _userServiceMock.Setup(s => s.CreateUserAsync(user)).ReturnsAsync(user);
        var controller = new UserController(_userServiceMock.Object, _jwtSettings, _configurationMock.Object);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtAction = (CreatedAtActionResult)result.Result;
        createdAtAction.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task CreateUser_WhenUserAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@email.com", Password = "123" };
        _userServiceMock.Setup(s => s.CreateUserAsync(user)).ThrowsAsync(new InvalidOperationException("Usuário já existe."));
        var controller = new UserController(_userServiceMock.Object, _jwtSettings, _configurationMock.Object);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        result.Result.Should().BeOfType<ConflictObjectResult>();
        var conflictResult = (ConflictObjectResult)result.Result;
        conflictResult.Value.Should().BeEquivalentTo(new { message = "Usuário já existe." });
    }

    [Fact]
    public async Task CreateUser_WithInvalidUser_ReturnsBadRequest()
    {
        // Arrange
        var user = new User { Id = "1", Email = "", Password = "" };
        _userServiceMock.Setup(s => s.CreateUserAsync(user)).ThrowsAsync(new ArgumentException("Email é obrigatório."));
        var controller = new UserController(_userServiceMock.Object, _jwtSettings, _configurationMock.Object);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = (BadRequestObjectResult)result.Result;
        badRequest.Value.Should().BeEquivalentTo(new { message = "Email é obrigatório." });
    }

    [Fact]
    public async Task CreateUser_WhenUnexpectedException_ReturnsStatus500()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@email.com", Password = "123" };
        _userServiceMock.Setup(s => s.CreateUserAsync(user)).ThrowsAsync(new Exception("Erro inesperado."));
        var controller = new UserController(_userServiceMock.Object, _jwtSettings, _configurationMock.Object);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result.Result;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { message = "Erro inesperado." });
    }

    // [Fact]
    // public async Task Login_WithValidCredentials_ReturnsToken()
    // {
    //     // Arrange
    //     var loginRequest = new TodoApi.Controllers.LoginRequest { Email = "test@email.com", Password = "123" };
    //     var user = new User { Id = "1", Email = loginRequest.Email, Password = loginRequest.Password };
    //     _userServiceMock.Setup(s => s.LoginAsync(loginRequest.Email, loginRequest.Password)).ReturnsAsync(user);
    //     var controller = new UserController(_userServiceMock.Object, _jwtSettings, _configurationMock.Object);

    //     // Act
    //     var result = await controller.Login(loginRequest);


    //     // Assert
    //     result.Result.Should().BeOfType<OkObjectResult>();
    //     var okResult = (OkObjectResult)result.Result;
    //     okResult.Value.Should().BeAssignableTo<object>();
    //     var tokenProperty = okResult.Value.GetType().GetProperty("token");
    //     tokenProperty.Should().NotBeNull();
    //     var tokenValue = tokenProperty.GetValue(okResult.Value) as string;
    //     tokenValue.Should().NotBeNullOrEmpty();
    // }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new TodoApi.Controllers.LoginRequest { Email = "test@email.com", Password = "wrong" };
        _userServiceMock.Setup(s => s.LoginAsync(loginRequest.Email, loginRequest.Password)).ThrowsAsync(new Exception("User or password incorrect"));
        var controller = new UserController(_userServiceMock.Object, _jwtSettings, _configurationMock.Object);

        // Act
        var result = await controller.Login(loginRequest);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorized = (UnauthorizedObjectResult)result.Result;
        unauthorized.Value.Should().BeEquivalentTo(new { message = "User or password incorrect" });
    }
}
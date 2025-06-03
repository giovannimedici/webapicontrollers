using System.Threading.Tasks;
using FluentAssertions;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;


namespace TodoApi.Tests;

public class TodoItemsControllerTests
{
    private Mock<IMongoDbService> _repositoryStub = new Mock<IMongoDbService>();

    public TodoItemsControllerTests()
    {
        _repositoryStub = new Mock<IMongoDbService>();
    }

    [Fact]
    //public void UnitOfWork_StateUnderTest_ExpectedBahavior
    public async Task GetTodoItem_WithUnexistingItem_ReturnsNotFound()
    {
        // arrange
        _repositoryStub.Setup(repo => repo.GetItemById("27")).ReturnsAsync((TodoItem?)null);
        var controller = new TodoItemsController(_repositoryStub.Object);
        suco

        // act
        var result = await controller.GetTodoItem("27");

        // assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetTodoItems_ReturnsAllItems()
    {
        // arrange
        var items = new List<TodoItem> { new TodoItem { Id = "1", Name = "Teste", IsComplete = false } };
        _repositoryStub.Setup(repo => repo.GetAllItensAsync()).ReturnsAsync(items);
        var controller = new TodoItemsController(_repositoryStub.Object);

        // act
        var result = await controller.GetTodoItems();

        // assert
        result.Value.Should().BeEquivalentTo(
            items,
            options => options.ComparingByMembers<TodoItem>());
    }

    [Fact]
    public async Task GetTodoItem_WithExistingItem_ReturnsItem()
    {
        // arrange
        var item = new TodoItem { Id = "1", Name = "Teste", IsComplete = false };
        _repositoryStub.Setup(repo => repo.GetItemById("1")).ReturnsAsync(item);
        var controller = new TodoItemsController(_repositoryStub.Object);

        // act
        var result = await controller.GetTodoItem("1");

        // assert
        result.Value.Should().BeEquivalentTo(
            item,
            options => options.ComparingByMembers<TodoItem>());
    }

    [Fact]
    public async Task GetTodoItem_WithMatchingItems_ReturnMatchingItems()
    {
        // arrange
        List<TodoItem> allItems = new List<TodoItem>
        {
            new TodoItem(){ Name = "Study"},
            new TodoItem(){ Name = "Sleep"},
            new TodoItem(){ Name = "Study coding"}
        };

        string nameToMatch = "Study";

        _repositoryStub.Setup(repo => repo.GetAllItensAsync()).ReturnsAsync(allItems);
        var controller = new TodoItemsController(_repositoryStub.Object);

        // act
        var result = await controller.GetTodoItems(nameToMatch);

        // assert
        result.Value.Should().OnlyContain(
            item => item.Name == allItems[0].Name || item.Name == allItems[2].Name
        );
    }

    [Fact]
    public async Task PutTodoItem_WithUnexistingItem_ReturnsNotFound()
    {
        // arrange
        _repositoryStub.Setup(repo => repo.GetItemById("1")).ReturnsAsync((TodoItem?)null);
        var controller = new TodoItemsController(_repositoryStub.Object);
        var updatedItem = new TodoItem { Id = "1", Name = "Atualizado", IsComplete = true };

        // act
        var result = await controller.PutTodoItem("1", updatedItem);

        // assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PutTodoItem_WithExistingItem_ReturnsNoContent()
    {
        // arrange
        var item = new TodoItem { Id = "1", Name = "Teste", IsComplete = false };
        _repositoryStub.Setup(repo => repo.GetItemById("1")).ReturnsAsync(item);
        _repositoryStub.Setup(repo => repo.UpdateTodoItemAsync("1", It.IsAny<TodoItem>())).Returns(Task.CompletedTask);
        var controller = new TodoItemsController(_repositoryStub.Object);
        var updatedItem = new TodoItem { Id = "1", Name = "Atualizado", IsComplete = true };

        // act
        var result = await controller.PutTodoItem("1", updatedItem);

        // assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task PostTodoItem_ReturnsCreatedAtAction()
    {
        // arrange
        var item = new TodoItem { Id = "1", Name = "Novo", IsComplete = false };
        _repositoryStub.Setup(repo => repo.CreateAsync(item)).Returns(Task.CompletedTask);
        var controller = new TodoItemsController(_repositoryStub.Object);

        // act
        var result = await controller.PostTodoItem(item);

        // assert
        result.Should().BeOfType<ActionResult<TodoItem>>();
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)result.Result;
        createdAtActionResult.ActionName.Should().Be("GetTodoItem");
        createdAtActionResult.Value.Should().BeEquivalentTo(item);
    }

    [Fact]
    public async Task DeleteTodoItem_WithUnexistingItem_ReturnsNotFound()
    {
        // arrange
        _repositoryStub.Setup(repo => repo.GetItemById("1")).ReturnsAsync((TodoItem?)null);
        var controller = new TodoItemsController(_repositoryStub.Object);

        // act
        var result = await controller.DeleteTodoItem("1");

        // assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteTodoItem_WithExistingItem_ReturnsNoContent()
    {
        // arrange
        var item = new TodoItem { Id = "1", Name = "Teste", IsComplete = false };
        _repositoryStub.Setup(repo => repo.GetItemById("1")).ReturnsAsync(item);
        _repositoryStub.Setup(repo => repo.DeleteAsync("1")).Returns(Task.CompletedTask);
        var controller = new TodoItemsController(_repositoryStub.Object);

        // act
        var result = await controller.DeleteTodoItem("1");

        // assert
        result.Should().BeOfType<NoContentResult>();
    }
}
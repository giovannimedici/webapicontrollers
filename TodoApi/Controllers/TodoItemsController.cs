using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemsService _todoItemsService;
        public TodoItemsController(ITodoItemsService todoItemsService)
        {
            _todoItemsService = todoItemsService;
        }

        // GET: api/TodoItems
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems(string? nameToMatch = null)
        {
            var items = await _todoItemsService.GetAllItensAsync();

            if (!string.IsNullOrEmpty(nameToMatch))
            {
                items = items.Where(xs => xs.Name.Contains(nameToMatch)).ToList();
            }

            return items;
        }



        // GET: api/TodoItems/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(string id)
        {
            var todoItem = await _todoItemsService.GetItemById(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(string id, TodoItem updatedTodoItem)
        {
            var item = await _todoItemsService.GetItemById(id);

            if (item is null)
            {
                return NotFound();
            }

            updatedTodoItem.Id = item.Id;

            await _todoItemsService.UpdateTodoItemAsync(id, updatedTodoItem);

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            await _todoItemsService.CreateAsync(todoItem);

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(string id)
        {
            var todoItem = await _todoItemsService.GetItemById(id);

            if (todoItem is null)
            {
                return NotFound();
            }

            await _todoItemsService.DeleteAsync(id);

            return NoContent();
        }


    }
}

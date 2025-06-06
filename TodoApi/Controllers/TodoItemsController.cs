using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly IMongoDbService _mongodbService;
        public TodoItemsController(IMongoDbService mongodbService)
        {
            _mongodbService = mongodbService;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems(string? nameToMatch = null)
        {
            var items = await _mongodbService.GetAllItensAsync();

            if (!string.IsNullOrEmpty(nameToMatch))
            {
                items = items.Where(xs => xs.Name.Contains(nameToMatch)).ToList();
            }

            return items;
        }

        

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(string id)
        {
            var todoItem = await _mongodbService.GetItemById(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(string id, TodoItem updatedTodoItem)
        {
            var item = await _mongodbService.GetItemById(id);

            if (item is null)
            {
                return NotFound();
            }

            updatedTodoItem.Id = item.Id;

            await _mongodbService.UpdateTodoItemAsync(id, updatedTodoItem);

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            await _mongodbService.CreateAsync(todoItem);

            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(string id)
        {
            var todoItem = await _mongodbService.GetItemById(id);

            if (todoItem is null)
            {
                return NotFound();
            }

            await _mongodbService.DeleteAsync(id);

            return NoContent();
        }


    }
}

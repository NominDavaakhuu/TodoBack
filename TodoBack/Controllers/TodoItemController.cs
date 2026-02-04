using Shared.DTOs.TodoItem;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TodoBack.Services.Interfaces;

namespace TodoBack.Controllers
{
    [RoutePrefix("api/todoItem")]
    public class TodoItemController : ApiController
    {
        private readonly ITodoItemService _service;

        public TodoItemController(ITodoItemService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // ---------------------------
        // Helper: get current userId (int stored in session)
        // ---------------------------
        private long CurrentUserId
        {
            get
            {
                var session = HttpContext.Current?.Session;
                if (session == null || session["UserId"] == null)
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);

                return (long)(int)session["UserId"];
            }
        }

        // ========================================
        // GET api/todoItem   (get all todos)
        // ========================================
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetMyTodos()
        {
            long userId = CurrentUserId;

            var response = await _service.GetByUserIdAsync(userId);

            // NOTE: service always returns ApiResponse<T>
            return Ok(response.Data);
        }

        // ========================================
        // GET api/todoItem/{id}
        // ========================================
        [HttpGet]
        [Route("{id:long}", Name = "GetTodoById")]
        public async Task<IHttpActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Invalid todo id.");

            long userId = CurrentUserId;

            var response = await _service.GetByIdAsync(id, userId);

            if (!response.Success || response.Data == null)
                return NotFound();

            return Ok(response.Data);
        }

        // ========================================
        // POST api/todoItem  (create)
        // ========================================
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] TodoCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Payload is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            long userId = CurrentUserId;

            var response = await _service.CreateAsync(userId, dto);

            if (!response.Success)
                return BadRequest(response.Error);

            return CreatedAtRoute(
                "GetTodoById",
                new { id = response.Data.Id },
                response.Data
            );
        }

        // ========================================
        // PUT api/todoItem/{id}  (update)
        // ========================================
        [HttpPut]
        [Route("{id:long}")]
        public async Task<IHttpActionResult> Update(long id, [FromBody] UpdateTodoDto dto)
        {
            if (dto == null)
                return BadRequest("Payload is required.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (id <= 0)
                return BadRequest("Invalid id.");

            long userId = CurrentUserId;

            var response = await _service.UpdateAsync(userId, id, dto);

            if (!response.Success || response.Data == null)
                return NotFound();

            return Ok(response.Data);
        }

        // ========================================
        // DELETE api/todoItem/{id}
        // ========================================
        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            long userId = CurrentUserId;

            var response = await _service.DeleteAsync(id, userId);

            if (!response.Success)
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // ========================================
        // DELETE api/todoItem  (delete all user todos)
        // ========================================
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> DeleteMyTodos()
        {
            long userId = CurrentUserId;

            var response = await _service.DeleteByUserIdAsync(userId);

            if (!response.Success)
                return BadRequest(response.Error);

            if (response.Data == 0)
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // ========================================
        // PATCH api/todoItem/{id}/completed
        // ========================================
        [HttpPatch]
        [Route("{id:long}/completed")]
        public async Task<IHttpActionResult> MarkCompleted(long id)
        {
            if (id <= 0)
                return BadRequest("Invalid id.");

            long userId = CurrentUserId;

            var response = await _service.MarkAsCompletedAsync(id, userId);

            if (!response.Success || response.Data == null)
                return NotFound();

            return Ok(response.Data);
        }
    }
}
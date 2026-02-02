using Shared.DTOs.TodoItem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TodoBack.DTOs;
using TodoBack.DTOs.Auth;
using TodoBack.Services.Interfaces;

namespace TodoBack.Controllers
{
    [RoutePrefix("api/todo")]
    public class TodoItemController : ApiController
    {
        private readonly ITodoItemService _service;
        public TodoItemController(ITodoItemService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // ---------------------------
        // Helper: get current userId
        // ---------------------------
        private int CurrentUserId
        {
            get
            {
                // Web API doesn't expose Session directly; use HttpContext.Current
                var session = HttpContext.Current?.Session;
                if (session == null || session["UserId"] == null)
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);

                return (int)session["UserId"];
            }
        }

        // GET api/todo  (gets todos for current user)
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetMyTodos()
        {
            var userId = CurrentUserId;
            var items = await _service.GetByUserIdAsync(userId);
            return Ok(items);
        }


        // GET api/todo/{id}  (get specific todo for current user)
        [HttpGet]
        [Route("{id:int}", Name ="GetTodoById")]
        public async Task<IHttpActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid todo id.");

            int userId = CurrentUserId;

            var item = await _service.GetByIdAsync(id, userId);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // POST api/todo
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Create([FromBody] TodoCreateDto todoItem)
        {
            if (todoItem == null) return BadRequest("Payload is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = CurrentUserId;

            var created = await _service.CreateAsync(userId, todoItem);
            if (created == null)
                return BadRequest("Unable to create todo item.");
            return CreatedAtRoute("GetTodoById", new { id = created.Id }, created);

        }

        // PUT api/todo/{id}
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UpdateTodoDto todoItem)
        {
            if (todoItem == null) return BadRequest("Payload is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id <= 0) return BadRequest("Invalid id.");

            var userId = CurrentUserId;

            var updated = await _service.UpdateAsync(userId, id, todoItem);
            if (updated == null) return NotFound(); // not found or not owned

            return Ok(updated);
        }


        // DELETE api/todo/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid id.");

            var userId = CurrentUserId;

            var success = await _service.DeleteAsync(id, userId);
            if (success == null) return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }


        // DELETE api/todo (delete all todos for current user)
        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> DeleteMyTodos()
        {
            var userId = CurrentUserId;

            // returns count deleted
            var deletedCount = await _service.DeleteByUserIdAsync(userId);
            if (deletedCount == 0) return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }


        [HttpPatch]
        [Route("{id:int}/completed")]
        public async Task<IHttpActionResult> MarkCompleted(int id)
        {
            if (id <= 0) return BadRequest("Invalid id.");

            int userId = CurrentUserId;

            var updated = await _service.MarkAsCompletedAsync(id, userId);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

    }
}

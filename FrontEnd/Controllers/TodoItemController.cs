using Shared.DTOs.TodoItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FrontEnd.Controllers
{
    public class TodoItemController : Controller
    {

        private TodoBackApiClient Api =>
               new TodoBackApiClient(ApiClientFactory.BaseUrl, ApiClientFactory.GetCookieJar());

        public async Task<ActionResult> Index()
        {
            try
            {
                var todos = await Api.GetMyTodosAsync();
                return View(todos);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<TodoDto>());
            }
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View(new TodoCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TodoCreateDto dto)
        {
            try
            {
                await Api.CreateTodoAsync(dto);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {

            if (id <= 0) return HttpNotFound();

            try
            {
                var todo = await Api.GetTodoByIdAsync(id);
                if (todo == null) return HttpNotFound();

                var dto = new UpdateTodoDto
                {
                    Title = todo.Title,
                    CategoryId = todo.CategoryId
                };

                ViewBag.TodoId = id;

                return View(dto);
            }

            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, UpdateTodoDto dto)
        {
            if (id <= 0) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.TodoId = id;
                return View(dto);
            }

            try
            {
                var result = await Api.UpdateTodoAsync(id, dto);
                if (result == null)
                    return HttpNotFound();

                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.TodoId = id;
                return View(dto);
            }
        }


        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0) return HttpNotFound();

            try
            {
                var todo = await Api.GetTodoByIdAsync(id);
                if (todo == null) return HttpNotFound();

                return View(todo); 
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0) return HttpNotFound();

            try
            {
                await Api.DeleteTodoAsync(id);
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0) return HttpNotFound();
            try
            {
                var todo = await Api.GetTodoByIdAsync(id);
                if (todo == null) return HttpNotFound();

                return View(todo);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
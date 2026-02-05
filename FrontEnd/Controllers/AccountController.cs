using Shared.DTOs.Auth;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

public class AccountController : Controller
{
    private TodoBackApiClient Api =>
        new TodoBackApiClient(ApiClientFactory.BaseUrl, ApiClientFactory.GetCookieJar());

    [HttpGet]
    public ActionResult Login() => View(new LoginDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var result = await Api.LoginAsync(dto);
        if (!result.ok)
        {
            ModelState.AddModelError("", result.error ?? "Login failed.");
            return View(dto);
        }

        // Optional: call /api/auth/me to store username
        var me = await Api.MeAsync();
        Session["ApiLoggedIn"] = me.loggedIn;
        Session["ApiUsername"] = me.username;

        return RedirectToAction("Index", "TodoItem");

    }


    [HttpGet]
    public ActionResult Register()
    {
        return View(new RegisterDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterDto dto){

        if (!ModelState.IsValid) return View(dto);

        var result = await Api.RegisterAsync(dto);
        if (!result.ok)
        {
            ModelState.AddModelError("", result.error ?? "Registration failed.");
            return View(dto);
        }
        return RedirectToAction("Login", "Account");

    }

    [HttpGet]
    public async Task<ActionResult> Me()
    {
        try
        {
            var me = await Api.MeAsync();
            ViewBag.Username = me.username;
            if (!me.loggedIn)
                return RedirectToAction("Login");
            return View(me);
        }
        catch
        {
            return RedirectToAction("Login");
        }
    }

    [HttpGet]
    public async Task<ActionResult> DeleteMyTodos()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteMyTodosConfirmed()
    {
        try
        {
            var deleted = await Api.DeleteMyTodosAsync();

            TempData["Flash.Type"] = deleted ? "success" : "info";
            TempData["Flash.Message"] = deleted
                ? "All your notes were deleted."
                : "You don’t have any notes to delete.";

            return RedirectToAction("Me");
        }

        catch (UnauthorizedAccessException)
        {
            TempData["Flash.Type"] = "warning";
            TempData["Flash.Message"] = "Session expired. Please log in again.";
            return RedirectToAction("Login");
        }
        catch
        {
            TempData["Flash.Type"] = "danger";
            TempData["Flash.Message"] = "Unexpected error while deleting your notes.";
            return RedirectToAction("Me");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Logout()
    {
        await Api.LogoutAsync();
        ApiClientFactory.ClearCookieJar();
        Session.Remove("ApiLoggedIn");
        Session.Remove("ApiUsername");

        return RedirectToAction("Login");
    }
}
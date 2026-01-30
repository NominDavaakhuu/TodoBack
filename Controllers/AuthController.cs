using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TodoBack.DTOs.Auth;
using TodoBack.Services.Interfaces;

namespace TodoBack.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auth.RegisterAsync(dto);

            if (!result.Success)
                return Content(HttpStatusCode.BadRequest, result.Error);

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _auth.LoginAsync(dto);
            if(!result.Success)
                return Content(HttpStatusCode.Unauthorized, result.Error);

            HttpContext.Current.Session["UserId"] = result.UserId.Value;
            HttpContext.Current.Session["Username"] = result.Username;

            return Ok(result);
        }


        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            var session = HttpContext.Current?.Session;
            session?.Clear();
            session?.Abandon();
            return StatusCode(HttpStatusCode.NoContent);
        }


        [HttpGet]
        [Route("me")]
        public IHttpActionResult Me()
        {
            var session = HttpContext.Current?.Session;
            if (session == null || session["UserId"] == null)
                return StatusCode(HttpStatusCode.Unauthorized);

            return Ok(new
            {
                UserId = (int)session["UserId"],
                Username = (string)session["Username"]
            });
        }


    }
}

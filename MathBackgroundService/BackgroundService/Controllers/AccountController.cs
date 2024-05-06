using BackgroundServiceVote.Data;
using BackgroundServiceVote.DTOs;
using BackgroundServiceVote.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundServiceVote.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly UserManager<IdentityUser> UserManager;
        readonly SignInManager<IdentityUser> SignInManager;
        readonly BackgroundServiceContext _context;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, BackgroundServiceContext context)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterDTO register)
        {
            if (register.Password != register.PasswordConfirm)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new { Message = "Les deux mots de passe spécifiés sont différents." });
            }
            IdentityUser user = new IdentityUser()
            {
                UserName = register.Username,
                Email = register.Email
            };
            IdentityResult identityResult = await this.UserManager.CreateAsync(user, register.Password);
            if (!identityResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "La création de l'utilisateur a échoué." });
            }

            var player = new Player()
            {
                Id = 0,
                NbRightAnswers = 0,
                User = user
            };

            _context.Player.Add(player);
            _context.SaveChanges();

            return Ok(new { Message = "Inscription réussie ! 🥳" });
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDTO login)
        {
            var result = await SignInManager.PasswordSignInAsync(login.Username, login.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok();
            }

            return NotFound(new { Error = "L'utilisateur est introuvable ou le mot de passe de concorde pas" });
        }

        public async Task<ActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Ok();
        }
    }
}

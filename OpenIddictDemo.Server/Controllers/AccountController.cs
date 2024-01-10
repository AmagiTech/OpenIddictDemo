using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddictDemo.Models;

namespace OpenIddictDemo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AccountController(SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        // I didn't write Register Method Write Your Own
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> Register()
        //{
        //    var email = "info@amagitech.com";
        //    var username = "0A2B07D3-7396-49B4-B824-449DF9D3C2EE";
        //    var pass = "5wsc31G#!d2Zuz*yB6Asfg!8UUzH%nMd";
        //    var name = "Amagi";
        //    var surname = "Tech";

        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByNameAsync(email);
        //        if (user != null)
        //        {
        //            return StatusCode(StatusCodes.Status409Conflict);
        //        }

        //        user = new User
        //        {
        //            UserName = username,
        //            Email = email,
        //            Id = Guid.NewGuid(),
        //            Name = name,
        //            Surname = surname,
        //            IsActive = true,
        //            IsDeleted = false,
        //        };
        //        var result = await _userManager.CreateAsync(user, pass);
        //        if (result.Succeeded)
        //        {
        //            return Ok();
        //        }
        //        return BadRequest();
        //    }

        //    // If we got this far, something failed.
        //    return BadRequest(ModelState);
        //}

    }
}

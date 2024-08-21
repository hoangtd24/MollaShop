using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Molla.Models;
using Molla.Models.Account;
using Molla.Services;

namespace Molla.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly ISendMailService _sendMailService;


    public AccountController(ILogger<AccountController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ISendMailService sendMailService)
    {
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _sendMailService = sendMailService;
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.ActionLink(
                    nameof(ConfirmEmail),
                    values: new { userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                await _sendMailService.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToAction("RegisterConfirmation", "Account");
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }

    [HttpGet("/login")]
    public IActionResult Login(string? returnUrl)
    {
        ViewData["returnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                // Handle successful login
                _logger.LogInformation("signInManager");
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            if (result.RequiresTwoFactor)
            {
                // Handle two-factor authentication case
            }
            if (result.IsLockedOut)
            {
                // Handle lockout scenario
            }
            else
            {
                // Handle failure
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
        // If we got this far, something failed, redisplay form
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    [AllowAnonymous]
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> IsEmailAvailable(string Email)
    {
        //Check If the Email Id is Already in the Database
        var user = await _userManager.FindByEmailAsync(Email);
        if (user == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Email {Email} is already in use.");
        }
    }

    [HttpGet("/confirm-email")]
    [AllowAnonymous]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return View("ErrorConfirmEmail");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View("ErrorConfirmEmail");
        }
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return View(result.Succeeded ? "ConfirmEmail" : "ErrorConfirmEmail");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

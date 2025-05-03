using CaptchaVerification.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;

namespace CaptchaVerification.Controllers
{
    public class HomeController(CaptchaService captchaService, UserManager<CaptchaVerificationUser> userManager, IEmailSender emailSender) : Controller
    {
        private readonly UserManager<CaptchaVerificationUser> _userManager = userManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly CaptchaService _captchaService = captchaService;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (!Request.Form.ContainsKey("g-recaptcha-response")) return View("index");

                var token = Request.Form["g-recaptcha-response"].ToString();
                if (!await _captchaService.IsValid(token)) return View("index");

                //Email not available so will skip all the logic

                //var user = await _userManager.FindByEmailAsync(model.Email);
                //if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return RedirectToPage("/Account/ForgotPasswordConfirmation",new { area = "Identity" });
                //}

                var user = new CaptchaVerificationUser
                {
                    Id= Guid.NewGuid().ToString(),
                    Email = model.Email,
                    UserName = model.Email
                };
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                //Not using a callbackUrl for now

                //var callbackUrl = Url.Page(
                //    "Account/ResetPassword",
                //    pageHandler: null,
                //    values: new { area = "Identity", code },
                //    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(code)}'>clicking here</a>.");

                return RedirectToPage("/Account/ForgotPasswordConfirmation", new { area = "Identity" });
            }
            return View("index");
        }
    }
}

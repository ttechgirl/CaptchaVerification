using CaptchaVerification.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;

namespace CaptchaVerification
{
    public class ForgotPasswordModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}

using eTickets.Data;
using eTickets.Data.Static;
using eTickets.Data.ViewModels;
using eTickets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace eTickets.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            this._userManager = userManager;  
            this._signInManager = signInManager;
            this._context = context;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        #region Login
        public IActionResult Login() => View(new LoginVM());

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM); //if user exists in the database

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Movies");
                    }
                }
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View(loginVM);
            }

            TempData["Error"] = "Wrong credentials. Please, try again!";
            return View(loginVM);
        }
        #endregion

        #region Register
        public IActionResult Register() => View(new RegisterVM());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (registerVM.EmailAddress != null)
            {
                var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
                if (user != null)
                {
                    TempData["Error"] = "This email address is already in use";
                    return View(registerVM);
                }
            }

            if (ModelState["FullName"].ValidationState != ModelValidationState.Valid ||
              ModelState["EmailAddress"].ValidationState != ModelValidationState.Valid ||
              ModelState["Password"].ValidationState != ModelValidationState.Valid ||
              ModelState["ConfirmPassword"].ValidationState != ModelValidationState.Valid)
            {
                return View(registerVM);
            }

            var newUser = new ApplicationUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (newUserResponse.Succeeded)
            {
                SendEmailToRegisteredUser(registerVM.EmailAddress, registerVM.FullName);
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                
            }

            LoginVM loginVM = new LoginVM();
            loginVM.EmailAddress = registerVM.EmailAddress;
            loginVM.Password = registerVM.Password;

            return await Login(loginVM);
        }
        #endregion

        private void SendEmailToRegisteredUser(string receiverEmailAddress, string userName)
        {
            string fromMail = "elvindunyamektebi@gmail.com";
            string fromPassword = "vlydykfeavdeyqee";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Registration Successful";
            message.To.Add(new MailAddress(receiverEmailAddress));

            #region Custom HTML template
            message.Body = @"<!DOCTYPE html>
                             <html>
                             <head>
                                 <style>
                                     body {
                                         font-family: Arial, sans-serif;
                                         background-color: #f2f2f2;
                                         margin: 0;
                                         padding: 0;
                                     }
                             
                                     table {
                                         border-collapse: collapse;
                                         width: 100%;
                                     }
                             
                                     td {
                                         padding: 20px;
                                     }
                             
                                     h1 {
                                         color: #333;
                                     }
                             
                                     p {
                                         color: #666;
                                         line-height: 1.6;
                                     }
                             
                                     a {
                                         text-decoration: none;
                                         color: #007bff;
                                     }
                             
                                     .social-icon {
                                         width: 30px;
                                         height: 30px;
                                         vertical-align: middle;
                                         margin-right: 10px;
                                     }
                             
                                     .social-link {
                                         display: inline-block;
                                         margin-right: 20px;
                                     }
                                 </style>
                             </head>
                             <body>
                                 <table width=""100%"" bgcolor=""#f2f2f2"">
                                     <tr>
                                         <td align=""center"">
                                             <table width=""600"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border: 1px solid #e0e0e0;"">
                                                 <tr>
                                                     <td>
                                                         <table width=""100%"">
                                                             <tr>
                                                                 <td align=""center"" style=""padding: 20px;"">
                                                                     <h1>Welcome to CINEMATIX</h1>
                                                                     <p>Dear <b>" + userName + @"</b>, </p>
                                                                     <p>Thank you for registering with us. We are excited to have you as a member of our community.</p>
                                                                     <p>Your account has been successfully created. Feel free to explore our website and start enjoying all the benefits of being a member.</p>
                                                                     <p>If you have any questions or need assistance, please don't hesitate to contact our support team at <a href=""mailto:rnet102sql@gmail.com"">[Support Email]</a>.</p>
                                                                     <p>Happy browsing!</p>
                                                                 </td>
                                                             </tr>
                                                         </table>
                                                     </td>
                                                 </tr>
                                                 <tr>
                                                     <td>
                                                         <table width=""100%"" bgcolor=""#f2f2f2"">
                                                            <tr>
                                                                <td align=""center"" style=""padding: 20px;"">
                                                                    <p>Stay connected with us:</p>
                                                                    <div class=""social-link"">
                                                                        <a href=""https://www.facebook.com/code.edu.az"" style=""text-decoration: none;"">
                                                                            <img src=""https://cdn-icons-png.flaticon.com/512/124/124010.png"" alt=""Facebook"" class=""social-icon"" style=""border-radius: 5px; margin-right: 0px;""> Facebook
                                                                        </a>
                                                                    </div>
                                                                    <div class=""social-link"">
                                                                        <a href=""https://x.com/elonmusk?s=20"" style=""text-decoration: none;"">
                                                                            <img src=""https://cdn-icons-png.flaticon.com/512/124/124021.png"" alt=""Twitter"" class=""social-icon"" style=""border-radius: 5px; margin-right: 0px;""> Twitter
                                                                        </a>
                                                                    </div>
                                                                    <div class=""social-link"">
                                                                        <a href=""https://www.instagram.com/code.edu.az/"" style=""text-decoration: none;"">
                                                                            <img src=""https://upload.wikimedia.org/wikipedia/commons/thumb/5/58/Instagram-Icon.png/1024px-Instagram-Icon.png"" alt=""Instagram"" class=""social-icon"" style=""border-radius: 5px; margin-right: 0px;""> Instagram
                                                                        </a>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                         </table>
                                                     </td>
                                                 </tr>
                                             </table>
                                         </td>
                                     </tr>
                                 </table>
                             </body>
                             </html>"; 
            #endregion

            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);
        }

        #region Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Movies");
        } 
        #endregion
    }
}

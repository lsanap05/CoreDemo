using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core_WebApp.CustomFilters;
using CoreDemo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CoreDemo.Controllers
{
    [Authorize(Roles = "Administration")]
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private IHostingEnvironment _hostingEnvironment;
        // private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger,
            // IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            // _emailSender = emailSender;
            _roleManager = roleManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public User Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
               // ReturnUrl = returnUrl;
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                // ViewBag.Name = new SelectList(await catRepo.GetAsync(), "CategoryRowId", "CategoeyName");
                ViewData["Name"] = _roleManager.Roles.ToList();

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User _user,string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {

                    var role = await _roleManager.FindByNameAsync(Input.Name);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(user, Input.Name);
                    }

                    //_logger.LogInformation("User created a new account with password.");

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                    //}
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            // return View();
            return RedirectToAction("Index");
        }
        public IActionResult Index()
        {
            var user =  _userManager.Users.ToList();
            return View(user);
        }

        //[HttpPost]
         public IActionResult Upload()
        {
            return RedirectToAction("Index");

        }
        //[HttpPost]
        public async Task<IActionResult> Download()
        {
            try
            {
                #region for csv
                //StringBuilder stringBuilder = new StringBuilder();
                //stringBuilder.AppendLine("Id,FirstName,Email,PhoneNumber");
                //var user = _userManager.Users.ToList();
                //foreach (var _user in user)
                //{
                //    stringBuilder.AppendLine($"{_user.Id},{ _user.UserName},{ _user.Email},{ _user.PhoneNumber}");
                //}
                //return File(Encoding.UTF8.GetBytes
                //(stringBuilder.ToString()), "text/csv", "user.csv"); 
                #endregion

                #region for xlsx
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                string sFileName = @"demo.xlsx";
                string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, sFileName);
                FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
                var memory = new MemoryStream();
                using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook;
                    workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Sheet1");
                    var user = _userManager.Users.ToList();

                    IRow row = excelSheet.CreateRow(0);

                    row.CreateCell(0).SetCellValue("ID");
                    row.CreateCell(1).SetCellValue("Name");
                    row.CreateCell(2).SetCellValue("Age");

                    row = excelSheet.CreateRow(1);
                    row.CreateCell(0).SetCellValue(1);
                    row.CreateCell(1).SetCellValue("Kane Williamson");
                    row.CreateCell(2).SetCellValue(29);

                    row = excelSheet.CreateRow(2);
                    row.CreateCell(0).SetCellValue(2);
                    row.CreateCell(1).SetCellValue("Martin Guptil");
                    row.CreateCell(2).SetCellValue(33);

                    row = excelSheet.CreateRow(3);
                    row.CreateCell(0).SetCellValue(3);
                    row.CreateCell(1).SetCellValue("Colin Munro");
                    row.CreateCell(2).SetCellValue(23);

                    workbook.Write(fs);
                }
                using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
                #endregion
        
            catch
            {
                
            }
            return RedirectToAction("Index");
        }
       // [HttpPost]
        public IActionResult Print()
        {
            return RedirectToAction("Index");
        }
    }
}

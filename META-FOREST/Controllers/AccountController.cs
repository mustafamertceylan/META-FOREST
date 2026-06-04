using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MetaForest.Models.ViewModels;
using System.Threading.Tasks;

namespace MetaForest.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (ModelState.IsValid) // 4. Madde: Validations kontrolü
                {
                    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        // Ödev kuralı gereği Roller (Admin/User) otomatik kontrol ediliyor
                        string roleName = model.IsAdmin ? "Admin" : "User";

                        // Veritabanında bu rol yoksa önce oluştur
                        if (!await _roleManager.RoleExistsAsync(roleName))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(roleName));
                        }

                        // Kullanıcıya rolünü ata
                        await _userManager.AddToRoleAsync(user, roleName);

                        // Giriş yap ve ana sayfaya yönlendir
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kayıt işlemi sırasında bir hata oluştu.");
                ModelState.AddModelError(string.Empty, "Kayıt sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Giriş işlemi sırasında bir hata oluştu.");
                ModelState.AddModelError(string.Empty, "Giriş sırasında bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied() => View();
    }
}
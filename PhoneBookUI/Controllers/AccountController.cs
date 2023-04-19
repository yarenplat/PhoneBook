using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PhoneBookBusinessLayer.EmailSenderBusiness;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookEntityLayer.ViewModels;
using PhoneBookUI.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhoneBookUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMemberManager _memberManager;
        private readonly IEmailSender _emailSender;

        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public AccountController(IMemberManager memberManager, IEmailSender emailSender)
        {
            _memberManager = memberManager;
            _emailSender = emailSender;
        }

        public IActionResult Register()
        {
            //Bu metot sayfayı sadece getirir. HTTPGET
            return View(); // bu metot geriye bir sayfa gönderecek
        }

        [HttpPost] // sayfadaki submit türündeki butona tıkladığında yazdığı bilgilerle bu metoda düşecektir
        public IActionResult Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) // gelen bilgiler class içindeki annotationslara uygun değilse 
                {
                    ModelState.AddModelError("", "Gerekli alanları lütfen doldurunuz!");
                    return View(model);
                }

                // Ekleme işlemleri yapılacak

                //1) Aynı emailden tekrar kayıt olamaz!
                var isSameEmail = _memberManager.GetByConditions
                    (x => x.Email.ToLower() == model.Email.ToLower()).Data;

                if (isSameEmail != null)
                {
                    ModelState.AddModelError("", "Dikkat bu kullanıcı sistemde zaten mevcuttur!");
                    return View(model);
                }
                MemberViewModel member = new MemberViewModel()
                {
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    Gender = model.Gender,
                    BirthDate = model.BirthDate,
                    CreatedDate = DateTime.Now,
                    IsRemoved = false
                };

                member.PasswordHash = HashPasword(model.Password, out byte[] salt);
                member.Salt = salt;

                var result = _memberManager.Add(member);
                if (result.IsSuccess)
                {
                    // Hoşgeldiniz emaili gönderilecek
                    var email = new EmailMessage()
                    {
                        To = new string[] { member.Email },
                        Subject = $"503 Telefon Rehberi - HOŞGELDİNİZ!",
                        //body içinde html yazılıyor
                        Body = $"<html lang='tr'><head></head><body>" +
                        $"Merhaba Sayın {member.Name} {member.Surname},<br/>" +
                        $"Sisteme kaydınız gerçekleşmiştir. Başımız ağrıdı aktivasyona gerek yok. Direk sisteme giriş yapıp kullanabilirsiniz." +
                        $"</body></hmtl>"
                    };

                    //sonra async'ye çevirelim
                    _emailSender.SendEmail(email);

                    // login sayfasına yönlendirilecek
                    TempData["RegisterSuccessMessage"] = $"{member.Name} {member.Surname} kaydınız gerçekleşti. Giriş yapabilirsiniz.";

                    return RedirectToAction("Login", "Account", new { email = model.Email });
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir sorun oluştu! " + ex.Message);
                //ex loglanmalı biz şimdi geçici olarak yazdık
                return View(model); // burada return View(model) parametre olarak model vermemizin sebebi sayfadaki bilgiler silinmesin.
            }
        }


        [HttpGet]
        public IActionResult Login(string? email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                LoginViewModel model = new LoginViewModel()
                {
                    Email = email
                };
                return View(model);
            }

            return View(new LoginViewModel());
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.LoginErrorMsg = $"Gerekli alanları doldurunuz!";
                    return View(model);
                }

                var user = _memberManager.GetById(model.Email).Data;
                if (user==null)
                {
                    ViewBag.LoginErrorMsg = $"Kullanıcı adınız ya da şifrenizi doğru yazdığınızdan emin olunuz!";
                    return View(model);
                }
                var passwordCompare = VerifyPassword(model.Password, user.PasswordHash, user.Salt);
                if (!passwordCompare)
                {
                    ViewBag.LoginErrorMsg = $"Kullanıcı adınız ya da şifrenizi doğru yazdığınızdan emin olunuz!";
                    return View(model);
                }
                // Giriş yapılacak
                //Bu kişini bilgileri (email) oturum(session) cokkie olarak kayıt edeceğiz
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new(ClaimTypes.Name, user.Email));
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                //Ardından Home Index sayfasına yönlendireceğiz
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                // ex loglanacak şuan development aşaması old için mesajını yazdırdık
                ViewBag.LoginErrorMsg = $"Beklenmedik bir hata oluştu! {ex.Message}";
                return View(model);
            }
        }


       [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        private string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }
        private bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);

            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }




    }
}
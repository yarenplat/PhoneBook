using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using NuGet.Common;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookEntityLayer.Entities;
using PhoneBookEntityLayer.ViewModels;
using PhoneBookUI.Models;
using System.Diagnostics;

namespace PhoneBookUI.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhoneTypeManager _phoneTypeManager;
        private readonly IMemberPhoneManager _memberPhoneManager;

        public HomeController(ILogger<HomeController> logger, IPhoneTypeManager phoneTypeManager, IMemberPhoneManager memberPhoneManager)
        {
            _logger = logger;
            _phoneTypeManager = phoneTypeManager;
            _memberPhoneManager = memberPhoneManager;
        }

        public IActionResult Index()
        {
            //Eğer giriş yapmış ise giriş yapan kullanıcının rehberini model olarak sayfaya gönderelim
            if (HttpContext.User.Identity?.Name != null)
            {
                var userEmail = HttpContext.User.Identity?.Name;
                var data = _memberPhoneManager.GetAll(x =>
                x.MemberId == userEmail).Data;
                return View(data);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        [Authorize] // authorize login olmadan sayfaya erişimi önler
        public IActionResult AddPhone()
        {
            try
            {
                var phoneTypes = _phoneTypeManager.GetAll().Data;
                ViewBag.PhoneTypes = phoneTypes; // not IsRemoved viewmodelin içine eklensin
                ViewBag.FirstPhoneTypeId = -1;
                if (phoneTypes.Count > 0)
                {
                    ViewBag.FirstPhoneTypeId = phoneTypes.FirstOrDefault()?.Id;
                }

                MemberPhoneViewModel model = new MemberPhoneViewModel()
                {
                    MemberId = HttpContext.User.Identity?.Name
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!" + ex.Message);
                ViewBag.PhoneTypes = new List<PhoneTypeViewModel>();
                return View();

            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddPhone(MemberPhoneViewModel model)
        {
            try
            {
                ViewBag.PhoneTypes = _phoneTypeManager.GetAll().Data; // not IsRemoved viewmodelin içine eklensin
                if (!ModelState.IsValid)
                {
                    //Gerekli alanaları doldurunuzu bu sefer yazmadıkkk 
                    return View(model);
                }

                model.Phone = model.CountryCode + model.Phone;

                //1) Aynı telefondan var mı?
                var samePhone = _memberPhoneManager.GetByConditions(x =>
                x.MemberId == model.MemberId && x.Phone == model.Phone).Data;
                if (samePhone != null)
                {
                    ModelState.AddModelError("", $"Bu telefon {samePhone.PhoneType.Name} türünde zaten eklenmiştir!");
                    return View(model);
                }

                if (model.AnotherPhoneTypeName != null)
                {//Eğer AnotherPhoneTypeName null değil ise Diğer seçeneğini seçmiştir.

                    //Diğer'i seçip veritabanında zaten mevcut olan türü yazarsa?
                    var samePhoneType = _phoneTypeManager.GetByConditions(x => x.Name.ToLower() == model.AnotherPhoneTypeName.ToLower()).Data;

                    if (samePhoneType != null)
                    {
                        ModelState.AddModelError("", $"{samePhoneType.Name} zaten mevcuttur! Türlerden seçerek rehbere eklemeyi tekrar deneyiniz!");
                        return View(model);
                    }


                    // Diğer ile yazdığı türü ekledik ve id'sini aldık
                    PhoneTypeViewModel phoneType = new PhoneTypeViewModel()
                    {
                        CreatedDate = DateTime.Now,
                        Name = model.AnotherPhoneTypeName
                    };
                    var result = _phoneTypeManager.Add(phoneType).Data;
                    model.PhoneTypeId = result.Id;
                } // if bitti

                //2) Telefonu ekle
                //Diğer seçeneğinin senaryosunu yarın yazacağız.
                model.CreatedDate = DateTime.Now;
                model.IsRemoved = false;


                if (!_memberPhoneManager.Add(model).IsSuccess)
                {
                    ModelState.AddModelError("", "EKLEME BAŞARISIZ! TEKRAR DENEYİNİZ!");
                    return View(model);
                }
                TempData["AddPhoneSuccessMsg"] = "Yeni numara rehbere eklendi";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!" + ex.Message);
                ViewBag.PhoneTypes = new List<PhoneTypeViewModel>();
                return View();
            }
        }



        [Authorize]
        public IActionResult DeletePhone(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["DeleteFailedMsg"] = $"id değeri düzgün değil!";
                    return RedirectToAction("Index", "Home");
                }
                var phone = _memberPhoneManager.GetById(id).Data;
                if (phone == null)
                {
                    TempData["DeleteFailedMsg"] = $"Kayıt bulunamadığı için silme başarısızdır!";
                    return RedirectToAction("Index", "Home");
                }
                if (!_memberPhoneManager.Delete(phone).IsSuccess)
                {
                    TempData["DeleteFailedMsg"] = $"Silme başarısızdır!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["DeleteSuccessMsg"] = $"Telefon rehberden silindi";
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                TempData["DeleteFailedMsg"] = $"Beklenmedik bir hata oldu! {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost] // FromQueryli halini yazalım
        public JsonResult PhoneDelete([FromBody] int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isSuccess = false, message = $"id değeri düzgün değil" });
                }
                var phone = _memberPhoneManager.GetById(id).Data;
                if (phone == null)
                {
                    return Json(new { isSuccess = false, message = $"Kayıt bulunamadığı için silme başarısızdır!" });
                }
                //hard delete
                if (!_memberPhoneManager.Delete(phone).IsSuccess)
                {
                    return Json(new { isSuccess = false, message = $"Silme başarısızdır!" });
                }
                var userEmail = HttpContext.User.Identity?.Name;
                var data = _memberPhoneManager.GetAll(x =>
                x.MemberId == userEmail).Data;

                return Json(new { isSuccess = true, message = $"Telefon rehberden silindi!", phones = data });

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = $"Beklenmedik bir hata oluştu! {ex.Message}" });
            }

        }


        [HttpGet]
        [Authorize]
        public IActionResult EditPhone(int id)
        {
            try
            {
                ViewBag.PhoneTypes = _phoneTypeManager.GetAll().Data;
                if (id <= 0)
                {
                    ModelState.AddModelError("", "id değeri sıfırdan küçük olamaz!");
                    return View();
                }

                var phone = _memberPhoneManager.GetById(id).Data;
                if (phone == null)
                {
                    ModelState.AddModelError("", "Kayıt bulunamadı!");
                    return View();
                }

                var country = phone.Phone.Substring(0, 3);
                phone.CountryCode = country;
                phone.Phone = phone.Phone.Substring(3);

                return View(phone);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik hata" + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditPhone(MemberPhoneViewModel model)
        {
            try
            {
                ViewBag.PhoneTypes = _phoneTypeManager.GetAll().Data;

                var phone = _memberPhoneManager.GetById(model.Id).Data;
                if (phone == null)
                {
                    ModelState.AddModelError("", "Kayıt bulunmadı!");
                    return View(model);
                }

                //Var olan bir telefonu mu yazmış?
                var samePhone = _memberPhoneManager.GetByConditions(
                    x => x.Id!=model.Id &&
                    x.MemberId == HttpContext.User.Identity.Name
                    && x.Phone == (model.CountryCode+ model.Phone)).Data;

                if (samePhone != null)
                {
                    ModelState.AddModelError("", $"{model.Phone} şeklindeki telefon {samePhone.FriendNameSurname} adlı kişiye aittir! Lütfen numarayı kontrol ediniz!");
                    return View(model);
                }

                phone.Phone = model.CountryCode+ model.Phone;
                phone.FriendNameSurname = model.FriendNameSurname;
                phone.PhoneTypeId = model.PhoneTypeId;

                if (_memberPhoneManager.Update(phone).IsSuccess)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Güncelleme başarısız oldu! Tekrar deneyiniz!");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik hata" + ex.Message);
                return View();
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookEntityLayer.ViewModels;

namespace PhoneBookUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Route("a/[Controller]/[Action]/{id?}")] bu route verildiğinde [action] yazan yere action'ın tam adını yazmadan sayfa açılmaz
    [Route("admin")] // bu route verildiğinde controller'a nasıl ulaşıldığı belirtilir ve action'a ulaşılma konusundaki  kuralı action üzerinde yazılan kural belirler.
    public class HomeController : Controller
    {
        private readonly IMemberManager _memberManager;
        private readonly IPhoneTypeManager _phoneTypeManager;
        private readonly IMemberPhoneManager _memberPhoneManager;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IMemberManager memberManager, IPhoneTypeManager phoneTypeManager, IMemberPhoneManager memberPhoneManager, IWebHostEnvironment environment)
        {
            _memberManager = memberManager;
            _phoneTypeManager = phoneTypeManager;
            _memberPhoneManager = memberPhoneManager;
            _environment = environment;
        }

        [Route("dsh")] //Action'un ismi çok uzun olabilir url'e action'ın isminin hepsini yazmak istemezsek action'a Route verebiliriz.
        public IActionResult Dashboard()
        {
            //Bu ay sisteme kayıt olan üye sayısı.
            DateTime thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.MontlyMemberCount = _memberManager.GetAll(x => x.CreatedDate > thisMonth.AddDays(-1)).Data.Count;
            //Bu ay sisteme eklenen numara sayısı.
            ViewBag.MontlyContactCount = _memberPhoneManager.GetAll(x => x.CreatedDate > thisMonth.AddDays(-1)).Data.Count;

            //En son eklenen üyenin adı soyadı
            var lastMember = _memberManager.GetAll().Data.OrderBy(x => x.CreatedDate).LastOrDefault();
            ViewBag.LastMember = $"{lastMember?.Name} {lastMember?.Surname}";

            //Rehbere son eklenen kişi
            var lastContact = _memberPhoneManager.GetAll().Data.LastOrDefault();
            ViewBag.LastContact = lastContact?.FriendNameSurname;


            return View();
        }

        [Route("/admin/GetPhoneTypePieData")]
        public JsonResult GetPhoneTypePieData()
        {
            try
            {
                Dictionary<string, int> model = new Dictionary<string, int>();
                var data = _memberPhoneManager.GetAll().Data;
                foreach (var item in data)
                {
                    var count = 1;
                    if (!model.ContainsKey(item.PhoneType.Name))
                    {
                        model.Add(item.PhoneType.Name, count);
                    }
                    else
                    {
                        model[item.PhoneType.Name]++;
                    }
                }
                return Json(new { isSuccess = true, message = "Veriler geldi", types = model.Keys.ToArray(), points = model.Values.ToArray() });

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Veriler getirilemedi!" });
            }
        }

        [HttpGet]
        [Route("uye")]
        public IActionResult MemberIndex()
        {
            try
            {
                var data = _memberManager.GetAll().Data;

                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu! " + ex.Message);
                return View();
            }
        }

        [HttpGet]
        [Route("duzenle")]
        public IActionResult MemberEdit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ModelState.AddModelError("", "Id gelmediği için kullanıcı bulunamadı");
                    return View(new MemberViewModel());
                }
                var member = _memberManager.GetById(id).Data;
                if (member == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı");
                    return View(new MemberViewModel());
                }
                return View(member);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu" + ex.Message);
                return View(new MemberViewModel());
            }
        }

        [HttpPost]
        [Route("duzenle")]
        public IActionResult MemberEdit(MemberViewModel model)
        {
            try
            {
                var member = _memberManager.GetById(model.Email).Data;
                if (member == null)
                {
                    ModelState.AddModelError("", "Kullanıcı bulunamadı!");
                    return View(model);
                }
                member.Name = model.Name;
                member.Surname = model.Surname;
                member.BirthDate = model.BirthDate;
                member.Gender = model.Gender;

                //1) Upload pic null değilse RESİM yüklemesi yapılmalı.
                //2) Upload pic yüklenen resim mi?
                //3) Upload pis dosya boyutu >0 mı?
                if (model.UploadPicture != null &&
                    model.UploadPicture.ContentType.Contains("image") &&
                    model.UploadPicture.Length > 0)
                {
                    //wwwroot klasörü içerisine  MemberPictures isimli bir klasör oluşturulup o klasörün çine resmi kaydetmeliyim.
                    //Resmi kayedederken isimlendirmesini burada yeniden yapmalıyız.
                    string uploadPath = Path.Combine(_environment.WebRootPath, "MemberPictures");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    string memberPicturesName = model.Email.Replace("@", "-").Replace(".", "-");

                    string extentionName = Path.GetExtension(model.UploadPicture.FileName);

                    string filePath = Path.Combine(uploadPath, $"{memberPicturesName}{extentionName}");

                    using (Stream filestream = new FileStream(filePath, FileMode.Create))
                    {
                        model.UploadPicture.CopyTo(filestream);
                    }

                    member.Picture = $"/MemberPictures/{memberPicturesName}{extentionName}";

                }
                if (_memberManager.Update(member).IsSuccess)
                {
                    TempData["MemberEditSuccessMsg"] = $"{model.Name} {model.Surname} isimli üyenin bilgileri güncellenmiştir.";
                    return RedirectToAction("MemberIndex");
                }
                else
                {
                    ModelState.AddModelError("", "Güncelleme başarısız");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu" + ex.Message);
                return View(model);
            }
        }
    }
}

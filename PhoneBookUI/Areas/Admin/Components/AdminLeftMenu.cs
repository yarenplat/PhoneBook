using Microsoft.AspNetCore.Mvc;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookUI.Areas.Admin.Models;

namespace PhoneBookUI.Areas.Admin.Components
{
    public class AdminLeftMenu : ViewComponent
    {
        private readonly IMemberManager _memberManager;
        private readonly IPhoneTypeManager _phoneTypeManager;
        private readonly IMemberPhoneManager _memberPhoneManager;

        public AdminLeftMenu(IMemberManager memberManager, IPhoneTypeManager phoneTypeManager, IMemberPhoneManager memberPhoneManager)
        {
            _memberManager = memberManager;
            _phoneTypeManager = phoneTypeManager;
            _memberPhoneManager = memberPhoneManager;
        }
        // Eğer email gönderimi yapılacaksa buraya IEmailSender eklenmelidir


        public IViewComponentResult Invoke()
        {
            try
            {
                AdminLeftMenuDataCountModel model =
                    new AdminLeftMenuDataCountModel()
                {
                    //toplam üye sayısı TempData["TotalMemberCount"]
                    TotalMemberCount=_memberManager.GetAll().Data.Count,
                    //toplam telefon tipi sayısı 
                    TotalPhoneTypeCount=_phoneTypeManager.GetAll().Data.Count,
                    //toplam numara sayısı
                    TotalContactNumberCount=_memberPhoneManager.GetAll().Data.Count
                };

                return View(model);
            }
            catch (Exception ex)
            {
                //TempData ile burada oluşan hata gönderilebilir
                return View(new AdminLeftMenuDataCountModel());
            }
        }
    }
}

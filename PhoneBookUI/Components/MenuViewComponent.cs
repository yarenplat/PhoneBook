using Microsoft.AspNetCore.Mvc;
using PhoneBookBusinessLayer.InterfacesOfManagers;

namespace PhoneBookUI.Components
{
    public class MenuViewComponent:ViewComponent
    {
        //Asp Net Core'da ViewComponentler sayfa parçalarını yönettiğimiz yapıdır
        //Bu nedenle controllerların içinde yaptığımız DI'ları burada da yapabiliriz.
        private readonly IMemberManager _memberManager;

        public MenuViewComponent(IMemberManager memberManager)
        {
            _memberManager = memberManager;
        }


        public  IViewComponentResult Invoke()
        {
            string? userEmail = HttpContext.User.Identity?.Name; // Emaile bakacak
            TempData["LoggedInUserNameSurname"] = null;
            if (userEmail!=null)
            {
                var user = _memberManager.GetById(userEmail).Data;
                TempData["LoggedInUserNameSurname"] = $"{user.Name} {user.Surname}";
            }
            return View();
        }
    }

    
}

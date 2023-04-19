using System.ComponentModel.DataAnnotations;

namespace PhoneBookUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email gereklidir!")]
        // Regular expression eklenebilir
        public string Email { get; set; }
        [Required(ErrorMessage = "Parola gereklidir!")]
        // Regular expression eklenebilir
        public string Password { get; set; }

    }
}

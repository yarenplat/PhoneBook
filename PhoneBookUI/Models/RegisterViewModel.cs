
using System.ComponentModel.DataAnnotations;

namespace PhoneBookUI.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="İsim alanı gereklidir!")]
        [StringLength(50, MinimumLength =2, ErrorMessage ="İsim mak 50 min 2 karakter olmalıdır!")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyisim alanı gereklidir!")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Soyisim mak 50 min 5 karakter olmalıdır!")]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Email alanı gereklidir!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Parola alanı gereklidir!")]
        //NOT: Regular Expression ifadesi eklenecek
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor!")]
        public string ComparePassword { get; set; }
        public DateTime? BirthDate { get; set; }
        public byte? Gender { get; set; }

    }
}

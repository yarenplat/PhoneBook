using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookEntityLayer.ViewModels
{
    public class MemberViewModel //MeberDTO
    {
        [Required(ErrorMessage = "Email adresi boş geçilemez!")]
        [StringLength(100, ErrorMessage = "Email maks. 100 karakter olmalıdır!")]
        public string Email { get; set; }

        [Required] // dbdeki not null kutucuğu gelsin aklınıza
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Surname { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public byte[] Salt { get; set; }

        public DateTime? BirthDate { get; set; }
        public byte? Gender { get; set; }

        [Required]
        public bool IsRemoved { get; set; }
        public string? ForgetPasswordToken { get; set; }

        public string? Picture { get; set; }

        public IFormFile? UploadPicture { get; set; }

    }
}

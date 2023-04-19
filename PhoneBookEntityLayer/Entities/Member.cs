using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookEntityLayer.Entities
{
    [Table("Members")]
    public class Member
    {
        [Key]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "Email address max. 100 character")]
        public string Email { get; set; }

        [Required] // dbdeki not null kutucuğu gelsin aklınıza
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(50,MinimumLength =2)]
        public string Name { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Surname { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public byte[] Salt { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }
        public byte? Gender { get; set; }

        [Required]
        public bool IsRemoved { get; set; }
        public string? ForgetPasswordToken { get; set; }

        public string? Picture { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookEntityLayer.Entities
{
    [Table("PhoneTypes")]
    public class PhoneType: Base<byte>
    {
        [Required]
        [StringLength(50, MinimumLength =2)]
        public string Name { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookEntityLayer.Entities
{
    public class Deneme1
    {
        [Key]
        public byte Id { get; set; }
    }

    public class Deneme2
    {
        [Key]
        public byte Id { get; set; }
        public int ForeignKeyOlacakMi{ get; set; }
       
        [ForeignKey("ForeignKeyOlacakMi")]
        public virtual Deneme1 Deneme1 { get; set; }

        public string Deneme2Alan { get; set; }

    }
}

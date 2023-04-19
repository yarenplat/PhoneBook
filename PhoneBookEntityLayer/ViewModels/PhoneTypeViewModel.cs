using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookEntityLayer.ViewModels
{
    public class PhoneTypeViewModel // PhoneTypeDTO
    {
        public byte Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
    }
}

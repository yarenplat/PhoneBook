using PhoneBookEntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookDataLayer.InterfacesOfRepo
{
   public interface IMemberRepository:IRepository<Member, string>
    {
    }
}

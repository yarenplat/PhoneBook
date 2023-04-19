using PhoneBookDataLayer.InterfacesOfRepo;
using PhoneBookEntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookDataLayer.ImplementationsOfRepo
{
    public class MemberRepository:Repository<Member, string>, IMemberRepository
    {
        public MemberRepository(MyContext context):base(context)
        {
            //Kalıtım aldığı atasındaki ctor 1 parametre aldığı için burada o parametreyi kendisine ilettik
       
        }

        
        //_context burada kullanılabilir. Çünkü kalıtım aldığı atası Repository'de protected olarak yazıldı. 

    }
}

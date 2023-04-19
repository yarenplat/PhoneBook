using PhoneBookDataLayer.InterfacesOfRepo;
using PhoneBookEntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookDataLayer.ImplementationsOfRepo
{
    public class MemberPhoneRepository:Repository<MemberPhone,int>, IMemberPhoneRepository
    {
        public MemberPhoneRepository(MyContext context):base(context)
        {

        }
    }
}

using AutoMapper;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookDataLayer.InterfacesOfRepo;
using PhoneBookEntityLayer.Entities;
using PhoneBookEntityLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookBusinessLayer.ImplementationsOfManagers
{
    public class MemberManager:Manager<MemberViewModel,Member,string>
    , IMemberManager
    {
        public MemberManager(IMemberRepository repo, IMapper mapper)
            :base(repo, mapper,null)
        {

        }
    }
}

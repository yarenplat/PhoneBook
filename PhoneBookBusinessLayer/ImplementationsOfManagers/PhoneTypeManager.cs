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
    public class PhoneTypeManager:Manager<PhoneTypeViewModel,PhoneType, byte> , IPhoneTypeManager
    {

        public PhoneTypeManager(IPhoneTypeRepository repo, IMapper mapper):base(repo, mapper, null)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore;
using PhoneBookEntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookDataLayer
{
    public class MyContext : DbContext
    {

        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
            // ctora parameter verdik.
            // Generic bir parametre verdik.
            // Böylece connectionstring özelliğimizi OPSİYON olarak Program.cs üzerinden ayarlayacağız.

        } // ctor bitti


        // tabloların DBSet propertylerini yazmamız gerekiyor
            public DbSet<Member> MemberTable { get; set; }
            public DbSet<MemberPhone> PhoneTable { get; set; }
            public DbSet<PhoneType> PhoneType { get; set; }
           

    }
}

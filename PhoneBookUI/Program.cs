using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PhoneBookBusinessLayer.EmailSenderBusiness;
using PhoneBookBusinessLayer.ImplementationsOfManagers;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookDataLayer;
using PhoneBookDataLayer.ImplementationsOfRepo;
using PhoneBookDataLayer.InterfacesOfRepo;
using PhoneBookEntityLayer.Mappings;

var builder = WebApplication.CreateBuilder(args);

//context bilgisi eklenir

builder.Services.AddDbContext<MyContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Local"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


//CookieAuthentication ayarý eklendi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();


builder.Services.AddAutoMapper(x =>
{
    x.AddExpressionMapping();
    x.AddProfile(typeof(Maps)); //Kimin kime dönüþeceðini Maps class'ý içinde tanýmladýk. Yaptýðýmýz tanýmlamayý ayarlara ekledik.

});
      

// Add services to the container.
builder.Services.AddControllersWithViews();



//interfacelerin iþlerini gerçekleþtirecek classlarý burada yaþam döngülerini tanýmlamalýyýz.

builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberManager, MemberManager>();
    
builder.Services.AddScoped<IEmailSender, EmailSender>();


builder.Services.AddScoped<IPhoneTypeRepository, PhoneTypeRepository>();
builder.Services.AddScoped<IPhoneTypeManager, PhoneTypeManager>();

builder.Services.AddScoped<IMemberPhoneRepository, MemberPhoneRepository>();
builder.Services.AddScoped<IMemberPhoneManager, MemberPhoneManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles(); // wwwroot klasörünü görmesi için

app.UseRouting(); // browserdaki url için /home/indexe gidebilmesi için

app.UseAuthentication(); // login ve logout iþlemlerimiz için
app.UseAuthorization(); // Yetkilendirme için 



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Dashboard}/{id?}"
    ); // area route default pattern 


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // route' default pattern vermek için

app.Run(); // uygulamayý çalýþtýrýr

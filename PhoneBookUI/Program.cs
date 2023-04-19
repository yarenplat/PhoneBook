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


//CookieAuthentication ayar� eklendi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();


builder.Services.AddAutoMapper(x =>
{
    x.AddExpressionMapping();
    x.AddProfile(typeof(Maps)); //Kimin kime d�n��ece�ini Maps class'� i�inde tan�mlad�k. Yapt���m�z tan�mlamay� ayarlara ekledik.

});
      

// Add services to the container.
builder.Services.AddControllersWithViews();



//interfacelerin i�lerini ger�ekle�tirecek classlar� burada ya�am d�ng�lerini tan�mlamal�y�z.

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
app.UseStaticFiles(); // wwwroot klas�r�n� g�rmesi i�in

app.UseRouting(); // browserdaki url i�in /home/indexe gidebilmesi i�in

app.UseAuthentication(); // login ve logout i�lemlerimiz i�in
app.UseAuthorization(); // Yetkilendirme i�in 



app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Dashboard}/{id?}"
    ); // area route default pattern 


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // route' default pattern vermek i�in

app.Run(); // uygulamay� �al��t�r�r

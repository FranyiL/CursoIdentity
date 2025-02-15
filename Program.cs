using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Models;
using ProyectoIdentity.Servicios;
var builder = WebApplication.CreateBuilder(args);

//Configuramos la conexión a la base de datos
builder.Services.AddDbContext<AplicationDbContext>(options =>
    //Agregamos la cadena de conexión
    options.UseMySql(builder.Configuration.GetConnectionString("ConexionSql"),
        new MySqlServerVersion(new Version(8, 0, 30)))
);

//Accediendo a la información que se encuentra en appsettings.json
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

//Agregar el servicio de Identity a la aplicacións
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AplicationDbContext>().AddDefaultTokenProviders();

//Esta línea es para la url de retorno al acceder, cambiamos la que tenemos por defecto
//cuando un usuario no se encuentra autenticado, lo que hará es redirigirlo a la página de acceso
//cada vez que el usuario no este autenticado y quiera acceder a un recurso protegido necesite la autenicación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuentas/Acceso";
    options.AccessDeniedPath = "/Cuentas/Bloqueado";
});

// Configurar duración del token de recuperación
builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => {
    opt.TokenLifespan = TimeSpan.FromHours(2);
});
//Estás son opciones de configuración del Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

//Añadiendo claves de la autenticación de las apps externas

//Autenticación de Facebook
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = configuration["FacebookOAuth:AppId"];
    options.AppSecret = configuration["FacebookOAuth:AppSecret"];
});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = configuration["GoogleOAuth:ClientId"];
    options.ClientSecret = configuration["GoogleOAuth:ClientSecret"];
});

//Inyectando la interfaz para envío de mensajes
builder.Services.AddTransient<IMessage, Message>();

// Configurar EmailSettings
builder.Services.Configure<GmailSettings>(builder.Configuration.GetSection("GmailSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//Aquí se agrega la aurorización y autenticación
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
var builder = WebApplication.CreateBuilder(args);

//Configuramos la conexión a la base de datos
builder.Services.AddDbContext<AplicationDbContext>(options =>
    //Agregamos la cadena de conexión
    options.UseMySql(builder.Configuration.GetConnectionString("ConexionSql"),
        new MySqlServerVersion(new Version(8, 0, 30)))
); 

//Agregar el servicio de Identity a la aplicacións
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AplicationDbContext>();

//Esta línea es para la url de retorno al acceder, cambiamos la que tenemos por defecto
//cuando un usuario no se encuentra autenticado, lo que hará es redirigirlo a la página de acceso
//cada vez que el usuario no este autenticado y quiera acceder a un recurso protegido necesite la autenicación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Cuentas/Acceso";
    options.AccessDeniedPath = "/Cuentas/Bloqueado";
});

//Estás son opciones de configuración del Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    options.Lockout.MaxFailedAccessAttempts = 3;
});

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

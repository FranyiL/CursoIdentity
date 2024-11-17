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

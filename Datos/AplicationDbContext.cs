using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Models;
namespace ProyectoIdentity.Datos
{
    public class AplicationDbContext : IdentityDbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        //Agregar las entidades que se van a necesitar
        public DbSet<AppUsuario> AppUsuario { get; set; }
    }
}

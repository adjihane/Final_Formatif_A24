using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mock.depart.Models;

namespace mock.depart.Data;

public class ApplicationDbContext : IdentityDbContext<CatOwner>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Cat> Cat { get; set; } = default!;
}


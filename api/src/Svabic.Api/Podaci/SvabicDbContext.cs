using Microsoft.EntityFrameworkCore;
using Svabic.Api.Domen;

namespace Svabic.Api.Podaci;

public class SvabicDbContext(DbContextOptions<SvabicDbContext> options) : DbContext(options)
{
    public DbSet<Kategorija> Kategorije => Set<Kategorija>();
    public DbSet<Proizvod> Proizvodi => Set<Proizvod>();
    public DbSet<Pakovanje> Pakovanja => Set<Pakovanje>();
    public DbSet<VrstaPapagaja> Vrste => Set<VrstaPapagaja>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SvabicDbContext).Assembly);
    }
}

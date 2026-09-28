using Microsoft.EntityFrameworkCore;
using Svabic.Api.Domen;

namespace Svabic.Api.Podaci;

public class SvabicDbContext(DbContextOptions<SvabicDbContext> options) : DbContext(options)
{
    public DbSet<Kategorija> Kategorije => Set<Kategorija>();
    public DbSet<Proizvod> Proizvodi => Set<Proizvod>();
    public DbSet<Pakovanje> Pakovanja => Set<Pakovanje>();
}

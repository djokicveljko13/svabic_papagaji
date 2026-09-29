using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Svabic.Api.Domen;

namespace Svabic.Api.Podaci.Konfiguracije;

public class VrstaPapagajaKonfiguracija : IEntityTypeConfiguration<VrstaPapagaja>
{
    public void Configure(EntityTypeBuilder<VrstaPapagaja> builder)
    {
        builder.HasKey(v => v.Slug);

        builder.Property(v => v.Slug).HasMaxLength(60);
        builder.Property(v => v.Naziv).HasMaxLength(100);

        builder.HasData(
            new VrstaPapagaja { Slug = "braunouhi", Naziv = "Braunouhi", DanaPoKg = 60, Aktivna = true, Redosled = 1 },
            new VrstaPapagaja { Slug = "mali-aleksandar", Naziv = "Mali aleksandar", DanaPoKg = 60, Aktivna = true, Redosled = 2 },
            new VrstaPapagaja { Slug = "veliki-aleksandar", Naziv = "Veliki aleksandar", DanaPoKg = 60, Aktivna = true, Redosled = 3 },
            new VrstaPapagaja { Slug = "kina-aleksandar", Naziv = "Kina aleksandar", DanaPoKg = 60, Aktivna = true, Redosled = 4 },
            new VrstaPapagaja { Slug = "senegalski-papagaj", Naziv = "Senegalski papagaj", DanaPoKg = 60, Aktivna = true, Redosled = 5 },
            new VrstaPapagaja { Slug = "venecuela-amazonac", Naziv = "Venecuela amazonac", DanaPoKg = 30, Aktivna = true, Redosled = 6 },
            new VrstaPapagaja { Slug = "plavoceli-amazonac", Naziv = "Plavočeli amazonac", DanaPoKg = 30, Aktivna = true, Redosled = 7 },
            new VrstaPapagaja { Slug = "zutoceli-amazonac", Naziv = "Žutočeli amazonac", DanaPoKg = 30, Aktivna = true, Redosled = 8 },
            new VrstaPapagaja { Slug = "edel", Naziv = "Edel", DanaPoKg = 30, Aktivna = true, Redosled = 9 },
            new VrstaPapagaja { Slug = "zako", Naziv = "Žako", DanaPoKg = 30, Aktivna = true, Redosled = 10 },
            new VrstaPapagaja { Slug = "roze-kakadu", Naziv = "Roze kakadu", DanaPoKg = 30, Aktivna = true, Redosled = 11 },
            new VrstaPapagaja { Slug = "zutocubi-kakadu", Naziv = "Žutoćubi kakadu", DanaPoKg = 30, Aktivna = true, Redosled = 12 },
            new VrstaPapagaja { Slug = "alba-kakadu", Naziv = "Alba kakadu", DanaPoKg = 30, Aktivna = true, Redosled = 13 },
            new VrstaPapagaja { Slug = "plavo-zuta-ara", Naziv = "Plavo-žuta ara", DanaPoKg = 15, Aktivna = true, Redosled = 14 },
            new VrstaPapagaja { Slug = "zelenokrila-ara", Naziv = "Zelenokrila ara", DanaPoKg = 15, Aktivna = true, Redosled = 15 });
    }
}

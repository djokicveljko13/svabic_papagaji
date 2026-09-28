namespace Svabic.Api.Domen;

public class Pakovanje
{
    public int Id { get; set; }
    public int ProizvodId { get; set; }
    public Proizvod Proizvod { get; set; } = null!;

    public string Oznaka { get; set; } = "";

    /// <summary>Null za proizvode bez težine (kavezi, oprema).</summary>
    public int? TezinaGrama { get; set; }

    public int CenaRsd { get; set; }

    /// <summary>Null znači da se poštarina potvrđuje naknadno.</summary>
    public int? PostarinaRsd { get; set; }

    public bool Aktivno { get; set; }
    public int Redosled { get; set; }
}

namespace Svabic.Api.Domen;

public class Proizvod
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public string Naziv { get; set; } = "";
    public int KategorijaId { get; set; }
    public Kategorija Kategorija { get; set; } = null!;
    public string KratakOpis { get; set; } = "";
    public bool JeZaAre { get; set; }
    public bool Aktivan { get; set; }
    public int Redosled { get; set; }

    public List<Pakovanje> Pakovanja { get; set; } = [];
}

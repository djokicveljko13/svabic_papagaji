namespace Svabic.Api.Domen;

public class Kategorija
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public string Naziv { get; set; } = "";
    public int Redosled { get; set; }

    public List<Proizvod> Proizvodi { get; set; } = [];
}

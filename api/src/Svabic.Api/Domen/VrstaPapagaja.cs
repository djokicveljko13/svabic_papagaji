namespace Svabic.Api.Domen;

public class VrstaPapagaja
{
    public string Slug { get; set; } = "";
    public string Naziv { get; set; } = "";
    public int DanaPoKg { get; set; }
    public bool Aktivna { get; set; }
    public int Redosled { get; set; }
}

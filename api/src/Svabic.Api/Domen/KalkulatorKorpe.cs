namespace Svabic.Api.Domen;

public class KalkulatorKorpe
{
    public ObracunKorpe Izracunaj(IReadOnlyList<StavkaZaObracun> stavke, PostavkePostarine postavke)
    {
       var medjuzbir = stavke.Sum(s => s.CenaRsd * s.Kolicina);
      var postarina=stavke.Sum(s=>s.PostarinaRsd * s.Kolicina);
      var ukupno=medjuzbir+ (postarina??0);
        return new ObracunKorpe(
    MedjuzbirRsd: medjuzbir,
    PostarinaRsd: postarina,
    PostarinaNaknadno: false,
    BesplatnaPostarina: false,
    FaliDoBesplatnePostarineRsd: null,
    UkupnoRsd: ukupno);


    }
}

namespace Svabic.Api.Domen;

public record ObracunKorpe(
    int MedjuzbirRsd,
    int? PostarinaRsd,
    bool PostarinaNaknadno,
    bool BesplatnaPostarina,
    int? FaliDoBesplatnePostarineRsd,
    int UkupnoRsd);

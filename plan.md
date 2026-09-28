# Plan rada: Odgajivačnica Švabić

Cilj: novi brz, mobile-first sajt sa web shopom (plaćanje pouzećem). Pravila rada su u [CLAUDE.md](CLAUDE.md).
Procena je **oko 50 sati** ukupno. Faze se rade redom. Posle svake stavke se staje i proverava.

## Pregled faza

| # | Faza | Sati | Zavisi od |
|---|---|---|---|
| 0 | Priprema i setup repozitorijuma | 2 | – |
| 1 | API osnova (projekat, EF Core, baza, seed kataloga) | 4 | 0 |
| 2 | Obračun korpe i poštarine sa testovima | 4 | 1 |
| 3 | Porudžbina sa planom B i emailovi | 5 | 2 |
| 4 | Next osnova (dizajn sistem i layout) | 4 | 0 |
| 5 | Stranice sa sadržajem | 5 | 4 |
| 6 | Migracija sadržaja | 4 | 5 |
| 7 | Shop u Next-u (korpa, checkout) povezan sa API-jem | 6 | 3, 4 |
| 8 | Podsetnik i „Ponovi porudžbinu“ | 4 | 3, 7 |
| 9 | SEO i preusmerenja | 3 | 5, 6, 7 |
| 10 | Dodaci iza feature flaga | 3 | 7 |
| 11 | Deploy (Vercel + MonsterASP) | 3 | 1–9 |
| 12 | Testiranje i puštanje | 3 | sve |
| | **Ukupno** | **50** | |

Faza 4 može da krene paralelno sa fazama 1–3 ako zatreba, ali podrazumevani redosled je kao u tabeli.

---

## Faza 0: Priprema i setup repozitorijuma (2 h)

- [x] `git init`, grana `main`, `.gitignore` (bin/, obj/, node_modules/, .next/, .env*, *.user, secrets)
- [x] `.editorconfig` (C# i TS stil), `README.md` (opis, stack, pokretanje)
- [ ] GitHub repo (javan), zaštita `main` grane, GitHub Secrets (placeholder)
- [ ] Supabase projekat (region Frankfurt). Zapisati connection string **poolera** (IPv4)
- [ ] Proveriti najnoviju stabilnu verziju Next.js-a i .NET 10 SDK lokalno

**Provera:** `git status` je čist, repo je na GitHub-u, Supabase connection string je zapisan van repoa.

Nalozi koji ne trebaju odmah otvaraju se u fazi u kojoj se koriste: Resend (faza 3), Cloudflare Turnstile (faza 7), cron-job.org (faza 8).

## Faza 1: API osnova (4 h)

- [ ] `api/Svabic.sln`, `src/Svabic.Api` (webapi, Controllers), `tests/Svabic.Api.Tests` (xUnit)
- [ ] `Nullable` i `TreatWarningsAsErrors` uključeni. Folderi prema CLAUDE.md
- [ ] EF Core + Npgsql, `SvabicDbContext`, connection string iz user secrets
- [ ] Entiteti i konfiguracije (šema u Dodatku B), `TimeProvider` registrovan u DI
- [ ] Prva migracija `Pocetna` + primena na Supabase
- [ ] Seed: 2 mešavine, 9 pakovanja hrane (tabela ispod), kavezi i oprema u RSD (poštarina `null`), 15 vrsta, 3 grupe potrošnje, `PodesavanjaShopa` (besplatna poštarina isključena)
- [ ] `GET /api/katalog`, `GET /api/katalog/{slug}`, `GET /api/vrste`
- [ ] `GET /api/provera` (upit bazi)
- [ ] CORS (domen sajta + localhost u dev-u), ProblemDetails za greške, Swagger samo u Development-u

**Provera:** `dotnet run` → Swagger → `GET /api/katalog` vraća 2 mešavine sa 9 pakovanja, `GET /api/provera` vraća ok.

## Faza 2: Obračun korpe i poštarine sa testovima (4 h)

- [ ] `KalkulatorKorpe` (čista klasa): stavke + katalog + podešavanja → rezultat obračuna
- [ ] `IPravilaPostarine` + `PostarinaZbirPoPaketu` (privremeno pravilo, vidi otvorena pitanja)
- [ ] Poštarina „naknadno“: ako bilo koje pakovanje ima `PostarinaRsd = null`, rezultat ima `postarinaNaknadno = true` i `ukupnoRsd` je samo međuzbir sa napomenom
- [ ] Besplatna poštarina: uključeno/isključeno + prag. Vraća `faliDoBesplatnePostarineRsd` kad je uključena i ispod praga
- [ ] `POST /api/korpa/obracun` + `KorpaObracunValidator` (FluentValidation)
- [ ] xUnit testovi:
  - [ ] prazna korpa se odbija
  - [ ] jedno pakovanje: cena + poštarina iz tabele
  - [ ] više pakovanja i količina > 1: zbir po paketu
  - [ ] pakovanje bez poznate poštarine → naknadno
  - [ ] besplatna poštarina: isključena, ispod praga (tačan iznos „fali još“), tačno na pragu, iznad praga
  - [ ] nepostojeće ili neaktivno pakovanje, količina 0 ili negativna → greška validacije

**Provera:** `dotnet test` je zelen, a Swagger poziv sa 2× „Standardna 3,3 kg“ vraća 3100 + 1300 = 4400 RSD.

## Faza 3: Porudžbina sa planom B i emailovi (5 h)

- [ ] Resend nalog, dodat domen `odgajivacnicasvabic.rs` (DNS zapisi čekaju pristup DNS-u; do tada se šalje sa Resend test adrese)
- [ ] `PorudzbinaDto` + validator (ime i prezime, grad, adresa, telefon u RS formatu, email, vrsta, napomena, hitno, `kljucIdempotentnosti`, Turnstile token)
- [ ] `TurnstileServis`: provera tokena (u testovima i dev-u koriste se javni Cloudflare test ključevi, nalog nije potreban)
- [ ] `PorudzbinaServis`: ponovni obračun → snapshot stavki → broj porudžbine (npr. `SV-2026-0001`) → upis
- [ ] Idempotentnost: isti ključ vraća postojeću porudžbinu
- [ ] **Plan B:** ako upis ne uspe, kupac dobija potvrdu, a vlasnik i programer email „PORUDŽBINA NIJE SAČUVANA U BAZI“ sa svim podacima. Greška se loguje
- [ ] `IEmailServis` + `ResendEmailServis` (HTTP API)
- [ ] Email šabloni (srpski, dijakritika): potvrda kupcu, obaveštenje vlasniku, kopija programeru, plan B
- [ ] `IObavestenjeVlasniku` + email implementacija (SMS čeka odluku klijenta)
- [ ] Rate limiter: `porudzbine` 5/min po IP-u, `javno` 60/min po IP-u
- [ ] Testovi: plan B (baza baca izuzetak → poslat plan B email, kupac dobija uspeh), idempotentnost, iznos iz zahteva se ignoriše

**Provera:** porudžbina iz Swagger-a se upisuje u bazu i stižu 3 emaila. Sa pogrešnim connection string-om stiže email „PORUDŽBINA NIJE SAČUVANA U BAZI“.

## Faza 4: Next osnova (4 h)

- [ ] `create-next-app` u `web/` (TS strict, Tailwind, App Router, ESLint, pnpm)
- [ ] `trailingSlash: true`, `lang="sr"`, `next/font` (latinica + latin-ext za dijakritiku)
- [ ] Dizajn tokeni: boje, tipografija, razmaci (Tailwind tema)
- [ ] UI komponente: `Button`, `Card`, `Container`, `Naslov`, `Cena` (samo prikaz broja iz API-ja), `DugmePoziv`, `DugmeViber`
- [ ] Layout: Header, Footer (telefon, SMS, Instagram, YouTube), mobilni meni
- [ ] `lib/api/`: tipizovan klijent (`API_URL` iz env-a), tipovi odgovora
- [ ] Skripte `typecheck` i `lint` u `package.json`

**Provera:** `pnpm dev` prikazuje prazan layout na mobilnoj širini, a `pnpm build` i `pnpm typecheck` prolaze.

## Faza 5: Stranice sa sadržajem (5 h)

- [ ] Početna (hero, prodavnica hrane, vrste, utisci, kontakt)
- [ ] Papagaji: lista 15 vrsta + `papagaji/[slug]` (opis, galerija, dugmad Poziv i Viber, **bez** cena i statusa)
- [ ] Saveti: lista + `saveti/[slug]` (MDX)
- [ ] O nama, Zadovoljni kupci (utisci + slike sa kupcima), Kontakt
- [ ] Uslovi kupovine i dostave, Politika privatnosti (predlog teksta, klijent odobrava)
- [ ] Stranica 404

**Provera:** sve stranice rade na telefonu, imaju tačno jedan H1, a navigacija vodi svuda.

## Faza 6: Migracija sadržaja (4 h)

- [ ] 15 vrsta → `content/vrste/*.mdx` (slug isti kao na starom sajtu)
- [ ] 10 saveta → `content/saveti/*.mdx` (proveriti da li je `vezbe-i-igracke-2` duplikat)
- [ ] Slike: preuzimanje, izbor, konverzija u WebP/AVIF, tačne dimenzije, smisleni nazivi fajlova i alt tekstovi
- [ ] Slike iz galerije → galerije po vrstama. Slike sa kupcima → Zadovoljni kupci
- [ ] Utisci kupaca → `content/utisci.json`
- [ ] Najčešća pitanja → strukturisano (pitanje/odgovor) za FAQ schema
- [ ] Provera dijakritike u svim tekstovima

**Provera:** nijedan tekst ne fali u odnosu na sitemap starog sajta, a nijedna slika nije veća od potrebnog.

## Faza 7: Shop u Next-u (6 h)

- [ ] Cloudflare nalog + Turnstile: site key i secret key (produkcija). Do tada se koriste test ključevi
- [ ] Prodavnica: `/prodavnica/`, `/prodavnica/hrana/`, `/prodavnica/kavezi-i-oprema/`. SSG iz `GET /api/katalog`, `revalidate`
- [ ] Stranica proizvoda `/prodavnica/[slug]/` sa izborom pakovanja
- [ ] `lib/korpa/`: localStorage (`pakovanjeId` + količina), hook `useKorpa`
- [ ] Korpa: iznos i poštarina iz `POST /api/korpa/obracun`, poruka „fali još X do besplatne poštarine“, poruka „poštarina se potvrđuje naknadno“
- [ ] Checkout forma: polja iz brifa, izbor vrste iz `GET /api/vrste`, napomena, „hitno“, Turnstile
- [ ] Pamćenje podataka kupca u localStorage (uz saglasnost) + dugme „Zaboravi moje podatke“
- [ ] Stranica potvrde (broj porudžbine, šta sledi: slanje pon/pet)
- [ ] `app/api/revalidate/route.ts` (tajni ključ) + `POST /api/interno/revalidate` u API-ju
- [ ] Stanja greške i učitavanja (API nedostupan → poruka sa telefonom za SMS porudžbinu)

**Provera:** kompletna porudžbina od prodavnice do potvrde radi sa lokalnim API-jem, a posle promene cene u bazi i poziva revalidate nova cena se vidi.

## Faza 8: Podsetnik i „Ponovi porudžbinu“ (4 h)

- [ ] cron-job.org nalog
- [ ] `KalkulatorPodsetnika`: ukupno kg hrane × `DanaPoKg` grupe vrste − `PodsetnikDanaPre` → datum slanja
- [ ] Testovi: mala, velika i najveća grupa, više pakovanja, porudžbina bez hrane (nema podsetnika), prelaz meseca
- [ ] Kreiranje `Podsetnik` zapisa pri porudžbini hrane
- [ ] `TokenPonovi`: HMAC-SHA256 (`podsetnikId` + rok), base64url + testovi (važeći, istekao, izmenjen potpis, pogrešan ključ)
- [ ] `POST /api/interno/podsetnici/posalji`: stvaran upit bazi, šalje dospele, beleži `PoslatoUtc` (ne šalje dvaput), preskače odjavljene
- [ ] Email podsetnik sa dugmetom „Ponovi porudžbinu“ i linkom za odjavu
- [ ] `GET /api/ponovi/{token}` → stavke. Next stranica `/ponovi/` puni korpu, pa kupac proverava i potvrđuje
- [ ] `POST /api/odjava` + stranica odjave
- [ ] cron-job.org: 1× dnevno (npr. 09:00), header `X-Interni-Kljuc`, obaveštenje o grešci na email

**Provera:** ručni poziv internog endpointa šalje podsetnik za test porudžbinu sa datumom pomerenim u prošlost, a link iz emaila otvara popunjenu korpu.

## Faza 9: SEO i preusmerenja (3 h)

- [ ] `config/redirects.ts` prema Dodatku A (301). Dopuniti iz GSC-a kad bude dostupan
- [ ] Metadata po stranici: naslovi tipa „Prirodna hrana za papagaje, dostava pouzećem | Švabić“, opisi, canonical
- [ ] `sitemap.ts`, `robots.ts`
- [ ] JSON-LD: `LocalBusiness` (sve stranice), `Product` + `Offer` (proizvodi), `FAQPage` (najčešća pitanja)
- [ ] Open Graph slike
- [ ] Provera: jedan H1, alt tekstovi, `lang="sr"`
- [ ] Lighthouse (mobilni) na početnoj, prodavnici i proizvodu

**Provera:** svaki URL iz Dodatka A vraća 301 na tačnu adresu (skripta sa `curl -I`), a Rich Results Test prolazi.

## Faza 10: Dodaci iza feature flaga (3 h)

- [ ] `config/feature-flags.ts` (web) + flagovi u `PodesavanjaShopa` (api), podrazumevano isključeni
- [ ] **Kalkulator hrane:** `GET /api/kalkulator?vrsta=&brojPtica=` → preporučeno pakovanje i trajanje. UI na stranici vrste i u prodavnici. Testovi
- [ ] **Lista čekanja:** tabela `ListaCekanja`, `POST /api/lista-cekanja` (Turnstile, rate limit), forma na stranici vrste, `POST /api/interno/lista-cekanja/{vrsta}/obavesti`, odjava

**Provera:** kada je flag isključen, nema UI-ja i endpoint vraća 404. Kada je uključen, ceo tok radi.

## Faza 11: Deploy (3 h)

- [ ] MonsterASP: sajt, .NET 10 runtime, env varijable, subdomen `api.odgajivacnicasvabic.rs` + SSL
- [ ] GitHub Action za deploy API-ja (Web Deploy / FTP)
- [ ] Migracije na produkcionu bazu (`migrations script --idempotent`)
- [ ] Vercel: projekat iz `web/`, env varijable, domen
- [ ] `ci.yml`: build + test (api), lint + typecheck + build (web)
- [ ] `backup.yml`: nedeljni `pg_dump` (verzija ista kao server) → `gpg --symmetric` (lozinka u Secrets) → artifact (90 dana)
- [ ] Probni restore backup-a u lokalni Postgres
- [ ] Monitoring: `/api/provera` na UptimeRobot-u ili cron-job.org

**Provera:** produkcioni sajt čita katalog sa produkcionog API-ja, backup artifact postoji i dešifruje se.

## Faza 12: Testiranje i puštanje (3 h)

- [ ] End-to-end porudžbina na produkciji (test proizvod ili stornirati)
- [ ] Plan B na staging-u/lokalno (neispravna baza)
- [ ] Svi emailovi stižu (i ne završavaju u spamu: SPF, DKIM, DMARC)
- [ ] Test na realnim telefonima (Android + iOS), Viber i poziv linkovi
- [ ] Lighthouse 90+ (mobilni) na početnoj, prodavnici i proizvodu
- [ ] Provera svih 301 preusmerenja na produkcionom domenu
- [ ] Prebacivanje DNS-a, sitemap predat Google Search Console-u
- [ ] Praćenje 404 u GSC-u prvih nedelju dana i dopuna preusmerenja

**Provera:** sajt je na glavnom domenu, stari URL-ovi rade, prva prava porudžbina je stigla.

---

## Početni podaci kataloga (seed)

| Mešavina | Pakovanje | Cena (RSD) | Poštarina (RSD) |
|---|---|---|---|
| Standardna (sve vrste osim ara) | 1,8 kg vakum | 1100 | 450 |
| Standardna | 3,3 kg džak | 1550 | 650 |
| Standardna | 4,8 kg džak | 2200 | 650 |
| Standardna | 9,8 kg džak | 4400 | 800 |
| Standardna | 19,8 kg džak | 9000 | 1100 |
| Za are | 3,3 kg džak | 1750 | 650 |
| Za are | 4,8 kg džak | 2500 | 650 |
| Za are | 9,8 kg džak | 4900 | 800 |
| Za are | 19,8 kg džak | 9900 | 1100 |

Kavezi i oprema sa starog sajta (13 stavki, poštarina `null` = naknadno). Cene u EUR treba preračunati u RSD i potvrditi sa klijentom:

| Proizvod | Varijanta | Cena na starom sajtu |
|---|---|---|
| Osnovni žičani kavez | po vrsti papagaja | nema fiksne cene |
| Ojačani žičani, model 1 | 1a 45×45×60 / 1b 60×60×70 cm | 120 € / 150 € |
| Ojačani žičani nerasklapajući, model 3 | 60×70×150 cm | 250 € |
| Ojačani žičani nerasklapajući, model 4 | 90×70×155 cm | 350 € |
| Ojačani žičani nerasklapajući, model 5 | 60×70×145 cm | 300 € |
| Profesionalni rasklapajući, model 6 | 60×64×135 cm | 350 € |
| Profesionalni rasklapajući, model 7 | 70×70×155 cm | 420 € |
| Profesionalni rasklapajući, model 8 | 80×84×145 cm | 450 € |
| Profesionalni rasklapajući, model 9 | 80×84×175 cm | 780 € |
| Profesionalni rasklapajući, model 10 | 97×97×185 cm | 680 € |
| Stajalice od tvrdog drveta | 4 kom / 1 kom | 2400 / 750 RSD |
| Stajalice igrališta | razne | 5000–20000 RSD |
| Povodac za papagaje | – | 1500 RSD |

Grupe potrošnje (predlog, potvrditi sa klijentom):

| Grupa | Dana po kg | Vrste |
|---|---|---|
| Mala | 60 | Braunouhi, Mali aleksandar, Kina aleksandar, Veliki aleksandar, Senegalski papagaj |
| Velika | 30 | Žako, Edel, Venecuela amazonac, Plavočeli amazonac, Žutočeli amazonac, Roze kakadu, Žutoćubi kakadu, Alba kakadu |
| Najveća | 15 | Plavo-žuta ara, Zelenokrila ara |

Poznato iz brifa: mali aleksandar oko 2 meseca po kg, žako oko 1 mesec, ara oko 15 dana. Ostale vrste su raspoređene po veličini i čekaju potvrdu.

---

## Dodatak A: tabela 301 preusmerenja

Izvor je sitemap starog sajta (`/sitemap_index.xml`, Yoast, proveren 28. 9. 2026): 27 postova, 7 stranica, 2 kategorije, 15 tagova i 1 author. **U sitemap-u nema `/uncategorized/` URL-ova.** Dopuniti iz Google Search Console-a kad bude dostupan.

**Vrste: URL ostaje isti, bez preusmerenja** (`trailingSlash: true`):
`/papagaji/braunouhi/`, `/papagaji/mali-aleksandar/`, `/papagaji/veliki-aleksandar/`, `/papagaji/kina-aleksandar/`, `/papagaji/senegalski-papagaj/`, `/papagaji/venecuela-amazonac/`, `/papagaji/plavoceli-amazonac/`, `/papagaji/zutoceli-amazonac/`, `/papagaji/edel/`, `/papagaji/zako/`, `/papagaji/roze-kakadu/`, `/papagaji/plavo-zuta-ara/`, `/papagaji/zutocubi-kakadu/`, `/papagaji/alba-kakadu/`, `/papagaji/zelenokrila-ara/`

**Stranice koje ostaju iste:** `/`, `/o-nama/`, `/kontakt/`, `/zadovoljni-kupci/`

**Preusmerenja (301):**

| Stari URL | Novi URL |
|---|---|
| `/pocetna-stranica/` | `/` |
| `/galerija-slika/` | `/papagaji/` |
| `/hrana-i-oprema/` | `/prodavnica/` |
| `/hrana-i-oprema/hrana-za-papagaje/` | `/prodavnica/hrana/` |
| `/hrana-i-oprema/ishrana-papagaja/` | `/saveti/ishrana-papagaja/` |
| `/hrana-i-oprema/spisak-opreme-za-vaseg-papagaja/` | `/saveti/spisak-opreme-za-vaseg-papagaja/` |
| `/papagaji/kavezi-i-oprema/` | `/prodavnica/kavezi-i-oprema/` |
| `/papagaji/vezbe-i-igracke/` | `/saveti/vezbe-i-igracke/` |
| `/papagaji/vezbe-i-igracke-2/` | `/saveti/vezbe-i-igracke/` (ako je duplikat, inače svoj slug) |
| `/papagaji/korisne-informacije/` | `/saveti/korisne-informacije/` |
| `/papagaji/najcesca-pitanja/` | `/saveti/najcesca-pitanja/` |
| `/papagaji/opsta-briga/` | `/saveti/opsta-briga/` |
| `/papagaji/smestaj/` | `/saveti/smestaj/` |
| `/papagaji/uzivajte-zajedno/` | `/saveti/uzivajte-zajedno/` |
| `/papagaji/papagaji-obozavaju-vodu/` | `/saveti/papagaji-obozavaju-vodu/` |
| `/category/papagaji/` | `/papagaji/` |
| `/category/hrana-i-oprema/` | `/prodavnica/` |
| `/tag/mali-aleksandar/` | `/papagaji/mali-aleksandar/` |
| `/tag/zako/` | `/papagaji/zako/` |
| `/tag/aleksandar/` | `/papagaji/` |
| `/tag/amazonac/` | `/papagaji/` |
| `/tag/ara/` | `/papagaji/` |
| `/tag/kakadu/` | `/papagaji/` |
| `/tag/papagaji/` | `/papagaji/` |
| `/tag/pitomi-papagaji/` | `/papagaji/` |
| `/tag/rucno-hranjeni-papagaju/` | `/papagaji/` |
| `/tag/igracke/` | `/saveti/vezbe-i-igracke/` |
| `/tag/igracke-za-papagaje/` | `/saveti/vezbe-i-igracke/` |
| `/tag/oprema-za-papagaje/` | `/prodavnica/kavezi-i-oprema/` |
| `/tag/odgajivacnica/` | `/o-nama/` |
| `/tag/odgajivacnica-papagaja/` | `/o-nama/` |
| `/tag/odgajivacnica-svabic/` | `/o-nama/` |
| `/author/admin/` | `/o-nama/` |
| `/uncategorized/:slug*` | `/saveti/` (privremeno, dok ne stignu podaci iz GSC-a) |
| `/sitemap_index.xml`, `/wp-sitemap.xml`, `/*-sitemap.xml` | `/sitemap.xml` |
| `/feed/` | `/saveti/` |

Napomena: `/wp-content/uploads/*` (stare slike) se ne preusmeravaju. Vraćaju 404, a Google ih vremenom izbacuje iz indeksa.

## Dodatak B: predlog šeme baze

Novac je `int` (RSD). Vreme je `timestamptz` (UTC). Nazivi prate entitete.

**Proizvod**: katalog koji shop prodaje (mešavina hrane, kavez, oprema).
`Id`, `Slug` (unique), `Naziv`, `Kategorija` (Hrana / Kavez / Oprema), `KratakOpis`, `JeZaAre` (bool), `Aktivan`, `Redosled`

**Pakovanje**: ono što se stvarno kupuje, sa svojom cenom i poštarinom (za kavez je to jedan „komad“).
`Id`, `ProizvodId` (FK), `Oznaka` („3,3 kg džak“), `TezinaGrama` (null za kaveze), `CenaRsd`, `PostarinaRsd` (null = potvrđuje se naknadno), `Aktivno`, `Redosled`

**GrupaPotrosnje**: tri podesive grupe dok klijent ne da tačnu potrošnju po vrsti.
`Kod` (PK: Mala / Velika / Najveca), `Naziv`, `DanaPoKg`

**VrstaPapagaja**: da API može da proveri vrstu iz checkout-a i zna koliko brzo troši hranu (opis i slike su u Next MDX-u, isti slug).
`Slug` (PK), `Naziv`, `GrupaPotrosnjeKod` (FK), `Aktivna`, `Redosled`

**PodesavanjaShopa**: konfiguracija koju programer menja u bazi, bez redeploy-a (jedan red).
`Id` (uvek 1), `BesplatnaPostarinaUkljucena`, `BesplatnaPostarinaPragRsd`, `PodsetnikDanaPre`, `TokenPonoviVaziDana`, `KalkulatorUkljucen`, `ListaCekanjaUkljucena`

**Porudzbina**: trajni zapis porudžbine sa kopijom podataka kupca (nema naloga ni tabele kupaca).
`Id` (Guid), `Broj` (unique, npr. SV-2026-0001), `KreiranoUtc`, `ImePrezime`, `Grad`, `Adresa`, `Telefon`, `Email`, `VrstaSlug`, `Napomena`, `Hitno`, `MedjuzbirRsd`, `PostarinaRsd` (null ako je naknadno), `PostarinaNaknadno`, `BesplatnaPostarina`, `UkupnoRsd`, `Status` (Nova / Poslata / Otkazana), `KljucIdempotentnosti` (unique)

**StavkaPorudzbine**: čuva cenu u trenutku kupovine, da se stara porudžbina ne promeni kad se promeni katalog.
`Id`, `PorudzbinaId` (FK), `PakovanjeId` (FK), `NazivSnapshot`, `CenaRsdSnapshot`, `PostarinaRsdSnapshot` (null), `TezinaGramaSnapshot` (null), `Kolicina`

**Podsetnik**: jedan planiran email po porudžbini hrane. Dnevni posao čita dospele.
`Id` (Guid, ide u potpisan token), `PorudzbinaId` (FK), `Email`, `DatumSlanja` (date), `PoslatoUtc` (null), `Otkazan`

**OdjavaEmail**: da se poštuje odjava od podsetnika i liste čekanja.
`Email` (PK, normalizovan: mala slova, bez razmaka), `KreiranoUtc`

**ListaCekanja** *(dodatak)*: ko čeka mladunce koje vrste.
`Id`, `Email`, `VrstaSlug` (FK), `KreiranoUtc`, `ObavestenoUtc` (null). Unique (`Email`, `VrstaSlug`)

## Dodatak C: spisak API endpointa

| Metoda | Ruta | Ko poziva | Opis |
|---|---|---|---|
| GET | `/api/katalog` | Next (build / ISR) | Svi aktivni proizvodi sa pakovanjima i cenama |
| GET | `/api/katalog/{slug}` | Next (build / ISR) | Jedan proizvod sa pakovanjima |
| GET | `/api/vrste` | Next (checkout, kalkulator) | Slugovi i nazivi vrsta |
| POST | `/api/korpa/obracun` | Browser (korpa, checkout) | Stavke → cene, međuzbir, poštarina ili „naknadno“, ukupno, „fali još X“ |
| POST | `/api/porudzbine` | Browser (checkout) | Turnstile, ponovni obračun, upis ili plan B, emailovi. Vraća broj porudžbine |
| GET | `/api/ponovi/{token}` | Browser (stranica `/ponovi/`) | Proverava potpis i rok, vraća stavke za korpu |
| POST | `/api/odjava` | Browser (link iz emaila) | Odjava od podsetnika i liste čekanja (potpisan token) |
| GET | `/api/provera` | Monitoring (UptimeRobot / cron) | Upit bazi → `ok` ili greška (503) |
| POST | `/api/interno/podsetnici/posalji` | cron-job.org, 1× dnevno | Šalje dospele podsetnike, stvaran upit bazi |
| POST | `/api/interno/revalidate` | Programer | Posle promene kataloga poziva Next `/api/revalidate` |
| GET | `/api/kalkulator` | Browser *(dodatak)* | Vrsta + broj ptica → preporučeno pakovanje i trajanje |
| POST | `/api/lista-cekanja` | Browser *(dodatak)* | Upis emaila za vrstu (Turnstile) |
| POST | `/api/interno/lista-cekanja/{vrsta}/obavesti` | Programer *(dodatak)* | Email svima sa liste za tu vrstu |
| POST | `/api/revalidate` **(Next)** | .NET API | On-demand ISR za prodavnicu i proizvode, tajni ključ |

- `/api/interno/*` zahteva header `X-Interni-Kljuc`. Bez njega API vraća 401.
- Rate limit po IP-u: `obracun` 60/min, `porudzbine` 5/min, `lista-cekanja` 5/min, ostali javni 60/min.
- Swagger je dostupan samo u Development-u.

## Dodatak D: otvorena pitanja

### Za klijenta

1. **Poštarina za više paketa u korpi:** kako se računa? Privremeno je ugrađen zbir po paketu.
2. **Poštarina za kaveze i teže pakete:** koliko košta? Do odgovora sajt prikazuje „poštarina se potvrđuje naknadno“.
3. **Kanal obaveštenja o porudžbini:** SMS ili email? Na koji broj ili adresu? (SMS ima mesečni trošak provajdera.)
4. **Potrošnja hrane po vrsti:** potvrda grupa iz tabele iznad za svih 15 vrsta.
5. **Broj ptica u checkout-u:** da li dodati polje? Podsetnik bi tada bio tačniji za kupce sa više ptica.
6. **Cene kaveza u RSD:** stare cene su u EUR. Koliko košta „Osnovni žičani“ kavez po vrsti, a koliko „Stajalice igrališta“? Da li se svi kavezi i dalje prodaju?
7. **Besplatna poštarina:** da li se uključuje na početku, sa kojim pragom, i da li važi i za kaveze?
8. **„Hitno“ slanje (sledeći dan):** da li ima doplatu?
9. **Slike kupaca:** saglasnost za objavljivanje na Zadovoljnim kupcima.
10. **Podaci o prodavcu** u uslovima kupovine i politici privatnosti (nema registrovane firme): čije ime i adresa? Preporuka je provera sa knjigovođom ili pravnikom.
11. **Email adresa vlasnika** za obaveštenja i kao pošiljalac odgovora (reply-to).
12. **Pristup DNS-u domena** (za Resend SPF/DKIM i prebacivanje na Vercel). Gde je domen registrovan?
13. **Facebook profil:** da li ga prikazati uz Instagram i YouTube?
14. **Google Search Console / WP admin:** pristup za pun spisak starih URL-ova (`/uncategorized/`).
15. **Dodaci:** da li bira kalkulator hrane i/ili listu čekanja (po 20 EUR)?

### Za hosting i infrastrukturu

1. **MonsterASP idle timeout i recycle app pool-a** na Premium paketu. Da li postoji „Always On“? (Utiče na prvi zahtev posle pauze i na cold start za checkout.)
2. **Podrška za .NET 10** na MonsterASP-u (runtime i hosting bundle).
3. **Odlazne HTTPS konekcije:** da li su dozvoljene ka Resend-u, Cloudflare Turnstile-u i Vercel-u?
4. **Konekcija ka Supabase-u:** direktna konekcija je samo IPv6, pa koristimo pooler (IPv4). Proveriti session vs transaction mode sa Npgsql-om (transaction mode traži isključene prepared statements).
5. **Subdomen `api.odgajivacnicasvabic.rs`** + besplatan SSL na MonsterASP-u.
6. **Env varijable:** kako se postavljaju (panel ili `web.config` `environmentVariables`), tako da tajne ne završe u repou.
7. **Deploy:** Web Deploy ili FTP iz GitHub Actions? Kredencijali idu u Secrets.
8. **Supabase free plan:** pauza posle 7 dana bez aktivnosti (rešava dnevni upit), 500 MB, nema automatskog backup-a (rešava naš `pg_dump`).
9. **`pg_dump` verzija** u GitHub Action-u mora odgovarati verziji Postgres-a na Supabase-u.
10. **GitHub gasi zakazane workflow-e posle 60 dana** bez commit-a u javnom repou. Za backup treba keepalive ili spoljni trigger (`workflow_dispatch` preko cron-job.org).
11. **Resend free limit** (100 emailova dnevno, 3000 mesečno): dovoljno za početak, pratiti pri slanju podsetnika i liste čekanja.
12. **IP adresa klijenta za rate limiting:** ako ispred API-ja stoji proxy (IIS ARR / Cloudflare), podesiti `ForwardedHeaders`.

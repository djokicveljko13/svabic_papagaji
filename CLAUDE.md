# CLAUDE.md: Odgajivačnica Švabić

Uputstva za rad na ovom repozitorijumu. Pročitaj pre svake izmene. Plan rada, šema baze i spisak endpointa su u [plan.md](plan.md).

## Projekat ukratko

- **Klijent:** Odgajivačnica Švabić, odgajivač papagaja iz Srbije. Stari sajt: https://odgajivacnicasvabic.rs (WordPress + Elementor, spor).
- **Izvođač:** Grafo Dizajn. Projekat je i deo portfolija, pa su čitljiv kod i čista arhitektura deo isporuke.
- **Cilj:** brz, mobile-first sajt sa web shopom (hrana, kavezi i oprema). Plaćanje je **isključivo pouzećem**, online plaćanja nema.
- **Nema admin panela.** Klijent ne koristi računar. Cene, proizvode, vrste, tekstove i slike menja programer (SQL/seed migracija za katalog, MDX/JSON za sadržaj).

## Podela odgovornosti: „Next prikazuje, .NET odlučuje“

| | `/web` (Next.js) | `/api` (ASP.NET Core) |
|---|---|---|
| Uloga | Prikaz, SEO, sadržaj, korpa u browseru | Poslovna logika shopa, jedini izvor istine |
| Podaci | Vrste papagaja, saveti (blog), utisci: MDX/JSON u repou | Proizvodi, pakovanja, cene, poštarina, porudžbine, podsetnici |
| Računa | Ništa što ima veze sa novcem | Cene, poštarinu, besplatnu poštarinu, ukupan iznos, datume podsetnika |
| Baza | Nikad ne piše u bazu | Jedini pristupa bazi (EF Core) |

- Katalog se u Next-u generiše statički (SSG/ISR) iz `GET /api/katalog`. Posle promene cene API poziva Next `/api/revalidate`.
- Iznos koji kupac vidi pre potvrde uvek dolazi iz `POST /api/korpa/obracun`.
- Korpa u browseru čuva **samo** `pakovanjeId` i količinu, nikad cenu.

## Stack

**`/web`**
- Next.js (najnovija stabilna verzija, proveriti pri setup-u), App Router, TypeScript `strict`, Tailwind CSS
- pnpm, MDX za sadržaj, `next/image` i `next/font`
- Cloudflare Turnstile na checkout-u (token proverava API)
- Hosting: Vercel

**`/api`**
- .NET 10 (LTS), C#, ASP.NET Core Web API sa **Controllers**
- EF Core + Npgsql. Baza je Supabase Postgres (region Frankfurt, konekcija preko Supabase poolera), koristi se samo kao Postgres.
- FluentValidation, Resend (HTTP API) za email, Swagger/OpenAPI samo u Development-u
- xUnit za testove
- Hosting: MonsterASP.NET Premium (deljeni IIS, EU)

## Arhitektonske odluke

Format: **šta**, zašto, alternativa.

1. **Controllers, ne Minimal API.** Svaki resurs ima svoj fajl, struktura je jasna i česta u poslu. Alternativa: Minimal API ima manje koda, ali se lako sve nagomila u `Program.cs`.
2. **Dva projekta: `Svabic.Api` + `Svabic.Api.Tests`.** Slojevi su folderi, a ne zasebni projekti. Alternativa: Clean Architecture sa 4 projekta je previše ceremonije za ovaj obim.
3. **Bez CQRS, MediatR i generičkog repozitorijuma preko EF-a.** Servis direktno koristi `DbContext`, koji je već Unit of Work + Repository. Uvodi se samo uz jasno obrazloženje.
4. **FluentValidation.** Pravila su odvojena od DTO-a, lako se testiraju i podržavaju uslovna i asinhrona pravila (npr. da li pakovanje postoji i aktivno je). Alternativa: DataAnnotations su dovoljni za proste slučajeve, ali su nezgrapni za složena pravila.
5. **Novac je `int` u celim dinarima (RSD).** Nema decimala ni grešaka zaokruživanja. Kavezi čije su cene na starom sajtu u EUR unose se ručno preračunati u RSD. Alternativa: `decimal`, koji ovde ne treba.
6. **Obračuni su čiste klase bez baze** (`KalkulatorKorpe`, `IPravilaPostarine`, `KalkulatorPodsetnika`). Primaju podatke, vraćaju rezultat i testiraju se bez baze. Vreme se čita preko `TimeProvider`-a, nikad `DateTime.Now`.
7. **Poštarina za više paketa je iza interfejsa `IPravilaPostarine`.** Trenutna implementacija sabira poštarinu po paketu, dok klijent ne potvrdi pravilo. Pakovanje sa `PostarinaRsd = null` znači da se poštarina potvrđuje naknadno. Tada API vraća `postarinaNaknadno = true` i ne obećava tačan ukupan iznos.
8. **Nema tabele `Kupac` ni naloga.** Podaci kupca se kopiraju u `Porudzbina`. Browser ih pamti u localStorage, **ne** po broju telefona.
9. **„Ponovi porudžbinu“ token:** `podsetnikId (Guid) + rok važenja`, potpisan HMAC-SHA256 ključem iz env-a, u base64url obliku. U URL-u nema ličnih podataka. Alternativa: ASP.NET Data Protection, koji na deljenom hostingu traži trajno čuvanje ključeva.
10. **Obaveštenje vlasniku ide preko `IObavestenjeVlasniku`.** Za sada postoji implementacija za email, a SMS se dodaje kao nova implementacija.
11. **Idempotentnost porudžbine:** browser šalje `kljucIdempotentnosti` (GUID). Ponovljen zahtev vraća postojeću porudžbinu umesto nove.
12. **Poslovi bez `BackgroundService`.** Aplikacija na deljenom IIS-u može da se uspava ili restartuje. Dnevne poslove pokreće spoljni cron (cron-job.org) pozivom internog endpointa.

## Struktura foldera

```
/
├─ CLAUDE.md, plan.md, README.md
├─ .github/workflows/
│   ├─ ci.yml                 build + test za web i api
│   └─ backup.yml             nedeljni pg_dump → gpg šifrovanje → artifact
├─ web/
│   ├─ app/
│   │   ├─ (sajt)/            javne stranice: page.tsx (početna), papagaji/[slug], saveti/[slug],
│   │   │                     prodavnica/[slug], korpa, porudzbina, potvrda, ponovi, o-nama, kontakt, ...
│   │   ├─ api/revalidate/    jedini Next route handler (webhook iz .NET API-ja)
│   │   ├─ sitemap.ts, robots.ts
│   ├─ components/
│   │   ├─ ui/                dizajn sistem (Button, Card, Container, ...)
│   │   ├─ layout/            Header, Footer, MobilniMeni
│   │   └─ shop/              KorpaStavka, IzborPakovanja, CheckoutForma, ...
│   ├─ content/
│   │   ├─ vrste/*.mdx        15 vrsta (slug = slug sa starog sajta)
│   │   ├─ saveti/*.mdx       blog tekstovi
│   │   └─ utisci.json        zadovoljni kupci
│   ├─ lib/
│   │   ├─ api/               tipizovan klijent ka .NET API-ju
│   │   ├─ korpa/             stanje korpe u localStorage (bez cena)
│   │   └─ seo/               metadata i JSON-LD pomoćne funkcije
│   ├─ config/
│   │   ├─ redirects.ts       tabela 301 preusmerenja sa starog sajta
│   │   └─ feature-flags.ts   dodaci (kalkulator, lista čekanja)
│   └─ public/
└─ api/
    ├─ Svabic.sln
    ├─ src/Svabic.Api/
    │   ├─ Kontroleri/        tanki kontroleri: validacija → servis → odgovor
    │   ├─ Domen/             entiteti i čiste klase obračuna (KalkulatorKorpe, PravilaPostarine, ...)
    │   ├─ Servisi/           orkestracija (PorudzbinaServis, PodsetnikServis, ...)
    │   ├─ Podaci/            SvabicDbContext, Konfiguracije/, Migracije/, Seed/
    │   ├─ Dto/               ulazni i izlazni modeli API-ja
    │   ├─ Validacija/        FluentValidation validatori
    │   ├─ Infrastruktura/    Email (Resend), Turnstile, Revalidate, InterniKljucFilter
    │   └─ Program.cs
    └─ tests/Svabic.Api.Tests/
```

## Konvencije imenovanja

- **Domenski nazivi na srpskom, bez dijakritika:** `Porudzbina`, `StavkaPorudzbine`, `Pakovanje`, `Proizvod`, `VrstaPapagaja`, `Podsetnik`, `IzracunajPostarinu`, `KalkulatorKorpe`.
- **Framework termini ostaju na engleskom:** `Controller`, `DbContext`, `Service`, `Validator`, `Middleware`, `Program`. Primer: `PorudzbineController`, `PorudzbinaValidator`.
- **C#:** PascalCase za tipove, metode i svojstva, camelCase za lokalne promenljive i parametre, `_camelCase` za privatna polja. Async metode dobijaju sufiks `Async`. Uključeni `Nullable` i `TreatWarningsAsErrors`.
- **Tabele i kolone:** isti nazivi kao entiteti (PascalCase, u navodnicima u Postgres-u), podešeni u `Konfiguracije/`.
- **TypeScript:** camelCase za promenljive i funkcije, PascalCase za komponente i tipove, fajlovi kebab-case (`checkout-forma.tsx`).
- **Rute i slugovi:** srpski, mala slova, bez dijakritika, sa završnom kosom crtom (`trailingSlash: true`): `/papagaji/mali-aleksandar/`, `/prodavnica/hrana/`, `/saveti/ishrana-papagaja/`.
- **API JSON:** camelCase (`cenaRsd`, `postarinaNaknadno`).
- **Tekst na sajtu i u emailovima:** srpska latinica **sa dijakritikom** (č, ć, š, ž, đ).

## Obavezna pravila

1. **Cena i poštarina se računaju isključivo u API-ju.** Iznos iz browsera se nikad ne koristi. API uvek ponovo računa iz baze.
2. **Plan B za porudžbinu:** API prvo upisuje u bazu. Ako upis ne uspe, kupac i dalje dobija potvrdu, a vlasniku i programeru stiže email sa svim podacima i naslovom **„PORUDŽBINA NIJE SAČUVANA U BAZI“**.
3. **Snapshot:** stavka porudžbine čuva naziv, cenu i poštarinu iz trenutka kupovine.
4. **Poštarina nepoznata → ne obećavaj iznos.** Prikazuje se „poštarina se potvrđuje naknadno“.
5. **CORS** dozvoljava samo domen sajta (i `localhost` u Development-u).
6. **Rate limiting** na svim javnim endpointima (ugrađeni ASP.NET Core rate limiter, po IP-u).
7. **Interni endpointi** (`/api/interno/*`) traže tajni ključ u headeru `X-Interni-Kljuc`.
8. **Dnevni posao za podsetnike** šalje stvaran upit bazi (sprečava pauziranje free Supabase projekta).
9. **`GET /api/provera`** za monitoring radi upit bazi i vraća ok ili grešku.
10. **Nedeljni `pg_dump` backup** ide preko GitHub Action-a. Dump se **šifruje** pre čuvanja jer je repo javan.
11. **Tajne** (connection string, API ključevi, HMAC ključ, interni ključ) postoje samo u env varijablama ili user secrets, **nikad u repou**. `appsettings.json` sadrži samo prazne ključeve.
12. **Podaci kupca** se pamte samo u localStorage na uređaju kupca. Nema pretrage po telefonu ili emailu.
13. **Performanse:** Lighthouse 90+ na mobilnom za početnu, prodavnicu i stranicu proizvoda. Slike su WebP/AVIF, sa tačnim dimenzijama i lazy load-om (osim LCP slike).
14. **SEO:** jedan H1 po stranici, `lang="sr"`, alt tekst na svakoj slici, sitemap, robots i strukturisani podaci.
15. **Dodaci** (kalkulator hrane, lista čekanja) su iza feature flaga i podrazumevano isključeni.

## Nikako ne sme

- Računati cenu, poštarinu, popust ili ukupan iznos u Next-u (ni „za prikaz“).
- Slati iznos iz browsera API-ju ili ga u API-ju koristiti.
- Pisati u bazu iz Next-a ili se iz Next-a direktno povezivati na bazu.
- Stavljati tajne u repo, u `appsettings.json` ili u `NEXT_PUBLIC_*` promenljive.
- Pretraživati ili prikazivati podatke kupca po broju telefona.
- Koristiti `BackgroundService` / `IHostedService` za poslove.
- Uvoditi CQRS, MediatR ili generički repozitorijum bez obrazloženja.
- Stavljati lične podatke u URL (tokeni sadrže samo potpisan ID i rok).
- Menjati ili brisati migraciju koja je već primenjena. Pravi se nova migracija.
- Pisati tekst na sajtu bez dijakritike ili ćirilicom.
- Koristiti `DateTime.Now` u logici. Koristi se `TimeProvider`.

## Komande

**Web** (iz `web/`):
```bash
pnpm install          # zavisnosti
pnpm dev              # razvojni server (http://localhost:3000)
pnpm build            # produkcijski build (SSG, traži dostupan API)
pnpm lint             # ESLint
pnpm typecheck        # tsc --noEmit
```

**API** (iz root-a):
```bash
dotnet run --project api/src/Svabic.Api          # razvojni server + Swagger
dotnet build api/Svabic.sln
dotnet test api/Svabic.sln
```

**Migracije** (alat: `dotnet tool install --global dotnet-ef`):
```bash
dotnet ef migrations add <NazivMigracije> --project api/src/Svabic.Api --output-dir Podaci/Migracije
dotnet ef database update --project api/src/Svabic.Api
dotnet ef migrations script --idempotent --project api/src/Svabic.Api   # SQL za produkciju
```

**Tajne u developmentu:**
```bash
dotnet user-secrets set "ConnectionStrings:Baza" "<supabase pooler connection string>" --project api/src/Svabic.Api
dotnet user-secrets set "Resend:ApiKljuc" "<kljuc>" --project api/src/Svabic.Api
```

## Način rada

- **Pitaj pre svakog novog koraka.** Odobrenje prethodnog koraka nije dozvola za sledeći. Predloži sledeći korak i sačekaj potvrdu. Kad je korak prost (git komanda, komanda u terminalu), ponudi da ga korisnik uradi sam uz uputstvo.
- **Mali koraci.** Posle svakog koraka stani i napiši šta je urađeno i kako da se proveri (komanda, URL, očekivan rezultat).
- **Objašnjavaj odluke** kratko i jednostavno: šta, zašto, koja je alternativa. Korisnik je junior i uči .NET.
- Posle završenog zadatka čekiraj stavku u `plan.md`.
- Testovi su obavezni za obračun korpe i poštarine, besplatnu poštarinu, datum podsetnika i token „Ponovi porudžbinu“.
- Pre commit-a: `dotnet test` i `pnpm lint && pnpm typecheck` moraju da prođu.

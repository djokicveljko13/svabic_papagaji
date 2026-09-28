# Odgajivačnica Švabić

Novi sajt sa web shopom za Odgajivačnicu Švabić, odgajivača papagaja iz Srbije. Brz, mobile-first, sa porudžbinom pouzećem.
Izrada: Grafo Dizajn.

## Delovi

| Folder | Šta je | Stack | Hosting |
|---|---|---|---|
| [`web/`](web/) | Sajt: prikaz, sadržaj, korpa | Next.js (App Router), TypeScript, Tailwind | Vercel |
| [`api/`](api/) | Shop logika: katalog, obračun, porudžbine, podsetnici | ASP.NET Core (.NET 10), EF Core, Postgres (Supabase) | MonsterASP.NET |

Osnovno pravilo: **Next prikazuje, .NET odlučuje.** Cene i poštarina se računaju isključivo u API-ju.

## Pokretanje

Potrebno: .NET 10 SDK, Node 24+, pnpm (`corepack enable pnpm`).

```bash
# API
dotnet user-secrets set "ConnectionStrings:Baza" "<connection string>" --project api/src/Svabic.Api
dotnet run --project api/src/Svabic.Api

# Web
cd web
pnpm install
pnpm dev
```

Testovi: `dotnet test api/Svabic.sln` i `pnpm lint && pnpm typecheck` u `web/`.

## Dokumentacija

- [CLAUDE.md](CLAUDE.md): arhitektura, konvencije, pravila i komande
- [plan.md](plan.md): faze rada, šema baze, API endpointi i otvorena pitanja

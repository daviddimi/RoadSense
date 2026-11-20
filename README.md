
# RoadSense

**A road-safety web app for Bulgaria: how risky is your route, hour by hour?**

Built for **FMI Open Data Hackaton** at Veliko Tarnovo University (20.11.2026).

<!-- Add 2–4 screenshots here: docs/route.png, docs/weather.png, docs/stats.png -->
<img width="1906" height="1078" alt="Screenshot 2026-11-20 230538" src="https://github.com/user-attachments/assets/55ee6995-1f66-4bb8-bb16-5d1e58688fb2" />
<img width="1910" height="1079" alt="Screenshot 2026-11-20 230618" src="https://github.com/user-attachments/assets/0cffeb0b-a74e-4885-a1e1-acc89fc48bcb" />
<img width="1903" height="1079" alt="Screenshot 2026-11-20 230630" src="https://github.com/user-attachments/assets/043a1804-9e4d-45d4-833f-a4a04b8f5d29" />
<img width="1907" height="1079" alt="Screenshot 2026-11-20 230638" src="https://github.com/user-attachments/assets/8f00494e-f27b-4120-b71b-1520bf7ab854" />
<img width="1905" height="1079" alt="Screenshot 2026-11-20 230701" src="https://github.com/user-attachments/assets/f7245a2b-756c-4384-b748-aef60bb2869a" />
<img width="1901" height="1078" alt="Screenshot 2026-11-20 230708" src="https://github.com/user-attachments/assets/5c0ce204-7aaf-46b3-b8a9-8f7ff3c9d221" />
<img width="1904" height="1078" alt="Screenshot 2026-11-20 230714" src="https://github.com/user-attachments/assets/212af617-6d92-42c9-9220-faabe0ecbb0a" />
<img width="1903" height="1079" alt="Screenshot 2026-11-20 230722" src="https://github.com/user-attachments/assets/ee13359e-d17e-43c0-9679-62e7b4f3d106" />


## The problem

On 10 November 2026 a major multi-vehicle crash happened on the Trakia motorway in dense fog.
Drivers have no easy way to see **where and when** weather makes a trip dangerous before they leave.
RoadSense answers that: pick a start, a destination, a vehicle and a departure time, and see which part of the route is riskiest and when.

## Features

| Page | What it does |
|---|---|
| **Route** | Fastest route (Dijkstra over a road network), a 0–100 risk score per leg for the hour you will actually pass through it, the riskiest leg, and a chart of how the score changes if you leave at a different hour |
| **Weather around me** | Location (GPS, falling back to IP), hourly forecast, live road-weather station data, and a **recommended speed** for your vehicle, road type and surface |
| **Statistics** | Severe-crash statistics by road, a ranking of the most dangerous roads, and a calendar |
| **First aid** | Step-by-step guidance for the first minutes after a serious crash |

## How the risk score works

Each leg gets a score from 0 to 100:

```
score = 0.45 · weather & visibility  +  0.25 · crash history  +  0.20 · incidents & roadworks  +  0.10 · traffic
```

(+ a small bonus for motorcycles, trucks and buses.) The route score is that of the **worst leg**, not the average,
so one foggy section is not hidden by many clear ones.

A rule-based **weather hazard index** (fog, ice, snow, rain, wind, night) is combined with a noisy-OR formula,
so one severe condition is not averaged away. It is used for the explanations shown next to the score.

**Recommended speed** is the lower of (1) the speed whose stopping distance `v·t + v²/(2μg)` fits within the visible distance
and (2) the speed limit reduced by rain, snow, ice, low-visibility and gust factors.

All thresholds and coefficients are expert estimates, not trained values.

## What is real and what is demo data

| Real data | Demo / seed data (hand-made for the prototype) |
|---|---|
| Ministry of Interior crash statistics (aggregated by region, month, hour) | Road network: 28 cities, 38 segments |
| Open-Meteo hourly forecast and archive | Per-segment fog exposure and crash-history weights |
| BGToll road-weather stations (incl. road-surface temperature) | Incidents and roadworks (text in CSV) |
| Road-infrastructure GIS data (roads, bridges, tunnels) | **Traffic factor: a fixed rush-hour rule (55 / 20), not real traffic** |

In Development mode the clock is fixed to **19.11.2026, 07:00** (`DemoNow`) so the morning-fog scenario is visible in the demo.

## Experimental ML.NET model

A Poisson regression (ML.NET) predicts the ratio of observed to expected severe crashes per region and month,
and feeds the "weather & visibility" part of the score.
**It does not beat the naive baseline** (MAE 0.213 vs 0.208, R² −0.046, 168 rows): the open crash data is aggregated
by region, with no location or time for individual crashes. The app flags it as unreliable (`IsReliable: false`).
A real per-segment crash model needs incident-level data (location, date, hour), which is not publicly available.

## Tech stack

ASP.NET Core MVC (.NET 10) · EF Core + SQL Server · ML.NET · Razor views, vanilla JS and CSS
External APIs: Open-Meteo (forecast), BGToll road-weather stations (live data), ipwho.is (location by IP)

## Getting started

Requires the .NET 10 SDK and a SQL Server instance (set `ConnectionStrings:RoadSense` in `appsettings.json`).

```bash
cd src/RoadSense.Web
dotnet run
```

Open http://localhost:5113. On first start the app applies EF migrations and loads empty tables from the CSV files in `dataSource/seed`.

## Project structure

```
src/
  RoadSense.Domain/              models and enums
  RoadSense.Application/         route risk, speed advisor, hazard index, stats, ML abstractions
  RoadSense.Infrastructure.Sql/  EF Core model, migrations, CSV seeding
  RoadSense.Web/                 MVC pages, JSON API, ML.NET predictor, static assets
dataSource/                      seed CSVs, processed public data, GIS, trained model
```

## Data sources and attribution

- Ministry of Interior, severe road-crash statistics, 01.01.2026–30.06.2026 (data.egov.bg)
- Open-Meteo (weather)
- National Toll Administration open data, **CC BY 4.0**: https://bgtoll.bg/otvoreni-danni
- Road-infrastructure GIS data (bridges, tunnels, roads)
- © OpenStreetMap contributors

## Team

| Name | GitHub | Role |
|---|---|---|
| Plamena Dimitrova | [@Plamena998](https://github.com/Plamena998) | Backend and database (SQL Server, EF Core) |
| David Dimitriev | [@daviddimi](https://github.com/daviddimi) | ML.NET risk model, weather hazard index |
| Darina Timoteeva | [@DarinaVT](https://github.com/DarinaVT) | Frontend |
| Madlen Veselinova | [@MadlenIlianova](https://github.com/MadlenIlianova) | Frontend |

## AI assistance

[Keep only if true:] Parts of the code were written with the help of an AI assistant (Claude).
Product idea, data choices, architecture decisions and testing were done by the team.

## Known limitations

- The score is a **weather hazard index, not a crash probability**.
- Traffic, incidents and the road network are demo data; the ML model is experimental and unreliable.
- Crash statistics are regional, so per-segment crash prediction is not possible with the open data.
- Fonts are loaded from Google Fonts.

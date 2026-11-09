# Handyman Workshop

## Om projektet

**Handyman Workshop** er en Full Stack Web App udviklet i C# med Blazor, ASP.NET Core, Entity Framework Core og SQL Server.

Formålet med projektet er at udvikle et system til håndtering af arbejdsordrer på et værksted. Applikationen samler administrationen af kunder, medarbejdere, afdelinger, produkter og arbejdsordrer ét sted.

## Teknologier

- **C#** – Primært programmeringssprog
- **Blazor** – Frontend og brugergrænseflade
- **ASP.NET Core Web API** – Backend og API
- **Entity Framework Core** – ORM og databasehåndtering
- **SQL Server** – Relationsdatabase
- **ASP.NET Core Identity** – Login og brugerhåndtering

## Funktionalitet

Applikationen indeholder følgende datamodeller:

- `Customer` – Kunder
- `Employee` – Medarbejdere
- `Department` – Afdelinger
- `Product` – Produkter
- `WorkOrder` – Arbejdsordrer
- `DepartmentProduct` – Forbindelse mellem afdelinger og produkter
- `WorkOrderProduct` – Forbindelse mellem arbejdsordrer og produkter

Systemet understøtter CRUD-funktionalitet (Create, Read, Update, Delete), så brugerne kan oprette, hente, redigere og slette data.

Derudover indeholder applikationen:

- Filtrering og sortering i tabeller.
- Oversigter over de forskellige datatyper.
- Formularer til oprettelse og redigering.
- Detaljesider for de enkelte poster.
- Sider til sletning af data.
- Styling og en brugervenlig grænseflade.

## Backend

Backend er udviklet med **ASP.NET Core Web API**, som håndterer HTTP-forespørgsler fra frontend og står for kommunikationen med databasen.

**Entity Framework Core** bruges til databasekommunikation med SQL Server gennem `ApplicationDbContext`.

Backend er struktureret med følgende komponenter:

- **Controllers** – Håndterer HTTP-forespørgsler og API-endpoints.
- **DTOs** – Bruges til at overføre data mellem frontend og backend.
- **Mappers** – Konverterer mellem DTO'er og databasemodeller.
- **Interfaces** – Definerer kontrakter for services.
- **Services** – Håndterer forretningslogik og databaseoperationer.
- **ApplicationDbContext** – Håndterer forbindelsen til databasen.

Denne opdeling sikrer en tydelig adskillelse mellem controllerne, forretningslogikken og databasearbejdet.

Et eksempel på denne struktur er `IDepartmentService` og `DepartmentService`.

## Frontend

Frontend er udviklet med **Blazor** og Razor-komponenter.

Klienten bruger `HttpClient` til at kommunikere med backendens Web API og hente eller sende data.

Brugergrænsefladen indeholder:

- Oversigtssider med tabeller.
- Formularer til oprettelse og redigering.
- Detaljesider.
- Sider til sletning.
- Filtrering og sortering.
- Styling på tværs af applikationen.

## Login og sikkerhed

Applikationen bruger **ASP.NET Core Identity** til login og brugerhåndtering.

Der er implementeret følgende brugerroller:

| Rolle | Beskrivelse |
|---|---|
| Admin | Har overordnede systemadministrationsrettigheder. |
| SuperUser | Har almindelige administrationsrettigheder. |
| User | Har adgang til funktioner baseret på brugerens rettigheder. |
| Customer | Har adgang til funktioner baseret på kunderollen. |

Der anvendes rollebaseret adgangskontrol til at begrænse adgangen til funktioner og API-endpoints.

Admin har desuden adgang til en administrationsside, hvor det er muligt at:

- Oprette brugere.
- Slette brugere.
- Nulstille brugeres passwords.

## Database og datamodeller

Databasen er opbygget med **SQL Server**, og Entity Framework Core bruges til at håndtere relationerne mellem datamodellerne.

Projektet indeholder en fælles `BaseEntity`, som indeholder generelle egenskaber:

- `ID`
- `UUID`
- `IsDeleted`
- `DeletedAt`

Der anvendes Data Annotations til validering af data.

Derudover er der implementeret en struktur til **Soft Delete**, hvor data markeres som slettet gennem `IsDeleted` og `DeletedAt` frem for nødvendigvis at blive fjernet permanent fra databasen.

## Test og afgrænsninger

Applikationens funktionalitet og adgangsrettigheder er testet manuelt med de forskellige brugerroller.

Der er på nuværende tidspunkt ikke implementeret automatiserede tests.

Mulige fremtidige forbedringer:

- Implementering af automatiserede tests.
- Yderligere fejlhåndtering.
- Videreudvikling og optimering af service-laget.
- Yderligere forbedringer af brugergrænsefladen.

## Brug af AI

AI er blevet anvendt som et hjælpemiddel under udviklingen af projektet. Det er blandt andet brugt som støtte til problemløsning, sparring og forståelse af forskellige tekniske løsninger.

---

**Projekt:** Handyman Workshop  
**Type:** Full Stack Web Application  
**Sprog:** C#  
**Frontend:** Blazor  
**Backend:** ASP.NET Core Web API  
**Database:** SQL Server

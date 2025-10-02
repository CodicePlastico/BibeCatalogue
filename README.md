# BibeCatalogue - Sistema di Gestione Corsi

Un'applicazione web ASP.NET Core per la gestione dei corsi di formazione con autenticazione utente e database MSSQL.

## Caratteristiche

- 🔐 **Autenticazione**: Login sicuro con email e password
- 📚 **Gestione Corsi**: Visualizzazione e ricerca corsi per titolo con evidenziazione
- � **Ricerca Avanzata**: Filtro real-time per titolo corso con contatori risultati
- �🗄️ **Database MSSQL**: Persistenza dati con Dapper ORM
- 🔄 **Migrazioni Automatiche**: FluentMigrator per la gestione dello schema
- 🐳 **Docker Support**: Containerizzazione completa con Docker Compose
- 🎨 **UI Moderna**: Interfaccia responsive con Bootstrap 5 e icone
- ⚡ **Performance**: Architettura HTML pura senza WebSocket/SignalR

## Struttura del Progetto

```
BibeCatalogue/
├── Models/
│   └── Models.cs                    # Modelli dati (User, Course)
├── Services/
│   ├── DatabaseConnectionService.cs # Gestione connessioni DB
│   ├── UserService.cs              # Logica autenticazione utenti
│   ├── CourseService.cs            # Logica gestione corsi
│   ├── SessionService.cs           # Gestione sessioni utente
│   └── DatabaseMigrationService.cs # Migrazioni database
├── Templates/
│   ├── login.html                  # Template pagina login
│   └── courses.html                # Template dashboard corsi
├── Migrations/
│   └── InitialMigrations.cs        # Schema e dati iniziali
├── wwwroot/
│   ├── app.css                     # Stili personalizzati
│   ├── favicon.ico                 # Icona del sito
│   └── test.html                   # Pagina di test (debug)
├── docker-compose.yml
├── Dockerfile
└── Program.cs                      # Configurazione app e endpoints
```

## Prerequisiti

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) e [Docker Compose](https://docs.docker.com/compose/)
- [SQL Server](https://www.microsoft.com/sql-server) (opzionale se si usa Docker)

## Avvio Rapido con Docker

1. **Clona il repository** (se applicabile):
   ```bash
   git clone <repository-url>
   cd BibeCatalogue
   ```

2. **Avvia l'applicazione con Docker Compose**:
   ```bash
   docker-compose up -d
   ```

3. **Accedi all'applicazione**:
   - URL: http://localhost:8080
   - Email: `test@example.com`
   - Password: `password123`

4. **Arresta l'applicazione**:
   ```bash
   docker-compose down
   ```

## Avvio in Sviluppo

1. **Configura SQL Server**:
   - Avvia un'istanza SQL Server locale sulla porta 1433
   - Oppure usa Docker: `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest`

2. **Aggiorna la stringa di connessione** in `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=BibeCatalogueDB_Dev;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true;"
     }
   }
   ```

3. **Esegui l'applicazione**:
   ```bash
   dotnet run --urls "http://localhost:5000"
   ```

4. **Accedi all'applicazione**:
   - URL: http://localhost:5000
   - Email: `test@example.com`
   - Password: `password123`

5. **Prova le funzionalità**:
   - Dashboard con 4 corsi di esempio
   - Ricerca per "C#", "Database", "Web", "Azure"
   - Logout e re-login

## Architettura del Database

### Tabella Users
- `Id` (int, PK, Identity)
- `Email` (nvarchar(255), Unique)
- `Password` (nvarchar(255))
- `FirstName` (nvarchar(100))
- `LastName` (nvarchar(100))
- `CreatedAt` (datetime)

### Tabella Courses
- `Id` (int, PK, Identity)
- `Title` (nvarchar(200))
- `StartDate` (datetime)
- `EndDate` (datetime)
- `Result` (int, 0-30)
- `UserId` (int, FK -> Users.Id)

## Tecnologie Utilizzate

- **Framework**: ASP.NET Core 8.0 (HTML puro + Server-Side Rendering)
- **Database**: Microsoft SQL Server
- **ORM**: Dapper
- **Migrazioni**: FluentMigrator
- **UI Framework**: Bootstrap 5
- **Icons**: Bootstrap Icons
- **Templating**: HTML Templates con placeholder replacement
- **Containerizzazione**: Docker & Docker Compose
- **Architettura**: REST Endpoints + HTML Forms (nessun JavaScript client-side)

## Funzionalità Principali

### Autenticazione
- **Login elegante** con form HTML e validazione server-side
- **Gestione sessioni** sicura con cookie HttpOnly
- **Redirect automatico** per utenti non autenticati
- **Logout** con pulizia sessione completa

### Dashboard Corsi
- **Visualizzazione lista corsi** dell'utente con layout a griglia
- **Barra di ricerca avanzata** con filtro real-time per titolo
- **Evidenziazione termine cercato** nei risultati (highlight)
- **Contatori risultati** ("Trovati X corsi su Y per 'termine'")
- **Bottone cancella ricerca** per reset rapido
- **Indicatori visivi** per performance (badges colorati verde/rosso)
- **Visualizzazione dettagli** corso (date inizio/fine, voto)

### Database e Performance
- **Migrazioni automatiche** all'avvio applicazione
- **Dati di esempio** pre-caricati per test
- **Query ottimizzate** con Dapper e SQL parametrizzate
- **Gestione connessioni** sicura con using statements
- **Architettura stateless** senza WebSocket/SignalR

## Configurazione

### Variabili di Ambiente
- `ASPNETCORE_ENVIRONMENT`: Ambiente di esecuzione (Development/Production)
- `ConnectionStrings__DefaultConnection`: Stringa di connessione al database

### Configurazione Sessioni
- Timeout: 30 minuti
- Cookie HttpOnly e Essential
- Storage in memoria

## Sicurezza

⚠️ **Nota Importante per la Produzione**:
- Le password sono salvate in chiaro per semplicità del demo
- In produzione implementare hashing sicuro (BCrypt, Argon2, ecc.)
- Implementare HTTPS obbligatorio
- Configurare CORS e CSP appropriatamente
- Usare Azure Key Vault per le stringhe di connessione
- Implementare rate limiting per login
- Aggiungere CSRF protection se necessario

✅ **Sicurezza Attuale**:
- **Nessun problema antiforgery** (architettura semplificata)
- **Sessioni HttpOnly** sicure
- **Connessioni database parametrizzate** (protezione SQL injection)
- **Validazione server-side** completa

## Monitoraggio

### Health Check
- Endpoint: `/health`
- Controlla lo stato dell'applicazione

### Logging
- Console logging in Development
- Configurabile per Production

## Sviluppo Futuro

### Miglioramenti Prioritari
- [ ] **Hashing password sicuro** (BCrypt/Argon2)
- [ ] **Paginazione lista corsi** (per grandi volumi)
- [ ] **CRUD completo corsi** (aggiungi/modifica/elimina)
- [ ] **Filtri avanzati** (per data, voto, stato)
- [ ] **Esportazione dati** (PDF, Excel)

### Funzionalità Aggiuntive
- [ ] **Dashboard statistiche** con grafici
- [ ] **Gestione categorie** corsi
- [ ] **Sistema notifiche** email
- [ ] **API REST** per integrazioni
- [ ] **App mobile** companion
- [ ] **Multitenancy** per organizzazioni

### Aspetti Tecnici
- [ ] **Test unitari** e di integrazione
- [ ] **CI/CD Pipeline** automatizzata
- [ ] **Logging strutturato** (Serilog)
- [ ] **Monitoraggio** (Application Insights)
- [ ] **Cache Redis** per performance
- [ ] **Autenticazione OAuth2/OIDC**

## Troubleshooting

### Errori Comuni

1. **Errore connessione database**:
   - Verificare che SQL Server sia in esecuzione
   - Controllare la stringa di connessione
   - Verificare le credenziali

2. **Errore migrazioni**:
   - Controllare i permessi sul database
   - Verificare la sintassi SQL nelle migrazioni

3. **Pagina bianca o errori 404**:
   - Verificare che la cartella `Templates/` esista
   - Controllare che i file `login.html` e `courses.html` siano presenti
   - Verificare i permessi di lettura sui template

4. **Errori di template**:
   - Verificare che tutti i placeholder `{{VARIABLE}}` siano sostituiti
   - Controllare la sintassi HTML nei template
   - Verificare i log per eccezioni di template rendering

5. **Errore Docker**:
   - Verificare che Docker sia in esecuzione
   - Controllare i log: `docker-compose logs`

6. **Container "bibecatalogue-db" resta in "Waiting" o "Unhealthy"**:
   - SQL Server richiede 1-2 minuti per l'inizializzazione al primo avvio
   - Controllare i log: `docker-compose logs sqlserver`
   - Se l'health check fallisce, verificare che `/opt/mssql-tools18/bin/sqlcmd` sia disponibile

7. **Errore "dependency failed to start: container is unhealthy"**:
   - Il container dell'app web aspetta che il database sia "healthy"
   - Lasciare più tempo per l'avvio del database
   - Se il problema persiste: `docker-compose down && docker-compose up -d`

### Log e Debug
```bash
# Visualizza log dell'applicazione
docker-compose logs web

# Visualizza log del database
docker-compose logs sqlserver

# Esegui in modalità sviluppo con log dettagliati
dotnet run --environment Development --urls "http://localhost:5000"

# Test endpoint health check
curl http://localhost:5000/health

# Pagina di test (solo per debug)
http://localhost:5000/test
```

### Endpoints Disponibili
- `GET /` → Redirect al login
- `GET /login` → Pagina di login
- `POST /login` → Processo di autenticazione
- `GET /courses` → Dashboard corsi (richiede autenticazione)
- `GET /courses?search=termine` → Ricerca corsi
- `GET /logout` → Logout e pulizia sessione
- `GET /health` → Health check applicazione
- `GET /test` → Pagina di test per debug

## Supporto

Per problemi o domande:
1. Controllare i log dell'applicazione
2. Verificare la configurazione del database
3. Consultare la documentazione di Blazor Server

## Licenza

Questo progetto è un esempio educativo.
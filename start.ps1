# Script PowerShell per avviare BibeCatalogue
# Autore: Assistente AI
# Descrizione: Script per avviare facilmente l'applicazione BibeCatalogue

param(
    [Parameter(HelpMessage="Modalità di avvio: docker, dev, build")]
    [ValidateSet("docker", "dev", "build", "stop")]
    [string]$Mode = "docker",
    
    [Parameter(HelpMessage="Forza ricostruzione dei container")]
    [switch]$Rebuild
)

Write-Host "🚀 BibeCatalogue - Sistema di Gestione Corsi" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green

function Test-DockerInstalled {
    try {
        docker --version | Out-Null
        docker-compose --version | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

function Test-DotNetInstalled {
    try {
        dotnet --version | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

function Start-DockerMode {
    Write-Host "🐳 Avvio in modalità Docker..." -ForegroundColor Blue
    
    if (!(Test-DockerInstalled)) {
        Write-Error "❌ Docker non trovato. Installa Docker Desktop per Windows."
        return
    }

    $composeArgs = @("up", "-d")
    
    if ($Rebuild) {
        Write-Host "🔄 Ricostruzione forzata dei container..." -ForegroundColor Yellow
        $composeArgs = @("up", "-d", "--build", "--force-recreate")
    }

    try {
        Write-Host "📦 Avvio dei servizi..." -ForegroundColor Yellow
        docker-compose @composeArgs
        
        Write-Host "⏳ Attesa avvio servizi..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        
        Write-Host "`n✅ Applicazione avviata con successo!" -ForegroundColor Green
        Write-Host "🌐 URL Applicazione: http://localhost:8080" -ForegroundColor Cyan
        Write-Host "📧 Email di test: test@example.com" -ForegroundColor Cyan
        Write-Host "🔑 Password di test: password123" -ForegroundColor Cyan
        Write-Host "`n📊 Per vedere i log: docker-compose logs -f" -ForegroundColor Yellow
        Write-Host "🛑 Per fermare: docker-compose down" -ForegroundColor Yellow
        
        # Apri il browser automaticamente
        Start-Process "http://localhost:8080"
    }
    catch {
        Write-Error "❌ Errore durante l'avvio: $($_.Exception.Message)"
    }
}

function Start-DevMode {
    Write-Host "💻 Avvio in modalità sviluppo..." -ForegroundColor Blue
    
    if (!(Test-DotNetInstalled)) {
        Write-Error "❌ .NET 8.0 SDK non trovato. Installa .NET 8.0 SDK."
        return
    }

    Write-Host "🗄️ Avvio SQL Server con Docker..." -ForegroundColor Yellow
    try {
        docker run -d --name bibecatalogue-dev-db -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
        Start-Sleep -Seconds 10
    }
    catch {
        Write-Warning "⚠️ Impossibile avviare SQL Server. Assicurati che sia in esecuzione sulla porta 1433."
    }

    try {
        Write-Host "🔧 Compilazione progetto..." -ForegroundColor Yellow
        dotnet build
        
        Write-Host "🚀 Avvio applicazione..." -ForegroundColor Yellow
        Write-Host "`n✅ Applicazione in avvio..." -ForegroundColor Green
        Write-Host "🌐 URL Applicazione: https://localhost:5001" -ForegroundColor Cyan
        Write-Host "📧 Email di test: test@example.com" -ForegroundColor Cyan
        Write-Host "🔑 Password di test: password123" -ForegroundColor Cyan
        Write-Host "`n🛑 Premi Ctrl+C per fermare l'applicazione" -ForegroundColor Yellow
        
        dotnet run
    }
    catch {
        Write-Error "❌ Errore durante l'avvio: $($_.Exception.Message)"
    }
}

function Start-BuildMode {
    Write-Host "🔨 Compilazione e test..." -ForegroundColor Blue
    
    if (!(Test-DotNetInstalled)) {
        Write-Error "❌ .NET 8.0 SDK non trovato. Installa .NET 8.0 SDK."
        return
    }

    try {
        Write-Host "📦 Restore dei pacchetti..." -ForegroundColor Yellow
        dotnet restore
        
        Write-Host "🔧 Compilazione..." -ForegroundColor Yellow
        dotnet build --configuration Release
        
        Write-Host "📋 Verifica del progetto..." -ForegroundColor Yellow
        dotnet publish --configuration Release --output ./publish
        
        Write-Host "`n✅ Build completato con successo!" -ForegroundColor Green
        Write-Host "📁 File pubblicati in: ./publish" -ForegroundColor Cyan
    }
    catch {
        Write-Error "❌ Errore durante la compilazione: $($_.Exception.Message)"
    }
}

function Stop-Application {
    Write-Host "🛑 Arresto applicazione..." -ForegroundColor Red
    
    try {
        Write-Host "🐳 Arresto container Docker..." -ForegroundColor Yellow
        docker-compose down
        
        Write-Host "🗑️ Rimozione container di sviluppo..." -ForegroundColor Yellow
        docker stop bibecatalogue-dev-db 2>$null
        docker rm bibecatalogue-dev-db 2>$null
        
        Write-Host "`n✅ Applicazione arrestata!" -ForegroundColor Green
    }
    catch {
        Write-Warning "⚠️ Alcuni servizi potrebbero essere ancora in esecuzione."
    }
}

# Menu principale
switch ($Mode) {
    "docker" { Start-DockerMode }
    "dev" { Start-DevMode }
    "build" { Start-BuildMode }
    "stop" { Stop-Application }
    default { 
        Write-Host "❓ Modalità non riconosciuta. Opzioni disponibili:" -ForegroundColor Yellow
        Write-Host "  - docker: Avvia con Docker Compose (default)" -ForegroundColor White
        Write-Host "  - dev: Avvia in modalità sviluppo" -ForegroundColor White
        Write-Host "  - build: Compila il progetto" -ForegroundColor White
        Write-Host "  - stop: Arresta tutti i servizi" -ForegroundColor White
        Write-Host "`nEsempio: .\start.ps1 -Mode docker" -ForegroundColor Cyan
    }
}
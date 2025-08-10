# Casa do Leão API
API: Responsável por fornecer dados e serviços para o aplicativo e backoffice.

# Comando para criar os Migrations Iniciais
dotnet ef migrations add Inicial --project "..\CDL.Models\CDL.Models.csproj" --startup-project "CDL.Api.csproj" --verbose

# Comando para persistir e criar no banco os Migrations
dotnet ef database update --project "..\CDL.Models\CDL.Models.csproj" --startup-project "CDL.Api.csproj"

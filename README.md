# Casa do Leão API
API: Responsável por fornecer dados e serviços para o aplicativo e backoffice.

## Contrato HTTP (base para o App / front)

- **Prefixo de versão**: `/api/v1/...` (substituir `v1` conforme versionamento).
- **CORS (dev)**: origem `http://localhost:3000` (ver `Program.cs`).
- **Autenticação**: header `Authorization: Bearer {token}` após login. O token inclui claims `uid`, `name`, `login`, `date` e papel (`admin` ou `user`) para autorização por perfil.

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | `/api/v1/core/login` | Anônimo | Login (`LoginRequest` → `LoginResponse` com JWT). |
| GET | `/api/v1/core/login/refresh-token` | Bearer | Renova JWT do usuário autenticado (identidade via claim `uid`; não enviar outro id na query). |
| POST | `/api/v1/core/login/logout/{IdUser}` | Bearer | Encerra sessão; `IdUser` na rota deve coincidir com o `uid` do token. |
| GET | `/api/v1/core/user/get-user` | Bearer | Detalhe usuário por `idUser`. |
| GET | `/api/v1/core/user/list` | Bearer | Lista paginada (`page`, `pageSize`, `search`). |
| POST | `/api/v1/core/user/create-user` | Bearer **admin** | Cria usuário (pode ser admin). |
| POST | `/api/v1/core/user/create-public-user` | Anônimo | Cadastro público (`Admin = false`). |
| PUT | `/api/v1/core/user/update-user` | Bearer | Atualiza usuário. |
| DELETE | `/api/v1/core/user/disable-user` | Bearer | Desativa por `idUser`. |
| GET | `/api/v1/core/event/get-event` | Bearer | Evento por `idEvent` (apenas ativos). |
| GET | `/api/v1/core/event/list` | Bearer | Lista paginada de eventos ativos. |
| GET | `/api/v1/core/event/public-list` | Anônimo | Lista pública de eventos ativos. |
| POST | `/api/v1/core/event/create-event` | Bearer | Cria evento. |
| PUT | `/api/v1/core/event/update-event` | Bearer | Atualiza evento. |
| DELETE | `/api/v1/core/event/disable-event` | Bearer | Desativa evento. |

Respostas usam o envelope `ApiResponse<T>` (`CDL.Models/Api/ApiResponse.cs`), com `HttpCode` no corpo. Em falhas de JWT, a API responde **401** com o mesmo formato de envelope quando o pipeline de autenticação desafia o cliente.

## Serviços (regras de negócio e erros)

- **DatabaseFactory**: cria um novo `DatabaseConnection` por operação (`Activator` + `DbContextOptions`), usado com `using` nos serviços.
- **TokenService**: gera JWT HS256 com issuer/audience/duração de `JwtConfiguration`; exige `Key` não vazia; claims incluem `uid` e `role` (`admin` ou `user`).
- **AuthenticationService**: login compara e-mail + senha cifrada com AES (`APIHelper.EncryptAES` + `APIEnvironment.CypherPass`); erros comuns: `Invalid credentials`, `User not authorized` (usuário inativo), `User not founded` / `User not found` no logout/refresh; persiste `Token` e `LastLogin` no usuário.
- **UserService**: listagem só usuários com `Active != false`; validação de e-mail com regex; e-mail único; criação/atualização de `UserFields` ligados 1:1; `CreatePublicUser` força `Admin = false`; senha sempre gravada cifrada.
- **EventService**: listagens filtram `Active`; busca por título com `Contains`; `GetEvent` só ativos; create/update atualiza `UpdatedAt`; desativação soft (`Active = false`).

## Configuração e segurança

- **Segredos**: não commitar chaves reais. Em desenvolvimento, use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) no projeto da API, por exemplo: `JwtConfiguration:Key`, `JwtConfiguration:Issuer`, `ConnectionStrings:DatabaseConnection`, `APIEnvironment:CypherPass`.
- **HTTPS / JWT**: em produção, use HTTPS; `RequireHttpsMetadata` no JWT fica `true` fora do ambiente **Development**.
- **Primeiro administrador**: o endpoint `create-user` exige papel **admin** no JWT. O primeiro usuário administrador precisa existir no banco (insert manual, script ou seed de migration) antes de usar esse endpoint.

# Comando para criar os Migrations Iniciais
dotnet ef migrations add Inicial --project "..\CDL.Models\CDL.Models.csproj" --startup-project "CDL.Api.csproj" --verbose

# Comando para persistir e criar no banco os Migrations
dotnet ef database update --project "..\CDL.Models\CDL.Models.csproj" --startup-project "CDL.Api.csproj"

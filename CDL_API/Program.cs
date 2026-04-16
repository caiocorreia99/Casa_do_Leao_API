using CDL.Api.Controllers.v1.Interface;
using CDL.Api.Controllers.v1.Models;
using CDL.Api.Controllers.v1.Services;
using CDL.Models.Configuration;
using CDL.Models.DataBase;
using CDL.Models.Services.Interface;
using CDL.Models.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using CDL.Models.Binder;
using CDL.Models.Api;
using CDL.Models.Helpers;
using static CDL.Models.Helpers.Constants;

var MyAllowSpecificOrigins = "AllowLocalhost";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000", "http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(option =>
{
    option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration configuration Sections at service loadersegue m
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(nameof(JwtConfiguration)));
builder.Services.Configure<APIEnvironment>(builder.Configuration.GetSection(nameof(APIEnvironment)));


// Transient instances
builder.Services.AddTransient<IDbAuthService, DbAuthService>();

// Add Database Context Configuration context
builder.Services.AddDbContext<DatabaseConnection>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DatabaseConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DatabaseConnection"))
    )
);

// Add Foundation Services 
builder.Services.AddScoped<IDatabaseFactory<DatabaseConnection>, DatabaseFactory<DatabaseConnection>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IContentService, ContentService>();
builder.Services.AddScoped<ICmsAdminService, CmsAdminService>();
builder.Services.AddScoped<IEventRegistrationService, EventRegistrationService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration[$"{nameof(JwtConfiguration)}:{nameof(JwtConfiguration.Issuer)}"],
        ValidAudience = builder.Configuration[$"{nameof(JwtConfiguration)}:{nameof(JwtConfiguration.Audience)}"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[$"{nameof(JwtConfiguration)}:{nameof(JwtConfiguration.Key)}"]))
    };
    x.Events = new JwtBearerEvents
    {
        OnChallenge = async (context) =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var failure = context.AuthenticateFailure ?? new Exception("Unauthorized");
            await context.HttpContext.Response.WriteAsJsonAsync(
                ApiResponse<Response>.GetErrorResponse(
                    InternalCode.TokenAccess,
                    System.Net.HttpStatusCode.Unauthorized,
                    failure.Message.Contains("expired", StringComparison.OrdinalIgnoreCase) ? "Token expirado." : "Token inválido ou não autorizado.",
                    failure));
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthPolicies.AdminOnly, p => p.RequireRole(UserRoles.Admin));
    options.AddPolicy(AuthPolicies.EditorOrAdmin, p => p.RequireRole(UserRoles.Editor, UserRoles.Admin));
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
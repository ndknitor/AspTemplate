using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using NewTemplate.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.ServiceConfiguration();
builder.Services.AddDbContext<EtdbContext>(o => o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"./keys"));
builder.Services.AddHostedService<PreloadHostedService>();

builder.WebHost.UseKestrel(option => option.AddServerHeader = false).ConfigureKestrel((context, option) =>
{
    // option.Limits.MaxConcurrentConnections = 1;
    // option.Limits.MaxConcurrentUpgradedConnections = 1;
    //option.Limits.MaxResponseBufferSize = 1;
    if (context.HostingEnvironment.IsProduction())
    {
        option.ConfigureHttpsDefaults(o =>
        {
            o.AllowAnyClientCertificate();
            o.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("certificate.pfx", "123456", System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet);
        });
    }
});
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddRouting(o =>
{
    o.ConstraintMap.Add("ulong", typeof(ULongRouteConstraint));
    o.ConstraintMap.Add("uint", typeof(UIntRouteConstraint));
});
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(policy =>
    {
        policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>());
    });
});

builder.Services.AddScoped((ins) => ConnectionMultiplexer.Connect("localhost").GetDatabase());
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = "localhost:6379"; // Change this to your Redis server connection
//     options.InstanceName = "SampleApp";
// });

int expireHours = int.Parse(builder.Configuration["AuthenticationExpireHours"]);
#region Combine with JWT and Cookie
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultScheme = "JWT_OR_COOKIE";
//     options.DefaultChallengeScheme = "JWT_OR_COOKIE";
// }).AddCookie("Cookies", options =>
// {
//     options.Events.OnRedirectToAccessDenied = context =>
//     {
//         context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//         return Task.CompletedTask;
//     };
//     options.Events.OnRedirectToLogin = context =>
//     {
//         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//         return Task.CompletedTask;
//     };
//     options.ExpireTimeSpan = TimeSpan.FromHours(expireHours);
// }).AddJwtBearer("Bearer", options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidIssuer = builder.Configuration["JwtProvider:Issuer"],
//         ValidateAudience = true,
//         ValidAudience = builder.Configuration["JwtProvider:Audience"],
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtProvider:SecretKey"])),
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.FromHours(expireHours)
//     };
// }).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
// {
//     options.ForwardDefaultSelector = context =>
//     {
//         string authorization = context.Request.Headers["Authorization"];
//         if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
//             return "Bearer";
//         return "Cookies";
//     };
// });
#endregion

#region Cookie
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
// {
//     options.Events.OnRedirectToAccessDenied = context =>
//     {
//         context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//         return Task.CompletedTask;
//     };
//     options.Events.OnRedirectToLogin = context =>
//     {
//         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//         return Task.CompletedTask;
//     };
//     options.ExpireTimeSpan = TimeSpan.FromHours(expireHours);
//     options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"./keys"));
// });
#endregion

#region JWT
// builder.Services.AddAuthentication(o =>
// {
//     o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(o =>
// {
//     o.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidIssuer = builder.Configuration["JwtProvider:Issuer"],
//         ValidAudience = builder.Configuration["JwtProvider:Audience"],
//         IssuerSigningKey = new SymmetricSecurityKey
//         (Encoding.UTF8.GetBytes(builder.Configuration["JwtProvider:Key"])),
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.FromMinutes(expireHours),
//         ValidateIssuerSigningKey = true
//     };
// });
#endregion

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerExcludeFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Description = "Please insert JWT token into field"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
    });
});

if (builder.Environment.IsProduction())
{
    // builder.Logging.ClearProviders();
    // Log.Logger =
    // new LoggerConfiguration()
    // .MinimumLevel.Verbose()
    // .Filter.ByExcluding(e =>
    // {
    //     var excludedNamespaces = new List<string>
    //     {
    //         "Microsoft.AspNetCore",
    //         "Microsoft.EntityFrameworkCore"
    //     };
    //     if (e.Properties.TryGetValue("SourceContext", out var sourceContextProperty) &&
    //             sourceContextProperty is ScalarValue sourceContextValue)
    //     {
    //         var sourceContext = sourceContextValue.Value.ToString();
    //         return excludedNamespaces.Any(excludedNamespace => sourceContext.StartsWith(excludedNamespace));
    //     }

    //     return false;
    // })
    // .WriteTo.Logger(c =>
    //     c.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information || e.Level == LogEventLevel.Warning)
    //         .WriteTo.RollingFile($"./logs/info.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"))
    // .WriteTo.Logger(c =>
    //     c.Filter.ByIncludingOnly(e => e.Level >= LogEventLevel.Error)
    //         .WriteTo.RollingFile($"./logs/error.log", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"))
    // .CreateLogger();
    // builder.Logging.AddSerilog(Log.Logger);
}

var app = builder.Build();

// app.Use(async (context, next) =>
// {
//     bool slow = Random.Shared.NextDouble() <= 0.1;
//     await Task.Delay(Random.Shared.Next(500, 1000) * Random.Shared.Next(1, 5) * (slow ? Random.Shared.Next(2, 5) : 1));
//     await next();
// });

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsProduction())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
    app.UseHsts();
    app.UseHttpsRedirection();
    app.UseMiddleware<LoggingMiddleware>();
}
app.UseStaticFiles();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

//dotnet ef dbcontext scaffold "Data Source=127.0.0.1;Initial Catalog=etdb;User ID=sa;Password=QU3yYwn9Jirkk5FX7yi1uZeK06H6iR89OaV5QAbH0nmbqXsx"  Microsoft.EntityFrameworkCore.SqlServer -f --no-pluralize --no-onconfiguring -o Context
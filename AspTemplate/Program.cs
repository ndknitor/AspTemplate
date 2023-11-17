using System.Text.Json.Serialization;
using AspTemplate.Context;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsProduction())
{
    builder.Logging.ClearProviders();
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration.GetSection("Logging")).CreateLogger();
    builder.Logging.AddSerilog(Log.Logger);
}

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
builder.Services.AddRouting(o =>
{
    o.ConstraintMap.Add("ulong", typeof(ULongRouteConstraint));
    o.ConstraintMap.Add("uint", typeof(UIntRouteConstraint));
});
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
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"./keys"));

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

builder.Services.ServiceConfiguration();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<EtdbContext>(o => o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(builder.Configuration.GetConnectionString("Default")));
//builder.Services.AddScoped((ins) => ConnectionMultiplexer.Connect("localhost").GetDatabase());
builder.Services.AddHostedService<PreloadHostedService>();
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
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

//dotnet ef dbcontext scaffold "Data Source=127.0.0.1;TrustServerCertificate=True;Initial Catalog=etdb;User ID=sa;Password=QU3yYwn9Jirkk5FX7yi1uZeK06H6iR89OaV5QAbH0nmbqXsx"  Microsoft.EntityFrameworkCore.SqlServer -f --no-pluralize --no-onconfiguring -o Context
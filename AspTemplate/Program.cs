using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
//using AspTemplate.Context;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using Serilog;

var builder = WebApplication.CreateBuilder();//CreateEmptyBuilder(new WebApplicationOptions { Args = args, ApplicationName = "AspTemplate", WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), ContentRootPath = Directory.GetCurrentDirectory(), EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") });

if (builder.Environment.IsProduction())
{
    // builder.Logging.ClearProviders();
    // Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration.GetSection("Logging")).CreateLogger();
    // builder.Logging.AddSerilog(Log.Logger);
    builder.Services.AddOpenTelemetry().WithMetrics(m =>
    {
        m.AddPrometheusExporter();
        m.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
        m.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
            });
    });
}
else
{
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
        c.EnableAnnotations();
    });
    builder.Services.AddEndpointsApiExplorer();
}

builder.WebHost.UseKestrel(option => option.AddServerHeader = false).ConfigureKestrel((context, option) =>
{
    // Limit request query
    option.Limits.MaxRequestLineSize = 1024;

    // option.Limits.MaxConcurrentConnections = 1;
    // option.Limits.MaxConcurrentUpgradedConnections = 1;
    // option.Limits.MaxResponseBufferSize = 1;
    // if (context.HostingEnvironment.IsProduction())
    // {
    //     option.ConfigureHttpsDefaults(o =>
    //     {
    //         o.AllowAnyClientCertificate();
    //         o.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("certificate.pfx", "123456", System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet);
    //     });
    // }

    /////////////       Create pfx file command     ///////////////////////////

    /// Create self signed certificate
    /// openssl req -x509 -nodes -newkey rsa:2048 -keyout private-key.pem -out certificate.pem -days 365

    /// Create csr
    /// openssl req -new -newkey rsa:2048 -nodes -keyout private-key.pem -out certificate.csr

    /// Convert pfx
    /// openssl pkcs12 -export -out certificate.pfx -inkey private-key.pem -in certificate.pem
    /// openssl pkcs12 -export -out certificate.pfx -inkey private-key.pem -in certificate.pem -passout pass:yourpassphrase

    //openssl pkcs12 -export -out certificate.p12 -inkey key.pem -in cert.pem
    //openssl pkcs12 -in certificate.p12 -out certificate.pfx
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
        .AllowCredentials()
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>());
    });
});
builder.Services.AddRouting(o =>
{
    o.ConstraintMap.Add("ulong", typeof(ULongRouteConstraint));
    o.ConstraintMap.Add("uint", typeof(UIntRouteConstraint));
});
builder.Services.Configure<ApiBehaviorOptions>(o =>
{
    o.ClientErrorMapping[StatusCodes.Status415UnsupportedMediaType] = new ClientErrorData();
    o.InvalidModelStateResponseFactory = actionContext =>
    {
        Dictionary<string, string[]> error = new();
        foreach (string item in actionContext.ModelState.Keys)
        {
            if (!string.IsNullOrEmpty(item))
            {
                error[item[0].ToString().ToLower() + item.Remove(0, 1)] = actionContext.ModelState[item].Errors.Select(e => e.ErrorMessage).ToArray();
            }
        }
        return new BadRequestObjectResult(error);
    };
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR(options =>
{
    options.AddFilter<SignalRExceptionFilter>();
});

builder.Services.Configure<ExampleOption>(builder.Configuration.GetSection("Example"));

int expireHours = int.Parse(builder.Configuration["AuthenticationExpireHours"]);
#region Combine with JWT and Cookie
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "JWT_OR_COOKIE";
    options.DefaultChallengeScheme = "JWT_OR_COOKIE";
}).AddCookie("Cookies", options =>
{
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Cookie.Name = "auth";
    options.ExpireTimeSpan = TimeSpan.FromHours(expireHours);
}).AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtProvider:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtProvider:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["JwtProvider:SecretKey"])),
        ValidateLifetime = true
    };
}).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        string authorization = context.Request.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            return "Bearer";
        return "Cookies";
    };
});
#endregion

#region Cookie
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
// {
//     options.Events.OnRedirectToAccessDenied = context =>
//     {
//         context.Response.StatusCode = StatusCodes.Status403Forbidden;
//         return Task.CompletedTask;
//     };
//     options.Events.OnRedirectToLogin = context =>
//     {
//         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//         return Task.CompletedTask;
//     };
//     options.Cookie.Name = "cookie";
//     options.ExpireTimeSpan = TimeSpan.FromHours(expireHours);
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
//         (Convert.FromBase64String(builder.Configuration["JwtProvider:Key"])),
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ClockSkew = TimeSpan.FromMinutes(expireHours),
//         ValidateIssuerSigningKey = true
//     };
// });
#endregion

builder.Services.AddAuthorization(options =>
{
    foreach (UserPolicy userPolicy in Enum.GetValues(typeof(UserPolicy)))
    {
        options.AddPolicy(userPolicy.ToString(), policy =>
            policy.RequireClaim(nameof(UserPolicy), userPolicy.ToString()));
    }
});

//builder.Services.AddDbContext<EtdbContext>(o => o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking).UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
//builder.Services.AddHostedService<PreloadHostedService>();
builder.Services.ServiceConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsProduction())
// {
//     //app.UseMiddleware<LoggingMiddleware>();
//     app.UseForwardedHeaders(new ForwardedHeadersOptions
//     {
//         ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
//     });
//     app.UseHsts();
//     app.UseHttpsRedirection();
// }
// else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<SignalRHub>("/signalr");
app.MapPrometheusScrapingEndpoint();

app.Run();
//dotnet ef dbcontext scaffold "Data Source=127.0.0.1;TrustServerCertificate=True;Initial Catalog=etdb;User ID=sa;Password=password"  Microsoft.EntityFrameworkCore.SqlServer -f --no-pluralize --no-onconfiguring -o Context
using DUY.API.Entities;
using C.Tracking.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using C.Tracking.API.IRepositories;
using Microsoft.AspNetCore.Http.Features;
using C.Tracking.Extensions;
using DUY.API.IRepositories;
using DUY.API.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var contexOptions = new DbContextOptionsBuilder<ApplicationContext>().UseSqlServer(connectionString).Options;
builder.Services.AddDbContext<ApplicationContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddTransient<IContractFileRepository, ContractFileRepository>();
builder.Services.AddTransient<IComMentRepository, ComMentRepository>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<ISongRepository, SongRepository>();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
string issuer = builder.Configuration["TokenSettings:Issuer"].ToString();

string audience = builder.Configuration["TokenSettings:Audience"].ToString();
string key = builder.Configuration["TokenSettings:Key"].ToString();
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidAudience = audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials());
});
builder.Services.AddControllersWithViews().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = 10485760;//10MB
    o.MemoryBufferThreshold = int.MaxValue;
});
var app = builder.Build();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["SyncfusionKey"]);
app.UseStaticFiles();
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    dataContext.Database.Migrate();
}
app.MapControllers();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("CorsPolicy");
app.Run();

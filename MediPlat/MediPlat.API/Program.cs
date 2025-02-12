using System.Text.Json;
using System.Text.Json.Serialization;
using MediPlat.Model;
using MediPlat.Repository.IRepositories;
using MediPlat.Repository.Repositories;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.AspNetCore.OData.Extensions;
using System.Text;
using Microsoft.AspNetCore.OData;
using MediPlat.Model.Model;
using MediPlat.Service.Mapping;
using Microsoft.OData.ModelBuilder;
using MediPlat.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Register Services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    })
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Count().Expand().SetMaxTop(100).AddRouteComponents("odata", GetEdmModel());
    });

// Add DbContext 
//builder.Services.AddDbContext<MediPlatContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
//});
builder.Services.AddDbContext<MediPlatContext>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", // S? d?ng ?úng schema
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" // S? d?ng ?úng schema cho role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims;
            foreach (var claim in claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DoctorPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Doctor"));
    options.AddPolicy("PatientPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Patient"));
});

// Build the app
var app = builder.Build();

// Configure CORS
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Hiển thị lỗi chi tiết
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Ensure the correct order of middlewares
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>(); // Move it here
app.MapControllers();
app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    // Định nghĩa các thực thể và tập thực thể
    builder.EntitySet<DoctorSubscription>("DoctorSubscriptions");
    builder.EntitySet<Patient>("Patients");
    builder.EntitySet<Doctor>("Doctors");
    builder.EntitySet<AppointmentSlot>("AppointmentSlots");
    builder.EntitySet<Review>("Reviews");
    builder.EntitySet<Transaction>("Transactions");

    // Định nghĩa các mối quan hệ nếu cần thiết
    // builder.EntitySet<EntityName>("EntitySetName");
    builder.EntityType<Patient>()
        .HasMany(p => p.AppointmentSlots);
    builder.EntityType<Patient>()
        .HasMany(p => p.Reviews);
    builder.EntityType<Patient>()
        .HasMany(p => p.Transactions);

    return builder.GetEdmModel();
}

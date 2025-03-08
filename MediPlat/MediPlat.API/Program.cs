using MediPlat.Repository.IRepositories;
using MediPlat.Repository.Repositories;
using MediPlat.Service.IServices;
using MediPlat.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using System.Text;
using Microsoft.AspNetCore.OData;
using MediPlat.Model.Model;
using MediPlat.Service.Mapping;
using Microsoft.OData.ModelBuilder;
using MediPlat.API.Middleware;
using MediPlat.Model.ResponseObject;
using Microsoft.EntityFrameworkCore;
using MediPlat.Model.ResponseObject.Patient;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Đăng ký các service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IDoctorSubscriptionService, DoctorSubscriptionService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<IAppointmentSlotMedicineService, AppointmentSlotMedicineService>();
builder.Services.AddScoped<IMedicineService, MedicineService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IMediPlatService, MediPlatService>();

// Đăng ký Repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Đăng ký Hosted Service
builder.Services.AddHostedService<DoctorSubscriptionCleanupService>();

//Register RazorPage
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    })
    .AddOData(options =>
    {
        options.Select().Filter().OrderBy().Count().Expand().SetMaxTop(100);
        options.AddRouteComponents("odata", GetEdmModel());
    });

// Add DbContext 
builder.Services.AddDbContext<MediPlatContext>(/*options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DB");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("⚠️ ConnectionString is missing in appsettings.json!");
    }

    options.UseSqlServer(connectionString);
}*/);

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
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
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
        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
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
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
    options.AddPolicy("PatientPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Patient"));
    options.AddPolicy("DoctorOrAdminPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                (c.Value == "Doctor" || c.Value == "Admin")))));
    options.AddPolicy("DoctorOrpatientPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                (c.Value == "Doctor" || c.Value == "Patient")))));
    options.AddPolicy("DoctorOrAdminorPatientPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                (c.Value == "Doctor" || c.Value == "Admin" || c.Value == "Patient")))));
    options.AddPolicy("PatientPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Patient"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.AddLogging();
// Build the app
var app = builder.Build();

// Middleware
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Hiển thị lỗi chi tiết
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();
app.Run();

static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();

    builder.EntitySet<AppointmentSlotResponse>("AppointmentSlots");
    builder.EntitySet<ReviewResponse>("Reviews");
    builder.EntitySet<TransactionResponse>("Transactions");
    builder.EntitySet<MedicineResponse>("Medicines");
    builder.EntitySet<AppointmentSlotMedicineResponse>("AppointmentSlotMedicines");
    builder.EntitySet<DoctorSubscriptionResponse>("DoctorSubscriptions");
    builder.EntitySet<ExperienceResponse>("Experiences");
    builder.EntitySet<DoctorResponse>("Doctors");
    builder.EntitySet<SpecialtyResponse>("Specialties");
    builder.EntitySet<SubscriptionResponse>("Subscriptions");
    builder.EntitySet<ProfileResponse>("Profiles");
    builder.EntitySet<PatientResponse>("Patients");
    builder.EntitySet<ServiceResponse>("Services");

    // Định nghĩa các mối quan hệ nếu cần thiết
    // builder.EntitySet<EntityName>("EntitySetName");
    builder.EntityType<Patient>()
        .HasMany(p => p.Transactions);

    return builder.GetEdmModel();
}

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Protocol: {context.Request.Protocol}");
    await next();
});

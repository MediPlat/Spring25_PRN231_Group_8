using MediPlat.Model.Model;
using MediPlat.Service.Mapping;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Cấu hình HttpClient cho API Backend
builder.Services.AddHttpClient("UntrustedClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Đăng ký DbContext
builder.Services.AddDbContext<MediPlatContext>();

// Cấu hình Authentication với CookieAuth + JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddJwtBearer(options =>
{
    options.Authority = "https://localhost:7002";
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                Console.WriteLine("🔹 Token found in Authorization Header.");
            }
            else if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                Console.WriteLine("🔹 Token found in Cookie.");
                var token = context.Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length);
                }
                context.Token = token;
            }
            else
            {
                Console.WriteLine("⚠️ No token found!");
            }

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            var claims = context.Principal?.Claims;
            Console.WriteLine("✅ Token validated successfully:");
            foreach (var claim in claims)
            {
                Console.WriteLine($" - {claim.Type}: {claim.Value}");
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
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") &&
                (c.Value == "Doctor" || c.Value == "Admin"))));
    options.AddPolicy("DoctorOrAdminorPatientPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role") &&
                (c.Value == "Doctor" || c.Value == "Admin" || c.Value == "Patient"))));
});

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();

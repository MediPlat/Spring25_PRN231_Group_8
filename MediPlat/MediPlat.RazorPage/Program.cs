using MediPlat.Model.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddHttpClient();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken"))
                {
                    var token = context.Request.Cookies["AuthToken"];
                    if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
                    {
                        token = token.Substring("Bearer ".Length);
                    }
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
        options.Authority = "https://localhost:7002";
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
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DoctorPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Doctor"));
    options.AddPolicy("AdminPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
    options.AddPolicy("patientPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
    options.AddPolicy("DoctorOrAdminPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                (c.Value == "Doctor" || c.Value == "Admin")))));
    options.AddPolicy("DoctorOrAdminorPatientPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" &&
                (c.Value == "Doctor" || c.Value == "Admin" || c.Value == "Patient")))));
});

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
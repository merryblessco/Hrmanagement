using AutoMapper;
using HRbackend.Data;
using HRbackend.Lib;
using HRbackend.Models.Auth;
using HRbackend.Models.Helper;
using HRbackend.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Add this using directive
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR Management", Version = "v1" });

    // Add JWT Authorization header to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    //// Add this to prepend "Bearer " automatically
    //c.OperationFilter<SwaggerTokenFilter>();
});

// Register HttpClientService
builder.Services.AddHttpClient<HttpClientService>();

// Register Notifications
builder.Services.AddSingleton<Notifications>();

// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HrDBCon")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero // Ensure exact expiration without extra time buffer

        };
    });

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
             .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var seeder = new DatabaseSeeder(userManager, roleManager);
    var statesAndLgaSeeder = new StateAndLgaSeeder(dbContext);

    await seeder.SeedAsync();
    await statesAndLgaSeeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();
}

app.UseHttpsRedirection();
app.UseRouting();  // Add this if missing
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

using EmployeeTaskManager.API.Data;
using EmployeeTaskManager.API.Services.Implementations;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EmployeeTaskManager.API.Config;
using EmployeeTaskManager.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging    

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    // Logs to console (good for local debugging)
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Logs to file, 1 file per day
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // 👈 Tells .NET to use Serilog

builder.Services.AddControllers();
// Swagger, DbContext, Services, etc. → Keep as they are...

//var app = builder.Build();



// Bind JWT settings from config
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

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

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

// Add services to the container.

// Add services to the container.
builder.Services.AddControllers();

// Register DbContext (already present, just confirming)
//builder.Services.AddDbContext<EmployeeTaskManagerContext>();// Uncomment if you want to use in-memory database for testing by Mahesh Kakad    

builder.Services.AddDbContext<EmployeeTaskManagerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// ✅ Register your Services here
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<JwtTokenService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//Added Middleware for Exception Handling by Mahesh Kakad
app.UseMiddleware<EmployeeTaskManager.API.Middlewares.ExceptionMiddleware>();

app.UseSerilogRequestLogging(); // ✅ Logs each HTTP Request

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

using EmployeeTaskManager.API.Data;
using EmployeeTaskManager.API.Services.Implementations;
using EmployeeTaskManager.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapControllers();

app.Run();

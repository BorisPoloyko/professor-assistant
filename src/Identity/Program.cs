using Identity.DataAccess.Context;
using Identity.DataAccess.Implementations;
using Identity.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
builder.Services.AddTransient<IUniversityGroupsRepository, UniversityGroupsRepository>();
builder.Services.AddTransient<IStudentsRepository, StudentRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

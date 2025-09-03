using System;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Domain.Entities; // para Producto en el seed
// using Application.UseCases.Pedidos; // no obligatorio aquí si registras por fullname

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Repositorios
builder.Services.AddScoped<Domain.Interfaces.IProductoRepository, Infrastructure.Repositories.SqlProductoRepository>();
builder.Services.AddScoped<Domain.Interfaces.IPedidoRepository, Infrastructure.Repositories.SqlPedidoRepository>();


// Casos de uso / Handlers
builder.Services.AddScoped<Application.UseCases.Pedidos.CreatePedidoHandler>();

var app = builder.Build();

// Swagger (dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

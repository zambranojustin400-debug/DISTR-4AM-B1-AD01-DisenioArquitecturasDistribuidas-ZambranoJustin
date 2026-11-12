
using Pedidos.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregar controladores
builder.Services.AddControllers();

// Conexion con SQL Server
builder.Services.AddDbContext<PedidosDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PedidosConnection")
    )
);

// Configuracion Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Activar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using APIMiri.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Se agregó la siguiente linea para la configuración de la cadena de conexión..................
builder.Services.AddDbContext<DbMiriContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexionstring")));
//..............................................

//Agregar politica CORS SOLO EN API WEB, en aplicación cliente no se agrega nada
builder.Services.AddCors(p => p.AddPolicy("CorsPolicy", builder =>
{

    builder.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin();
    
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app cors
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
//app.UseCors(prodCorsPolicy)

app.MapControllers();

app.Run();

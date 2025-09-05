using System.Text.Json.Serialization;
using AutoMapper;
using MessageApi.Data;
using MessageApi.Mappings;
using MessageApi.Repositories;
using MessageApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200") // URL de tu aplicación Angular
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

// Servicios
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPersonService, PersonService>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configuración de controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configuración de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });
});

// CORS Configuration - Permitir solicitudes desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL del frontend Angular
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Habilitar CORS
app.UseCors(MyAllowSpecificOrigins);

// Configuración de la canalización de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthorization();

// Mapeo de controladores y rutas
app.MapControllers();

// Ruta de bienvenida
app.MapGet("/", () => "Order Management API is running!");

// Middleware de manejo de errores personalizado
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            await context.Response.WriteAsJsonAsync(new 
            {
                StatusCode = context.Response.StatusCode,
                Message = "Error interno del servidor",
                Detail = contextFeature.Error.Message,
                StackTrace = app.Environment.IsDevelopment() ? contextFeature.Error.StackTrace : null
            });
        }
    });
});

app.Run();

using CategoryService.Data;
using CategoryService.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

var cosmosSettings = builder.Configuration.GetSection("CosmosDb");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseCosmos(
        cosmosSettings["EndpointUri"]!,
        cosmosSettings["PrimaryKey"]!,
        cosmosSettings["DatabaseName"]!,
        cosmosOptions =>
        {
            // 1. Forzar a usar SOLO localhost y no la IP interna de Docker
            cosmosOptions.LimitToEndpoint(true);
            
            // 2. Usar el modo Gateway, ideal para evitar bloqueos de puertos en local
            cosmosOptions.ConnectionMode(Microsoft.Azure.Cosmos.ConnectionMode.Gateway);
            // Esta configuración le dice a .NET que ignore el error de SSL del emulador local
            cosmosOptions.HttpClientFactory(() =>
            {
                var httpMessageHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                return new HttpClient(httpMessageHandler);
            });
        }
    ));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Crear la base de datos y contenedores automáticamente si no existen
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // EnsureCreated le dice a Cosmos DB: "Si no existe CategoryDb o Categories, créalos ahora"
    dbContext.Database.EnsureCreated(); 
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

using Microsoft.Extensions.Options;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.Repository;
using SmartGoals.CosmosDB.StoreAPI.SmartGoals.CosmosDB.StoreServices;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddSingleton<ICosmosDbClientService, CosmosDbClientService>();
//builder.Services.AddScoped(typeof(ICosmosDbGenericRepository<>), typeof(CosmosDbGenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "SmartGoals CosmosDB DEMO API",
            Version = "v1"
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        options.OrderActionsBy((apiDesc) => $"{apiDesc.HttpMethod}");
        options.IgnoreObsoleteActions();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Goals Store Api");
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using ClientGateway;
using ClientGateway.Controllers;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ClientGateway.Domain; // Add this using directive for the ClientGateway.Domain namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind Kafka configuration to ProducerConfig
var kafkaConfig = new ProducerConfig();
builder.Configuration.GetSection("Kafka").Bind(kafkaConfig);
builder.Services.AddSingleton(kafkaConfig);

// Register IProducer<string, Biometrics> using configured ProducerConfig
builder.Services.AddSingleton<IProducer<string, Biometrics>>(sp =>
{
    var config = sp.GetRequiredService<ProducerConfig>();
    var schemaRegistryClient = sp.GetRequiredService<ISchemaRegistryClient>();

    // Register JsonSerializer for Biometrics class
    var serializer = new JsonSerializer<Biometrics>(schemaRegistryClient);

    return new ProducerBuilder<string, Biometrics>(config)
        .SetValueSerializer(serializer)
        .Build();
});


// Load SchemaRegistry section of the config into a SchemaRegistryConfig object
var schemaRegistryConfig = new SchemaRegistryConfig();
builder.Configuration.GetSection("SchemaRegistry").Bind(schemaRegistryConfig);
builder.Services.AddSingleton(schemaRegistryConfig);

// Register an instance of ISchemaRegistryClient using a new CachedSchemaRegistryClient
builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<SchemaRegistryConfig>();
    return new CachedSchemaRegistryClient(config);
});
//builder.Services.AddCors(corsOptions =>
//{
//    corsOptions.AddPolicy("MyPolicy", CorsPolicyBuilder =>
//    {
//        // certain one
//        //CorsPolicyBuilder.WithOrigins("http://www.face.com");

//        // anyone have url  i can response to him
//        //CorsPolicyBuilder.AllowAnyOrigin();

//        //with certain meyhods 
//        // get , post   --put them in array of string  and replace
//        //CorsPolicyBuilder.AllowAnyOrigin().WithMethods("Get");

//        // any method 
//        //CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod();

//        // don’t  need a certain header
//        // anyone can access
//        CorsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();


//    });
//});

// Configure CORS policy
builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Allow requests from this origin
               .AllowAnyMethod() // Allow any HTTP method (GET, POST, PUT, etc.)
               .AllowAnyHeader(); // Allow any HTTP header
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Use CORS middleware
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();

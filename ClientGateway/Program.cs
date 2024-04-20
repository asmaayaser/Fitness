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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

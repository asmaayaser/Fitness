using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Confluent.Kafka.SyncOverAsync;
using HeartRateZoneService.Domain;
using HeartRateZoneService.Workers;
using System.Text;
//using Confluent.Kafka.Serialization;

////using Confluent.Kafka.Avro;
    



var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<HeartRateZoneWorker>();

        // Add a using directive for HeartRateZoneService.Domain namespace
        services.AddSingleton<ISchemaRegistryClient>(sp =>
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "https://pkc-12576z.us-west2.gcp.confluent.cloud:443", // Replace with actual Schema Registry URL
                BasicAuthUserInfo = "VM5LSG2OSIHZSNA4:oKmb16+TPuqywKdhoTxiSD9cTzf7dh71zoKGA/iIHGu3xCEZYcTu57OywxmHz8h6", // Replace with actual Schema Registry API Key
            };
            return new CachedSchemaRegistryClient(schemaRegistryConfig);
        });

        // Add a using directive for HeartRateZoneService.Domain namespace
        services.AddSingleton<IProducer<string, HeartRateZoneReached>>(sp =>
        {
            var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = "pkc-12576z.us-west2.gcp.confluent.cloud:9092", // Replace with actual Kafka bootstrap servers
                ClientId = "HeartRateZoneProducer",
                TransactionalId = "HeartRateZoneProducer",
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "P3VG5SXRGZDEMC2K",
                SaslPassword = "TNOpyhpdoUICR9d3uugJYgLQfS2kMAWh/LkFXFnKMaRDgbaad6OelKH1vWGhz6Jz",
                EnableIdempotence = true
            };

            //var builder = new ProducerBuilder<string, HeartRateZoneReached>(producerConfig)
            //    .SetKeySerializer(new StringSerializer(Encoding.UTF8))
            //    .SetValueSerializer(new AvroSerializer<HeartRateZoneReached>(schemaRegistry).AsSyncOverAsync());

            var builder = new ProducerBuilder<string, HeartRateZoneReached>(producerConfig)
                .SetValueSerializer(new JsonSerializer<HeartRateZoneReached>(schemaRegistry))
                .SetKeySerializer(new JsonSerializer<string>(schemaRegistry));

            return builder.Build();
        });

        services.AddSingleton<IConsumer<string, Biometrics>>(sp =>
        {
            // Load a ConsumerConfig from the configuration
            var consumerConfig = new ConsumerConfig();
            hostContext.Configuration.GetSection("Consumer").Bind(consumerConfig);

            // Create a new ConsumerBuilder with the specified key and value deserializers
            var builder = new ConsumerBuilder<string, Biometrics>(consumerConfig)
                .SetValueDeserializer(new JsonDeserializer<Biometrics>().AsSyncOverAsync());

            // Build the consumer and return it
            return builder.Build();
        });
    })
    .Build();

await host.RunAsync();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

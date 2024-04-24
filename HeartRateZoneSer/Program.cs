using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Confluent.Kafka.SyncOverAsync;
using HeartRateZoneSer.Domain;
using HeartRateZoneSer.Workers;
using System.Text;
namespace HeartRateZoneSer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<HeartRateZoneWorker>();

            // Add a using directive for HeartRateZoneService.Domain namespace
            builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
            {
                var schemaRegistryConfig = new SchemaRegistryConfig
                {
                    Url = "https://pkc-12576z.us-west2.gcp.confluent.cloud:443", // Replace with actual Schema Registry URL
                    BasicAuthUserInfo = "VM5LSG2OSIHZSNA4:oKmb16+TPuqywKdhoTxiSD9cTzf7dh71zoKGA/iIHGu3xCEZYcTu57OywxmHz8h6", // Replace with actual Schema Registry API Key
                };
                return new CachedSchemaRegistryClient(schemaRegistryConfig);
            });

            // Add a using directive for HeartRateZoneService.Domain namespace
            builder.Services.AddSingleton<IProducer<string, HeartRateZoneReached>>(sp =>
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

                var producerBuilder = new ProducerBuilder<string, HeartRateZoneReached>(producerConfig)
                    .SetValueSerializer(new JsonSerializer<HeartRateZoneReached>(schemaRegistry))
                    .SetKeySerializer(new JsonSerializer<string>(schemaRegistry));

                return producerBuilder.Build();
            });

            builder.Services.AddSingleton<IConsumer<string, Biometrics>>(sp =>
            {
                var consumerConfig = new ConsumerConfig();
                builder.Configuration.GetSection("Consumer").Bind(consumerConfig);

                var consumerBuilder = new ConsumerBuilder<string, Biometrics>(consumerConfig)
                    .SetValueDeserializer(new JsonDeserializer<Biometrics>().AsSyncOverAsync());

                return consumerBuilder.Build();
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
            // Register HeartRateZoneWorker service
            builder.Services.AddSingleton<HeartRateZoneWorker>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowSpecificOrigin");

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}

using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.WebSocket4Net;
using MQTTnet.Extensions.ManagedClient;
using System.Threading;
using Npgsql;
using NLog;
using Microsoft.Extensions.Configuration;
using MqttToTimescale;
using System.Text.Json;


namespace MQTTnet_Receiver
{


	class Program
	{

		JanitzaJson janjson = new JanitzaJson();


		static async Task Main(string[] args)
		{

			// Konfigurationsdatei laden
			var configuration = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			// Konfiguration in die AppConfig-Klasse binden
			var appConfig = new AppConfig();
			configuration.Bind(appConfig);




			var subscribed = false;
			var mqttFactory = new MqttFactory();
			var receivedMessages = new List<MqttApplicationMessage>();


			var options =
				new MqttClientOptionsBuilder()
					.WithTcpServer($"{appConfig.Mqtt.Host}", appConfig.Mqtt.Port)
					.WithCredentials(appConfig.Mqtt.User, appConfig.Mqtt.PW)
					.Build();

			var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
					.WithClientOptions(options)
					.Build();

			var client = mqttFactory.CreateManagedMqttClient();

			// // Init database 
			var con = new NpgsqlConnection(
			connectionString: $"Server={appConfig.Database.Host};Port={appConfig.Database.Port};User Id={appConfig.Database.User};Password={appConfig.Database.PW};Database={appConfig.Database.Database};");
			con.Open();


			// Erstelle die Tabelle "sensors"
			using (var cmd = new NpgsqlCommand())
			{
				cmd.Connection = con;
				cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS sensors (
                            id SERIAL PRIMARY KEY,
                            Type TEXT NOT NULL,
                            Location TEXT NOT NULL
                        )";
				cmd.ExecuteNonQuery();
			}

			// Erstelle die Tabelle "sensor_data"
			using (var cmd = new NpgsqlCommand())
			{
				cmd.Connection = con;
				cmd.CommandText = @"CREATE TABLE IF NOT EXISTS sensor_data (
                                        time TIMESTAMPTZ NOT NULL,
                                        sensor_id INTEGER,
                                        Data JSONB NOT NULL,
                                        FOREIGN KEY (sensor_id) REFERENCES sensors (id)
                                    )";
                       
				cmd.ExecuteNonQuery();
			}

			// Erstelle den Hypertable
			using (var cmd = new NpgsqlCommand())
			{
				cmd.Connection = con;
				cmd.CommandText = "SELECT create_hypertable('sensor_data', by_range('time'), if_not_exists => TRUE);";
				cmd.ExecuteNonQuery();
			}



			// Ist tabelles schon vorhanden wenn nein dann anlegen 
			// ist hyped_table da wenn nein anlegen 
			// 

			try
			{
				await client.StartAsync(managedMqttClientOptions);

				// The application message is not sent. It is stored in an internal queue and
				// will be sent when the client is connected.
				await client.EnqueueAsync("Postgress/connected", "connected");

				Console.WriteLine("The managed MQTT client is connected.");

				// Wait until the queue is fully processed.
				SpinWait.SpinUntil(() => client.PendingApplicationMessagesCount == 0, 10000);

				Console.WriteLine($"Pending messages = {client.PendingApplicationMessagesCount}");

				client.ApplicationMessageReceivedAsync += args => SubscriptionsResultAsync(args, subscribed, con);
				await client.SubscribeAsync("power/#").ConfigureAwait(false);


				SpinWait.SpinUntil(() => subscribed, 1000);
				Console.WriteLine("Subscription properly done");
			}
			catch (MQTTnet.Adapter.MqttConnectingFailedException ex)
			{
				Console.WriteLine($"Connection Error: '{ex.Message}'");
				Console.WriteLine("Finish");
				return;
			}

			while (true)
			{ } // wie kann man sonst programm endlos laufen lassen ?


		}


		private static Task SubscriptionsResultAsync(MqttApplicationMessageReceivedEventArgs arg, bool subscribed, Npgsql.NpgsqlConnection con)
		{

			Console.WriteLine(subscribed);
			Console.WriteLine(Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment));



			var janitzaData = JsonSerializer.Deserialize<JanitzaJson>(arg.ApplicationMessage.ConvertPayloadToString());

			var SplittedTopic = arg.ApplicationMessage.Topic.Split('/');  // Power/{type}}/{id}


			// Füge einen neuen Sensor hinzu
			int sensorId;
			using (var cmd = new NpgsqlCommand("INSERT INTO IF NOT EXISTS sensors  (Type, Location) VALUES (@type, @location) RETURNING ID", con))
			{
				cmd.Parameters.AddWithValue("id", SplittedTopic[2]); // Type
				cmd.Parameters.AddWithValue("type", SplittedTopic[1]); // Type
				cmd.Parameters.AddWithValue("location", "Building1");
				
			}

			// Füge die Sensordaten hinzu
			using (var cmd = new NpgsqlCommand("INSERT INTO sensor_data (sensor_id, Data, time) VALUES (@sensor_id, @Data, @time)", con))
			{
				cmd.Parameters.AddWithValue("sensor_id", Int32.Parse(SplittedTopic[2]));
				cmd.Parameters.AddWithValue("Data", janitzaData);
				cmd.Parameters.AddWithValue("time", janitzaData.Timestamp);
				cmd.ExecuteNonQuery();
			}




			return Task.CompletedTask;
		}
	}
}

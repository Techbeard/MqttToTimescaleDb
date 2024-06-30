namespace MQTTnet_Receiver
{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    // Klassen für die Konfiguration
public class DatabaseConfig
{
    public string Host { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string PW { get; set; }
        public int Port { get; set; }

}

public class MqttConfig
{
    public string Host { get; set; }
    public string User { get; set; }
    public string PW { get; set; }
    public int Port { get; set; }
    public bool tls { get; set; }
}

public class AppConfig
{
    public DatabaseConfig Database { get; set; }
    public MqttConfig Mqtt { get; set; }
}
}

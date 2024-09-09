﻿namespace locket.Helpers
{
    public class AppConfigurationOption
    {
        public class AuthenticationOption
        {
            public GoogleConfig Google { get; set; } = new GoogleConfig();
            public string JWTKey { get; set; } = String.Empty;
        }

        public class GoogleConfig
        {
            public string ClientSecret { get; set; } = String.Empty;
            public string ClientID { get; set; } = String.Empty;
        }

        public class KafkaOption
        {
            public string Topic { get; set; } = String.Empty;
            public string BootstrapServers { get; set; } = String.Empty;
        }

        public class DatabaseOption
        {
            public string ConnectionString { get; set; } = String.Empty;
        }

        public const string AppConfiguration = "AppConfiguration";
        public AuthenticationOption Authentication { get; set; } = new AuthenticationOption();
        public DatabaseOption Database { get; set; } = new DatabaseOption();
        public KafkaOption Kafka { get; set; } = new KafkaOption();
    }

    public class AppConfig
    {
        public AppConfigurationOption Option { get; private set; }
        public AppConfig(IConfiguration configuration)
        {
            Option = new AppConfigurationOption();
            configuration.GetSection(AppConfigurationOption.AppConfiguration).Bind(Option);
        }
    }
}
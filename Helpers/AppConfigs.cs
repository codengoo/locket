namespace locket.Helpers
{
    public class AppConfigurationOption
    {
        public const string AppConfiguration = "AppConfiguration";
        public string JWTKey { get; set; } = String.Empty;
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
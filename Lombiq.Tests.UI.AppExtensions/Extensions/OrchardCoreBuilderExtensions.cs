using Microsoft.Extensions.Configuration;
using OrchardCore.Email;
using OrchardCore.Media.Azure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrchardCoreBuilderExtensions
    {
        public static OrchardCoreBuilder ConfigureUITesting(
            this OrchardCoreBuilder builder,
            IConfiguration configuration,
            bool enableShortcutsDuringUITesting = false)
        {
            if (!configuration.IsUITesting()) return builder;

            if (enableShortcutsDuringUITesting) builder.AddTenantFeatures("Lombiq.Tests.UI.Shortcuts", "OrchardCore.Roles");

            var smtpPort = configuration.GetValue<string>("Lombiq_Tests_UI_SmtpSettings:Port");

            if (!string.IsNullOrEmpty(smtpPort)) builder.AddTenantFeatures("OrchardCore.Email");

            var blobStorageConnectionString = configuration
                .GetValue<string>("Lombiq_Tests_UI_MediaBlobStorageOptions:ConnectionString");

            if (!string.IsNullOrEmpty(blobStorageConnectionString))
            {
                builder.AddTenantFeatures("OrchardCore.Media.Azure.Storage", "OrchardCore.Media.Cache");
            }

            return builder.ConfigureServices(
                services => services
                    .PostConfigure<SmtpSettings>(settings =>
                        configuration.GetSection("Lombiq_Tests_UI_SmtpSettings").Bind(settings))
                    .PostConfigure<MediaBlobStorageOptions>(options =>
                        configuration.GetSection("Lombiq_Tests_UI_MediaBlobStorageOptions").Bind(options)));
        }
    }
}

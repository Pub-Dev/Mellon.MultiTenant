using Hangfire;
using Hangfire.Server;
using Mellon.MultiTenant.Interfaces;

namespace WebApiHangfire.Jobs
{
    [Queue("crons")]
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IMultiTenantConfiguration _multiTenantConfiguration;

        public EmailSender(
            ILogger<EmailSender> logger,
            IMultiTenantConfiguration multiTenantConfiguration)
        {
            _logger = logger;

            _multiTenantConfiguration = multiTenantConfiguration;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation($"Processing e-mail sending for {_multiTenantConfiguration.Tenant}");

            await Task.FromResult(true);
        }
    }
}

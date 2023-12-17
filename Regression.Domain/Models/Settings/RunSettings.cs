namespace Regression.Domain.Models.Settings
{
    public class RunSettings
    {
        public Guid RunId { get; set; }
        public Guid TestId { get; set; }
        public Guid HubId { get; set; }
        public string? AppId { get; set; }
        public string? AppSecret { get; set; }
        public string? XApiKey { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public RunSettings()
        { }

        public RunSettings(Guid runId, Guid hubId, string? appId,
            string? appSecret, string? xApiKey, CancellationToken cancellationToken)
        {
            RunId = runId;
            HubId = hubId;
            AppId = appId;
            AppSecret = appSecret;
            XApiKey = xApiKey;
            CancellationToken = cancellationToken;
        }
    }
}

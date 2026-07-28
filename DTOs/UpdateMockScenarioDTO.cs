namespace ApiMockServer.DTOs
{
    public class UpdateMockScenarioDTO
    {
        public string MockEndpointId { get; set; } = string.Empty;

        public string ScenarioName { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string ResponseBody { get; set; } = string.Empty;

        public int Delay { get; set; }

        public bool IsActive { get; set; }

        public bool? EnableRandomFailure { get; set; }

        public int? FailureRate { get; set; }

        public bool EnableTimeout { get; set; }

        public int TimeoutDelay { get; set; }
    }
}
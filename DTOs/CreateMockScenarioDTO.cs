namespace ApiMockServer.DTOs
{
    public class CreateMockScenarioDTO
    {
        public string MockEndpointId { get; set; } = string.Empty;

        public string ScenarioName { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string ResponseBody { get; set; } = string.Empty;

        public int Delay { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
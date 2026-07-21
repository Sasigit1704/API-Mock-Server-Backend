namespace ApiMockServer.DTOs
{
    public class PatchMockScenarioDTO
    {
        public string? MockEndpointId { get; set; }

        public string? ScenarioName { get; set; }

        public int? StatusCode { get; set; }

        public string? ResponseBody { get; set; }

        public int? Delay { get; set; }

        public bool? IsActive { get; set; }
    }
}
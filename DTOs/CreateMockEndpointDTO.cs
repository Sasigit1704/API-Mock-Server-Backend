namespace ApiMockServer.DTOs
{
    public class CreateMockEndpointDto
    {
        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Method { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string ResponseBody { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;
    }
}
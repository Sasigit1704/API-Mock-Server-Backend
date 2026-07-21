namespace ApiMockServer.DTOs
{
    public class CreateMockEndpointDTO
    {
        public string Name { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Method { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public string ResponseBody { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;

        public string CollectionId { get; set; } = string.Empty;
    }
}
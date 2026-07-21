namespace ApiMockServer.DTOs
{
    public class PatchMockEndpointDto
    {
        public string? Name { get; set; } = string.Empty;

        public string? Path { get; set; } = string.Empty;

        public string? Method { get; set; } = string.Empty;

        [Range(100, 599)]
        public int? StatusCode { get; set; }

        public string? ResponseBody { get; set; } = string.Empty;

        public bool? IsEnabled { get; set; } = true;

        public string? CollectionId { get; set; } = string.Empty;
    }
}
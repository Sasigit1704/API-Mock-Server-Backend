using System.ComponentModel.DataAnnotations;
namespace ApiMockServer.DTOs
{
    public class PatchMockEndpointDTO
    {
        public string? Name { get; set; }

        public string? Path { get; set; }

        public string? Method { get; set; }

        [Range(100, 599)]
        public int? StatusCode { get; set; }

        public string? ResponseBody { get; set; }

        public bool? IsEnabled { get; set; }

        public string? CollectionId { get; set; }
    }
}
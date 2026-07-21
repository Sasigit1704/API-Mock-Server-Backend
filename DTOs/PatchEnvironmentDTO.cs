namespace ApiMockServer.DTOs
{
    public class PatchEnvironmentDTO
    {
        public string? Name { get; set; }

        public string? BaseUrl { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }
}
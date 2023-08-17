namespace BomComparer.Models
{
    public class BomFile
    {
        public string? Name { get; set; }
        public List<BomDataRow> Data { get; set; } = new();
    }
}

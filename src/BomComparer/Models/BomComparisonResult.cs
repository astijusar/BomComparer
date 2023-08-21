namespace BomComparer.Models
{
    public class BomComparisonResult
    {
        public List<BomComparisonResultEntry> ResultEntries { get; set; } = new();
        public string SourceFileName { get; set; }
        public string TargetFileName { get; set; }
    }
}

namespace VlcTracker.Service.Models.Api.Core;

public struct Filters
{
    public string? FileNameRegex { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public bool? ExcludeImported { get; set; }
    
}
using System.ComponentModel.DataAnnotations;

namespace DiveCenterManager.Models;

public class DiveSite
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    [Range(0, 999, ErrorMessage = "Maximum depth cannot be negative.")]
    public decimal? MaximumDepthMeters { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
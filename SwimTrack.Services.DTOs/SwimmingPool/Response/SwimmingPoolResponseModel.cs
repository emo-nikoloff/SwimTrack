namespace SwimTrack.Services.DTOs.SwimmingPool.Response;

public class SwimmingPoolResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Length { get; set; }

    public string? Location { get; set; }
}

namespace SwimTrack.Services.DTOs.SwimmingPool.Response;

public class AllSwimmingPoolsResponseModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Length { get; set; }

    public string? Location { get; set; }
}

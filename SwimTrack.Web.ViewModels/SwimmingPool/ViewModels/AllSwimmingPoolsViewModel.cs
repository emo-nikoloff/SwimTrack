namespace SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;

public class AllSwimmingPoolsViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Length { get; set; }

    public string? Location { get; set; }
}

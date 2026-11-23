namespace SwimTrack.Web.ViewModels.SwimmingPool.ViewModels;

public class AllSwimmingPoolsPageViewModel
{
    public AllSwimmingPoolsPageViewModel(IReadOnlyCollection<AllSwimmingPoolsViewModel> swimmingPools)
    {
        SwimmingPools = swimmingPools;
    }

    public IReadOnlyCollection<AllSwimmingPoolsViewModel> SwimmingPools { get; }

    public bool HasUserPools
        => SwimmingPools.Count > 0;
}

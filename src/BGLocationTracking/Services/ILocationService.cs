namespace blecon.BGLocationTracking.Services;

public interface ILocationService
{
    Task Run(CancellationToken cancellationToken);
}

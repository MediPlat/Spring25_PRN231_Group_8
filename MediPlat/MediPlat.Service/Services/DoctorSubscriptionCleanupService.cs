using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediPlat.Repository.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DoctorSubscriptionCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DoctorSubscriptionCleanupService> _logger;

    public DoctorSubscriptionCleanupService(IServiceScopeFactory scopeFactory, ILogger<DoctorSubscriptionCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var expiredSubscriptions = unitOfWork.DoctorSubscriptions
                        .GetList(ds => ds.EndDate.HasValue && ds.EndDate.Value < DateTime.Now && ds.Status != "Đã hết hạn")
                        .ToList();

                    if (expiredSubscriptions.Any())
                    {
                        _logger.LogInformation($"Updating {expiredSubscriptions.Count} expired subscriptions to 'Đã hết hạn'...");

                        foreach (var subscription in expiredSubscriptions)
                        {
                            subscription.Status = "Đã hết hạn";
                            unitOfWork.DoctorSubscriptions.UpdatePartial(subscription, ds => ds.Status);
                        }

                        await unitOfWork.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating expired doctor subscriptions.");
            }

            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}

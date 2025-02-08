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
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var expiredSubscriptions = unitOfWork.DoctorSubscriptions
                    .GetList(ds => ds.EndDate.HasValue && ds.EndDate.Value < DateTime.Now)
                    .ToList();

                if (expiredSubscriptions.Any())
                {
                    _logger.LogInformation($"Deleting {expiredSubscriptions.Count} expired subscriptions...");

                    foreach (var subscription in expiredSubscriptions)
                    {
                        unitOfWork.DoctorSubscriptions.Remove(subscription);
                    }

                    await unitOfWork.SaveChangesAsync();
                }
            }
            var nextRunTime = DateTime.Today.AddDays(1).AddHours(0).AddMinutes(0);
            var delay = nextRunTime - DateTime.Now;
            await Task.Delay(delay, stoppingToken);
        }
    }
}

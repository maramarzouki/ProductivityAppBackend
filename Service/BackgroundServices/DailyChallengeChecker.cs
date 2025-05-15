using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.ChallengeRepositoy;
using Service.ChallengeService;

namespace Service.BackgroundServices
{
    public class DailyChallengeChecker: IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;

        public DailyChallengeChecker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // calculate first run: tomorrow at 00:01
            var now = DateTime.Now;
            var next = now.Date.AddDays(1).AddMinutes(1);
            var delay = next - now;

            _timer = new Timer(DoWork, null, delay, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IChallengeRepository>();
            var service = scope.ServiceProvider.GetRequiredService<IChallengeService>();

            // <-- you’ll need a method to fetch all active challenges:
            var all = await repo.GetAllChallenges();
            foreach (var c in all.Where(c => !c.IsCanceled && !c.IsCompleted))
            {
                await service.CheckChallengeStatus(c.Id);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}

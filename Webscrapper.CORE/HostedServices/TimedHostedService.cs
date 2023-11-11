using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProjectVulkan.Core.HostedService;

/// <summary>
/// Based on Microsoft.Extensions.Hosting.BackgroundService  https://github.com/aspnet/Extensions/blob/master/src/Hosting/Abstractions/src/BackgroundService.cs
/// Additional info: - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2&tabs=visual-studio#timed-background-tasks
///                  - https://stackoverflow.com/questions/53844586/async-timer-in-scheduler-background-service
/// Slightly modified by NU
/// </summary>
public abstract class TimedHostedService : IHostedService, IDisposable
{
    protected ILogger<TimedHostedService> LoggerAdapter { get; set; }
    protected IServiceProvider Services { get; set; }

    private Timer _timer;
    private Task _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

    private TimeSpan _timerDueTime;
    private TimeSpan _timerPeriod;

    public TimedHostedService(ILogger<TimedHostedService> logger, IServiceProvider services, TimeSpan timerDueTime, TimeSpan timerPeriod)
    {
        LoggerAdapter = logger;
        Services = services;
        _timerDueTime = timerDueTime;
        _timerPeriod = timerPeriod;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        LoggerAdapter.LogInformation("Timed Background Service is starting.", DateTime.UtcNow);

        _timer = new Timer(ExecuteTask, null, _timerDueTime, _timerPeriod);

        return Task.CompletedTask;
    }

    private void ExecuteTask(object state)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
    }

    private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
    {
        using (var scope = Services.CreateScope())
        {
            try
            {
                await RunJobAsync(scope, stoppingToken).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                LoggerAdapter.LogError(ex, "An unexpected error occured in TimedHostedService.", DateTime.UtcNow);
            }
            _timer.Change(_timerPeriod, TimeSpan.FromMilliseconds(-1));
        }
    }

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> starts. The implementation should return a task 
    /// </summary>
    /// <param name="stoppingToken">Triggered when <see cref="IHostedService.StopAsync(CancellationToken)"/> is called.</param>
    /// <returns>A <see cref="Task"/> that represents the long running operations.</returns>
    protected abstract Task RunJobAsync(IServiceScope serviceScope, CancellationToken stoppingToken);

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        LoggerAdapter.LogInformation("Timed Background Service is stopping.", DateTime.UtcNow);
        _timer?.Change(Timeout.Infinite, 0);

        // Stop called without start
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            _stoppingCts.Cancel();
        }
        finally
        {
            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);
        }

    }

    public virtual void Dispose()
    {
        _stoppingCts.Cancel();
        _stoppingCts.Dispose();
        _timer?.Dispose();
    }
}


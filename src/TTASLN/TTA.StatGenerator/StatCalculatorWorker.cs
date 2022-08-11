namespace TTA.StatGenerator;

public class StatCalculatorWorker : BackgroundService
{
    private readonly ILogger<StatCalculatorWorker> logger;

    public StatCalculatorWorker(ILogger<StatCalculatorWorker> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //TODO:Use CRON expressions and PeriodTaskTimer to leverage async ticks
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
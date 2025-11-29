using System.Diagnostics;
using System.Collections.Concurrent;

namespace MauiHybridApp.Services.Performance;

/// <summary>
/// Performance monitoring and optimization service
/// </summary>
public interface IPerformanceService
{
    Task<PerformanceMetrics> StartOperationAsync(string operationName);
    Task EndOperationAsync(PerformanceMetrics metrics);
    Task<PerformanceReport> GetPerformanceReportAsync();
    Task ClearMetricsAsync();
    Task<bool> IsSlowOperationAsync(string operationName, TimeSpan threshold);
}

public class PerformanceService : IPerformanceService
{
    private readonly ConcurrentDictionary<string, List<OperationMetric>> _metrics = new();
    private readonly ConcurrentDictionary<string, PerformanceMetrics> _activeOperations = new();

    public async Task<PerformanceMetrics> StartOperationAsync(string operationName)
    {
        var metrics = new PerformanceMetrics
        {
            OperationName = operationName,
            StartTime = DateTime.UtcNow,
            Stopwatch = Stopwatch.StartNew(),
            OperationId = Guid.NewGuid().ToString()
        };

        _activeOperations.TryAdd(metrics.OperationId, metrics);
        
        return await Task.FromResult(metrics);
    }

    public async Task EndOperationAsync(PerformanceMetrics metrics)
    {
        if (metrics?.Stopwatch == null)
            return;

        metrics.Stopwatch.Stop();
        metrics.EndTime = DateTime.UtcNow;
        metrics.Duration = metrics.Stopwatch.Elapsed;

        // Remove from active operations
        _activeOperations.TryRemove(metrics.OperationId, out _);

        // Add to historical metrics
        var operationMetric = new OperationMetric
        {
            OperationName = metrics.OperationName,
            Duration = metrics.Duration,
            StartTime = metrics.StartTime,
            EndTime = metrics.EndTime,
            Success = metrics.Success,
            ErrorMessage = metrics.ErrorMessage,
            MemoryUsed = GC.GetTotalMemory(false)
        };

        _metrics.AddOrUpdate(
            metrics.OperationName,
            new List<OperationMetric> { operationMetric },
            (key, existing) =>
            {
                existing.Add(operationMetric);
                // Keep only last 100 operations per type
                if (existing.Count > 100)
                {
                    existing.RemoveRange(0, existing.Count - 100);
                }
                return existing;
            });

        await Task.CompletedTask;
    }

    public Task<PerformanceReport> GetPerformanceReportAsync()
    {
        var report = new PerformanceReport
        {
            GeneratedAt = DateTime.UtcNow,
            TotalOperations = _metrics.Values.Sum(list => list.Count),
            ActiveOperations = _activeOperations.Count,
            OperationSummaries = new List<OperationSummary>()
        };

        foreach (var kvp in _metrics)
        {
            var operations = kvp.Value;
            if (operations.Any())
            {
                var summary = new OperationSummary
                {
                    OperationName = kvp.Key,
                    TotalExecutions = operations.Count,
                    SuccessfulExecutions = operations.Count(o => o.Success),
                    FailedExecutions = operations.Count(o => !o.Success),
                    AverageDuration = TimeSpan.FromMilliseconds(operations.Average(o => o.Duration.TotalMilliseconds)),
                    MinDuration = operations.Min(o => o.Duration),
                    MaxDuration = operations.Max(o => o.Duration),
                    LastExecuted = operations.Max(o => o.EndTime),
                    AverageMemoryUsage = operations.Average(o => o.MemoryUsed)
                };

                // Calculate percentiles
                var sortedDurations = operations.Select(o => o.Duration.TotalMilliseconds).OrderBy(d => d).ToArray();
                if (sortedDurations.Length > 0)
                {
                    summary.P50Duration = TimeSpan.FromMilliseconds(GetPercentile(sortedDurations, 0.5));
                    summary.P95Duration = TimeSpan.FromMilliseconds(GetPercentile(sortedDurations, 0.95));
                    summary.P99Duration = TimeSpan.FromMilliseconds(GetPercentile(sortedDurations, 0.99));
                }

                report.OperationSummaries.Add(summary);
            }
        }

        return Task.FromResult(report);
    }

    public Task ClearMetricsAsync()
    {
        _metrics.Clear();
        return Task.CompletedTask;
    }

    public Task<bool> IsSlowOperationAsync(string operationName, TimeSpan threshold)
    {
        if (!_metrics.TryGetValue(operationName, out var operations) || !operations.Any())
            return Task.FromResult(false);

        var recentOperations = operations.TakeLast(10);
        var averageDuration = TimeSpan.FromMilliseconds(recentOperations.Average(o => o.Duration.TotalMilliseconds));

        return Task.FromResult(averageDuration > threshold);
    }

    private static double GetPercentile(double[] sortedData, double percentile)
    {
        if (sortedData.Length == 0) return 0;
        if (sortedData.Length == 1) return sortedData[0];

        double index = percentile * (sortedData.Length - 1);
        int lowerIndex = (int)Math.Floor(index);
        int upperIndex = (int)Math.Ceiling(index);

        if (lowerIndex == upperIndex)
            return sortedData[lowerIndex];

        double weight = index - lowerIndex;
        return sortedData[lowerIndex] * (1 - weight) + sortedData[upperIndex] * weight;
    }
}

public class PerformanceMetrics
{
    public string OperationName { get; set; } = string.Empty;
    public string OperationId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public Stopwatch? Stopwatch { get; set; }
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

public class OperationMetric
{
    public string OperationName { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public long MemoryUsed { get; set; }
}

public class PerformanceReport
{
    public DateTime GeneratedAt { get; set; }
    public int TotalOperations { get; set; }
    public int ActiveOperations { get; set; }
    public List<OperationSummary> OperationSummaries { get; set; } = new();
}

public class OperationSummary
{
    public string OperationName { get; set; } = string.Empty;
    public int TotalExecutions { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public TimeSpan AverageDuration { get; set; }
    public TimeSpan MinDuration { get; set; }
    public TimeSpan MaxDuration { get; set; }
    public TimeSpan P50Duration { get; set; }
    public TimeSpan P95Duration { get; set; }
    public TimeSpan P99Duration { get; set; }
    public DateTime LastExecuted { get; set; }
    public double AverageMemoryUsage { get; set; }
    
    public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions * 100 : 0;
}

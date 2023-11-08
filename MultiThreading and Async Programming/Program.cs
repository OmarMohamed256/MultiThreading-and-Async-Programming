using System.Diagnostics;

public class AsyncAndMultithreadingExample
{
    public static async Task Main()
    {
        Stopwatch totalStopwatch = Stopwatch.StartNew(); // Start timing total operation

        // I/O-bound operation: Get numbers from the database asynchronously
        Stopwatch ioStopwatch = Stopwatch.StartNew(); // Start timing I/O operation
        List<int> numbers = await GetNumbersFromDatabaseAsync();
        ioStopwatch.Stop(); // Stop timing I/O operation
        Console.WriteLine($"Time elapsed for I/O operation: {ioStopwatch.ElapsedMilliseconds}ms");

        // CPU-bound operation: Compute Fibonacci for each number
        Stopwatch cpuStopwatch = Stopwatch.StartNew(); // Start timing CPU operation
        List<Task<(int, long, long)>> fibTasks = new();
        foreach (var number in numbers)
        {
            // Offload the computation to a thread pool thread
            fibTasks.Add(Task.Run(() =>
            {
                Stopwatch fibStopwatch = Stopwatch.StartNew(); // Start timing Fibonacci computation
                long result = Fibonacci(number);
                fibStopwatch.Stop(); // Stop timing Fibonacci computation
                return (number, result, fibStopwatch.ElapsedMilliseconds);
            }));
        }

        // Wait for all the tasks to complete
        var results = await Task.WhenAll(fibTasks);
        cpuStopwatch.Stop(); // Stop timing CPU operation
        Console.WriteLine($"Time elapsed for CPU-bound operations: {cpuStopwatch.ElapsedMilliseconds}ms");

        // Output the results
        foreach (var result in results)
        {
            Console.WriteLine($"Fibonacci of {result.Item1} is {result.Item2}. Computation took {result.Item3}ms");
        }

        totalStopwatch.Stop(); // Stop timing total operation
        Console.WriteLine($"Total time elapsed: {totalStopwatch.ElapsedMilliseconds}ms");
    }

    private static async Task<List<int>> GetNumbersFromDatabaseAsync()
    {
        // Simulating a database call with a delay
        await Task.Delay(1000); // Simulate async I/O operation

        // Assume these numbers are fetched from a database
        return new List<int> { 40, 40, 40, 40, 40 };
    }

    // Recursive method to calculate the n-th Fibonacci number
    private static long Fibonacci(int n)
    {
        if (n <= 1)
            return n;
        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }
}

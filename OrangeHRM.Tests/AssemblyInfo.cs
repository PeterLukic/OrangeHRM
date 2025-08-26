using NUnit.Framework;

// Configure parallel execution at the assembly level
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(3)] // Set the number of parallel workers
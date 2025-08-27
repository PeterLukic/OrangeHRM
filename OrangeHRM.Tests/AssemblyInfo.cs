// Configure parallel execution at the assembly level
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(1)] // Reduced to 1 for stability - you can increase after confirming tests work


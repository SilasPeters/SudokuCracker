# How to run

In Program.cs you can set two constants:
- Benchmark
  - True: shows benchmarks.\
    **NOTE: project must be run in release mode, and weird characters in the path to the project (like spaces or underscores) or a too long path may prevent compilation of the benchmarks.**
  - False: shows the solutions to the sudokus.
- UseMostConstrainedVariable
  - When not benchmarking, when set to true, will not show the results obtained from solving the sudokus using CBT, but CBT combined with the MCV heuristics.
- CountIterations
  - When not benchmarking, when set to true, will also print the number of iterations it took to solve a sudoku.
- SudokusPath
  - Relative path to a file containing the sudokus to solve, following the format specified in the example sudokus.
  - You can add as many sudokus as you like, the code will handle them.
  - Note that I added one sudoku which does not have a solution.

# How to read the benchmarks
*The benchmarks are generated by the [BenchmarkDotNet package](https://benchmarkdotnet.org/index.html).*

When running in benchmark mode, the program will print the results to the console.
At first you will see that BenchmarkDotNet is warming up, and after that it will run the benchmarks.
When completed, the following table is what you want to look at:
```
|  Method | Sudoku |        Mean |    Error |   StdDev |    Gen0 |  Allocated |
|-------- |------- |------------:|---------:|---------:|--------:|-----------:|
|     CBT |      0 |    48.21 us | 0.141 us | 0.132 us |  0.3052 |   28.17 KB |
| CBT_MCV |      0 |    97.45 us | 0.249 us | 0.233 us |  0.6104 |   56.27 KB |
|     CBT |      1 |    81.25 us | 0.216 us | 0.202 us |  0.4883 |   46.27 KB |
| CBT_MCV |      1 |   116.54 us | 0.473 us | 0.420 us |  0.7324 |   66.56 KB |
|     CBT |      2 |   680.53 us | 0.442 us | 0.369 us |  3.9063 |  380.02 KB |
| CBT_MCV |      2 |   228.38 us | 0.240 us | 0.224 us |  1.4648 |     127 KB |
|     CBT |      3 |   124.41 us | 0.376 us | 0.352 us |  0.7324 |   72.88 KB |
| CBT_MCV |      3 |   105.41 us | 0.308 us | 0.288 us |  0.7324 |   60.77 KB |
|     CBT |      4 |    38.74 us | 0.113 us | 0.105 us |  0.2441 |   22.66 KB |
| CBT_MCV |      4 |    84.29 us | 0.176 us | 0.165 us |  0.6104 |    50.2 KB |
|     CBT |      5 | 2,078.41 us | 4.665 us | 4.363 us | 11.7188 | 1189.86 KB |
| CBT_MCV |      5 |    43.99 us | 0.253 us | 0.236 us |  0.3052 |   25.56 KB |
```
**Method**: the method used to solve the sudoku.\
**Sudoku**: the sudoku to solve.\
**Mean**: the mean time it took to solve the sudoku.\
**Gen0**: the average number of times the garbage collector was called.\
**Allocated**: the amount of memory allocated to solve the sudoku.

These are some results I got on my laptop. As you can see, 12 benchmarks were run.

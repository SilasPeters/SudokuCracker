# SudokuCracker
Two approaches to solving Sudokus

I'm mostly proud of the Chronological Backtracking variant (CBT), which also implements Most-Constrained-Variable heuristics. I worked on the CBT variant myself, whilst the Iterated Local Search (ILS} variant was made with my school-mates.

## Report
Included with the CBT variant, is a [report](/SudokuCrackerCBT/Report%20on%20CBT%20with%20MCV.md) (also availabe in ``.docx`` and ``.pdf``) on developing and exploring the CBT algorithm.

## Benchmarks
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
```
**CBT**: Plain chronological Bactracking  
**CBT_MCV**: CBT but with Most-Constrained-Variable heuristics.

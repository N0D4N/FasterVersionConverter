``` ini

BenchmarkDotNet=v0.13.0, OS=neon 20.04
Intel Core i3-7130U CPU 2.70GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK=6.0.100-preview.5.21302.13
  [Host]     : .NET 6.0.0 (6.0.21.30105), X64 RyuJIT
  DefaultJob : .NET 6.0.0 (6.0.21.30105), X64 RyuJIT


```
|                       Method |      unparsedVersion |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------------------------- |--------------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|    **CurrentConverterSerialize** |                  **1.0** | **441.3 ns** | **0.22 ns** | **0.20 ns** | **0.1526** |     **-** |     **-** |     **240 B** |
|   ProposedConverterSerialize |                  1.0 | 430.0 ns | 0.44 ns | 0.41 ns | 0.1326 |     - |     - |     208 B |
|  CurrentConverterDeserialize |                  1.0 | 487.3 ns | 0.13 ns | 0.11 ns | 0.0553 |     - |     - |      88 B |
| ProposedConverterDeserialize |                  1.0 | 434.7 ns | 0.27 ns | 0.24 ns | 0.0353 |     - |     - |      56 B |
|    **CurrentConverterSerialize** |                **1.0.0** | **450.4 ns** | **0.36 ns** | **0.32 ns** | **0.1578** |     **-** |     **-** |     **248 B** |
|   ProposedConverterSerialize |                1.0.0 | 458.7 ns | 0.21 ns | 0.18 ns | 0.1373 |     - |     - |     216 B |
|  CurrentConverterDeserialize |                1.0.0 | 520.6 ns | 1.54 ns | 1.20 ns | 0.0553 |     - |     - |      88 B |
| ProposedConverterDeserialize |                1.0.0 | 465.3 ns | 0.17 ns | 0.14 ns | 0.0353 |     - |     - |      56 B |
|    **CurrentConverterSerialize** |              **1.0.0.0** | **467.4 ns** | **0.51 ns** | **0.48 ns** | **0.1631** |     **-** |     **-** |     **256 B** |
|   ProposedConverterSerialize |              1.0.0.0 | 460.8 ns | 0.30 ns | 0.27 ns | 0.1373 |     - |     - |     216 B |
|  CurrentConverterDeserialize |              1.0.0.0 | 557.7 ns | 0.33 ns | 0.31 ns | 0.0610 |     - |     - |      96 B |
| ProposedConverterDeserialize |              1.0.0.0 | 470.9 ns | 0.09 ns | 0.08 ns | 0.0353 |     - |     - |      56 B |
|    **CurrentConverterSerialize** | **21474(...)83647 [21]** | **499.3 ns** | **0.58 ns** | **0.54 ns** | **0.1984** |     **-** |     **-** |     **312 B** |
|   ProposedConverterSerialize | 21474(...)83647 [21] | 491.3 ns | 0.49 ns | 0.41 ns | 0.1574 |     - |     - |     248 B |
|  CurrentConverterDeserialize | 21474(...)83647 [21] | 566.1 ns | 0.55 ns | 0.49 ns | 0.0763 |     - |     - |     120 B |
| ProposedConverterDeserialize | 21474(...)83647 [21] | 522.3 ns | 0.21 ns | 0.19 ns | 0.0353 |     - |     - |      56 B |
|    **CurrentConverterSerialize** | **21474(...)83647 [32]** | **532.0 ns** | **0.91 ns** | **0.81 ns** | **0.2289** |     **-** |     **-** |     **360 B** |
|   ProposedConverterSerialize | 21474(...)83647 [32] | 507.1 ns | 0.40 ns | 0.36 ns | 0.1726 |     - |     - |     272 B |
|  CurrentConverterDeserialize | 21474(...)83647 [32] | 627.8 ns | 0.30 ns | 0.28 ns | 0.0916 |     - |     - |     144 B |
| ProposedConverterDeserialize | 21474(...)83647 [32] | 534.8 ns | 0.29 ns | 0.28 ns | 0.0353 |     - |     - |      56 B |
|    **CurrentConverterSerialize** | **21474(...)83647 [43]** | **583.9 ns** | **0.40 ns** | **0.38 ns** | **0.2546** |     **-** |     **-** |     **400 B** |
|   ProposedConverterSerialize | 21474(...)83647 [43] | 558.4 ns | 0.46 ns | 0.41 ns | 0.1831 |     - |     - |     288 B |
|  CurrentConverterDeserialize | 21474(...)83647 [43] | 692.8 ns | 0.49 ns | 0.44 ns | 0.1068 |     - |     - |     168 B |
| ProposedConverterDeserialize | 21474(...)83647 [43] | 584.8 ns | 0.18 ns | 0.14 ns | 0.0353 |     - |     - |      56 B |

# Cold-start measure
Simple application for AWS lambda function cold-start measurement.

Application measures the cold-start time by calculating the difference between cold and warm lambda invocations. The result is an average value from 10 measurements.

The results are provided for several memory size configurations. Be aware the application modifies the memory size of the provided lambda function.  

Cold-starts are enforced by modifying lambda's function description.

## Requirements
* dotnet sdk 3.1
* aws cli

## Usage
Run the application providing the lambda function name as an argument. 

```
Rozchmurzeni.ColdStartMeasure.exe my-lambda-function
```
## Sample output
```
128 MB: 11,4732 s
256 MB: 5,3842 s
512 MB: 2,7544 s
1024 MB: 1,451 s
2048 MB: 0,9834 s
3008 MB: 0,1958 s
```

Func<double, double> f = (x) => Math.Pow(x, 1/(double)3) + Math.Sin(x);

var numbers = Enumerable.Range(0, 21).ToArray();

List<string> tuples = new List<string>();
for(int i = 0; i < numbers.Length; i++)
{
	tuples.Add($"({(numbers[i] * 0.8).ToString("F2")},{f(numbers[i] * 0.8).ToString("F2")})");
}

Console.WriteLine(string.Join(",", tuples));
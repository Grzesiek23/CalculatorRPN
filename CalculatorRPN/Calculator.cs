using Xunit;

namespace CalculatorRPN;

public class Calculator
{
    private readonly List<double> _stack = new();

    public double Calculate(object[] input)
    {
        for (var i = 0; i < input.Length; i++)
        {
            var parseAble = double.TryParse(input[i].ToString(), out var number);

            if (i < 2 && !parseAble)
                throw new ArgumentException("Invalid input");

            if (parseAble)
                _stack.Add(number);
            else
            {
                if (_stack.Count < 2)
                    throw new ArgumentException("Invalid input");

                var operation = input[i].ToString();

                var first = _stack[^2];
                var second = _stack[^1];
                _stack.RemoveAt(_stack.Count - 1);
                _stack.RemoveAt(_stack.Count - 1);

                var result = CalculateOperation(first, second, operation!);
                _stack.Add(result);
            }
        }
        
        if (_stack.Count != 1)
            throw new InvalidOperationException("Something went wrong");
        
        return _stack.First();
    }

    private static double CalculateOperation(double a, double b, string operation)
    {
        if (b == 0 && operation.Equals("/"))
            throw new DivideByZeroException();

        return operation switch
        {
            "+" => a + b,
            "-" => a - b,
            "*" => a * b,
            "/" => a / b,
            _ => throw new ArgumentException("Invalid operation")
        };
    }
}

public class CalculatorTests
{
    private readonly Calculator _sut;

    public CalculatorTests()
    {
        _sut = new Calculator();
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void CheckCalculateReturnsCorrectResults(double expected, params object[] input)
    {
        Assert.Equal(expected, _sut.Calculate(input));
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] {8, new object[] {5, 3, '+'}};
        yield return new object[] {5, new object[] {15, 7, 1, 1, "+", '-', '/', 3, "*", 2, 1, 1, '+', "+", "-"}};
        yield return new object[] {2, new object[] {3, 1, '-'}};
        yield return new object[] {10, new object[] {3, 2, '*', 4, '+'}};
    }
}
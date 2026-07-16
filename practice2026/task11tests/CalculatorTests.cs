using System;
using Xunit;
using task11;

namespace task11tests;

public class CalculatorTests
{
    private readonly ICalculator _calc;

    public CalculatorTests()
    {
        _calc = CalculatorFactory.BuildDynamicCalculator();
    }

    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(-9, -3, -12)]
    [InlineData(0, 0, 0)]
    public void Add_ShouldReturnCorrectSum(int a, int b, int expected)
    {
        Assert.Equal(expected, _calc.Add(a, b));
    }

    [Theory]
    [InlineData(10, 9, 1)]
    [InlineData(-7, -9, 2)]
    public void Minus_ShouldReturnCorrectDifference(int a, int b, int expected)
    {
        Assert.Equal(expected, _calc.Minus(a, b));
    }

    [Theory]
    [InlineData(7, 3, 21)]
    [InlineData(-12, 3, -36)]
    public void Mul_ShouldReturnCorrectProduct(int a, int b, int expected)
    {
        Assert.Equal(expected, _calc.Mul(a, b));
    }

    [Theory]
    [InlineData(8, 2, 4)]
    [InlineData(-6, -6, 1)]
    public void Div_ShouldReturnCorrectQuotient(int a, int b, int expected)
    {
        Assert.Equal(expected, _calc.Div(a, b));
    }

    [Fact]
    public void Div_ByZero_ShouldThrowDivideByZeroException()
    {
        Assert.Throws<DivideByZeroException>(() => _calc.Div(10, 0));
    }
}
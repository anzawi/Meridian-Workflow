namespace Meridian.Core.Tests;

using Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("hello-world", "HelloWorld")]
    [InlineData("test_string", "TestString")]
    [InlineData("camelCase", "CamelCase")]
    [InlineData("PascalCase", "PascalCase")]
    [InlineData("multiple__underscores", "MultipleUnderscores")]
    [InlineData("mixed-case_string", "MixedCaseString")]
    [InlineData("123-number", "123Number")]
    [InlineData("special@#$characters", "SpecialCharacters")]
    public void ToPascalCase_ConvertsDifferentFormats_ToPascalCase(string input, string expected)
    {
        var result = input.ToPascalCase();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ToPascalCase_WithEmptyOrNullInput_ReturnsEmptyString(string input)
    {
        var result = input.ToPascalCase();
        Assert.Equal(string.Empty, result);
    }
}
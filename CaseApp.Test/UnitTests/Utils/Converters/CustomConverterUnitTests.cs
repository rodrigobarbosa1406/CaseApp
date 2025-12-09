using CaseApp.Enums.Invoices;
using CaseApp.Utils.Converters;

namespace CaseApp.Test.UnitTests.Utils.Converters;

public class CustomConverterUnitTests
{
    [Theory]
    [InlineData(ReconciliationStatus.PENDENTE, "Pendente")]
    [InlineData(ReconciliationStatus.CONCILIADO, "Conciliado")]
    [InlineData(ReconciliationStatus.DIVERGENTE, "Divergente")]
    [InlineData(ReconciliationStatus.ANOMALIA, "Anomalia")]
    [InlineData(ReconciliationStatus.FRAUDE, "Fraude")]
    public void GetEnumDescription_ShouldReturnCorrectDescription(ReconciliationStatus status, string expected)
    {
        var result = CustomConverter.GetEnumDescription(status);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetEnumDescription_ShouldReturnEmpty_WhenValueIsNull()
    {
        var result = CustomConverter.GetEnumDescription(null);

        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("Pendente", ReconciliationStatus.PENDENTE)]
    [InlineData("Conciliado", ReconciliationStatus.CONCILIADO)]
    [InlineData("Divergente", ReconciliationStatus.DIVERGENTE)]
    [InlineData("Anomalia", ReconciliationStatus.ANOMALIA)]
    [InlineData("Fraude", ReconciliationStatus.FRAUDE)]
    public void GetEnumValueFromDescription_ShouldReturnCorrectEnum(string description, ReconciliationStatus expected)
    {
        var result = CustomConverter.GetEnumValueFromDescription<ReconciliationStatus>(description);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetEnumValueFromDescription_ShouldReturnDefault_WhenDescriptionDoesNotExist()
    {
        var result = CustomConverter.GetEnumValueFromDescription<ReconciliationStatus>("Inexistente");

        Assert.Equal(default(ReconciliationStatus), result);
    }

    [Fact]
    public void GetEnumValueFromDescription_ShouldReturnDefault_WhenDescriptionIsNull()
    {
        var result = CustomConverter.GetEnumValueFromDescription<ReconciliationStatus>(null);

        Assert.Equal(default(ReconciliationStatus), result);
    }

}
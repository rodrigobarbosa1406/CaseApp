using CaseApp.DataTransferObjects.Invoices;
using CaseApp.Enums.Invoices;

namespace CaseApp.Test.UnitTests.DataTransferObjects.Invoices;

public class ReconciliationGetDTOUnitTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var customerId = 123;
        var invoiceId = "INV-001";
        var amount = 1500.75m;
        var status = ReconciliationStatus.CONCILIADO;
        var dueDate = new DateTime(2025, 12, 31);
        var reconciliationAt = DateTime.UtcNow;

        var dto = new ReconciliationGetDTO(
            id,
            customerId,
            invoiceId,
            amount,
            status,
            dueDate,
            reconciliationAt
        );

        Assert.Equal(id, dto.Id);
        Assert.Equal(customerId, dto.CustomerId);
        Assert.Equal(invoiceId, dto.InvoiceId);
        Assert.Equal(amount, dto.Amount);
        Assert.Equal("Conciliado", dto.ReconciliationStatus);
        Assert.Equal(dueDate, dto.DueDate);
        Assert.Equal(reconciliationAt, dto.ReconciliationAt);
    }

    [Fact]
    public void DefaultConstructor_ShouldAllowPropertyAssignment()
    {
        var dto = new ReconciliationGetDTO
        {
            Id = Guid.NewGuid(),
            CustomerId = 456,
            InvoiceId = "INV-002",
            Amount = 500m,
            ReconciliationStatus = "Pendente",
            DueDate = new DateTime(2025, 11, 30),
            ReconciliationAt = DateTime.UtcNow
        };

        Assert.Equal("Pendente", dto.ReconciliationStatus);
        Assert.Equal(456, dto.CustomerId);
        Assert.Equal("INV-002", dto.InvoiceId);
        Assert.Equal(500m, dto.Amount);
    }

}
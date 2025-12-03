using CaseApp.Enums.Invoices;
using CaseApp.Models.Invoices;

namespace CaseApp.Test.UnitTests.Models.Invoices;

public class ReconciliationUnitTests
{
    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var customerId = 123;
        var invoiceId = "INV-001";
        var amount = 1000.50m;
        var status = ReconciliationStatus.CONCILIADO;
        var dueDate = new DateTime(2025, 12, 31);
        var reconciliationAt = DateTime.UtcNow;

        var reconciliation = new Reconciliation(
            id,
            customerId,
            invoiceId,
            amount,
            status,
            dueDate,
            reconciliationAt
        );

        Assert.Equal(id, reconciliation.Id);
        Assert.Equal(customerId, reconciliation.CustomerId);
        Assert.Equal(invoiceId, reconciliation.InvoiceId);
        Assert.Equal(amount, reconciliation.Amount);
        Assert.Equal(status, reconciliation.ReconciliationStatus);
        Assert.Equal(dueDate, reconciliation.DueDate);
        Assert.Equal(reconciliationAt, reconciliation.ReconciliationAt);
    }

    [Fact]
    public void DefaultConstructor_ShouldAllowPropertyAssignment()
    {
        var reconciliation = new Reconciliation
        {
            CustomerId = 456,
            InvoiceId = "INV-002",
            Amount = 500m
        };

        Assert.Equal(456, reconciliation.CustomerId);
        Assert.Equal("INV-002", reconciliation.InvoiceId);
        Assert.Equal(500m, reconciliation.Amount);
    }
}
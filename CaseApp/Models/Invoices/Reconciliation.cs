using CaseApp.Enums.Invoices;

namespace CaseApp.Models.Invoices;

public class Reconciliation
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public string InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public ReconciliationStatus ReconciliationStatus { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ReconciliationAt { get; set; }

    public Reconciliation() { }

    public Reconciliation(Guid id, int customerId, string invoiceId, decimal amount, ReconciliationStatus reconciliationStatus, DateTime dueDate, DateTime reconciliationAt)
    {
        Id = id;
        CustomerId = customerId;
        InvoiceId = invoiceId;
        Amount = amount;
        ReconciliationStatus = reconciliationStatus;
        DueDate = dueDate;
        ReconciliationAt = reconciliationAt;
    }
}
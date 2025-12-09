using CaseApp.Enums.Invoices;
using CaseApp.Utils.Converters;

namespace CaseApp.DataTransferObjects.Invoices;

public class ReconciliationGetDTO
{
    public Guid Id { get; set; }
    public int CustomerId { get; set; }
    public string InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string ReconciliationStatus { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ReconciliationAt { get; set; }

    public ReconciliationGetDTO() { }

    public ReconciliationGetDTO(Guid id, int customerId, string invoiceId, decimal amount, ReconciliationStatus reconciliationStatus, DateTime dueDate, DateTime reconciliationAt)
    {
        Id = id;
        CustomerId = customerId;
        InvoiceId = invoiceId;
        Amount = amount;
        ReconciliationStatus = CustomConverter.GetEnumDescription(reconciliationStatus);
        DueDate = dueDate;
        ReconciliationAt = reconciliationAt;
    }
}
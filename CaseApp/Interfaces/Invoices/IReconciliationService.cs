using CaseApp.DataTransferObjects.Common;
using CaseApp.DataTransferObjects.Invoices;
using CaseApp.Enums.Invoices;

namespace CaseApp.Interfaces.Invoices;

public interface IReconciliationService
{
    Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsAsync(int skip, int take);

    Task<ReconciliationGetDTO> GetReconciliationByIdAsync(Guid id);

    Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsByCustomerIdAsync(int customerId, int skip, int take);

    Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsByInvoiceIdAsync(string invoiceId, int skip, int take);

    Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsByStatusAsync(ReconciliationStatus reconciliationStatus, int skip, int take);
}
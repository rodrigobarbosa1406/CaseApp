using CaseApp.Data;
using CaseApp.DataTransferObjects.Common;
using CaseApp.DataTransferObjects.Invoices;
using CaseApp.Enums.Invoices;
using CaseApp.Interfaces.Invoices;
using CaseApp.Utils.ErrorHandler.Commom;
using Microsoft.EntityFrameworkCore;

namespace CaseApp.Services.Invoices;

public class ReconciliationService(ApplicationDbContext context, ILogger<ReconciliationService> logger) : IReconciliationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ReconciliationService> _logger = logger;

    public async Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsAsync(int skip, int take)
    {
        var queryable = _context.Reconciliation.AsNoTracking();
        var queryableCount = queryable;

        queryable = queryable.Skip(skip).Take(take);

        var reconciliationsGetDTO = await queryable.Select(r =>
            new ReconciliationGetDTO(
                r.Id,
                r.CustomerId,
                r.InvoiceId,
                r.Amount,
                r.ReconciliationStatus,
                r.DueDate,
                r.ReconciliationAt
            )).ToListAsync();

        double totalRegs = await queryableCount.CountAsync();
        double totalPages = totalRegs > 0 ? Math.Ceiling(totalRegs / take) : 0;

        var getWithPaginationGetDTO = new GetWithPaginationGetDTO<ReconciliationGetDTO>(reconciliationsGetDTO, (int)totalRegs, (int)totalPages);

        return getWithPaginationGetDTO;
    }

    public async Task<ReconciliationGetDTO> GetReconciliationByIdAsync(Guid id)
    {
        var reconciliation = await _context.Reconciliation.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reconciliation == null)
        {
            _logger.LogInformation("Não foi encontrada uma conciliação com o id {Id}", id);
            throw new NotFoundException($"Não foi encontrada uma conciliação com o id {id}");
        }

        var reconciliationGetDTO = new ReconciliationGetDTO(
            reconciliation.Id,
            reconciliation.CustomerId,
            reconciliation.InvoiceId,
            reconciliation.Amount,
            reconciliation.ReconciliationStatus,
            reconciliation.DueDate,
            reconciliation.ReconciliationAt
        );

        return reconciliationGetDTO;
    }

    public async Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsByCustomerIdAsync(int customerId, int skip, int take)
    {
        var queryable = _context.Reconciliation.AsNoTracking().Where(r => r.CustomerId == customerId);
        var queryableCount = queryable;

        queryable = queryable.Skip(skip).Take(take);

        var reconciliationsGetDTO = await queryable.Select(r =>
            new ReconciliationGetDTO(
                r.Id,
                r.CustomerId,
                r.InvoiceId,
                r.Amount,
                r.ReconciliationStatus,
                r.DueDate,
                r.ReconciliationAt
            )).ToListAsync();

        double totalRegs = await queryableCount.CountAsync();
        double totalPages = totalRegs > 0 ? Math.Ceiling(totalRegs / take) : 0;

        var getWithPaginationGetDTO = new GetWithPaginationGetDTO<ReconciliationGetDTO>(reconciliationsGetDTO, (int)totalRegs, (int)totalPages);

        return getWithPaginationGetDTO;
    }

    public async Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsByInvoiceIdAsync(string invoiceId, int skip, int take)
    {
        var queryable = _context.Reconciliation.AsNoTracking().Where(r => r.InvoiceId == invoiceId);
        var queryableCount = queryable;

        queryable = queryable.Skip(skip).Take(take);

        var reconciliationsGetDTO = await queryable.Select(r =>
            new ReconciliationGetDTO(
                r.Id,
                r.CustomerId,
                r.InvoiceId,
                r.Amount,
                r.ReconciliationStatus,
                r.DueDate,
                r.ReconciliationAt
            )).ToListAsync();

        double totalRegs = await queryableCount.CountAsync();
        double totalPages = totalRegs > 0 ? Math.Ceiling(totalRegs / take) : 0;

        var getWithPaginationGetDTO = new GetWithPaginationGetDTO<ReconciliationGetDTO>(reconciliationsGetDTO, (int)totalRegs, (int)totalPages);

        return getWithPaginationGetDTO;
    }

    public async Task<GetWithPaginationGetDTO<ReconciliationGetDTO>> GetReconciliationsByStatusAsync(ReconciliationStatus reconciliationStatus, int skip, int take)
    {
        var queryable = _context.Reconciliation.AsNoTracking().Where(r => r.ReconciliationStatus == reconciliationStatus);
        var queryableCount = queryable;

        queryable = queryable.Skip(skip).Take(take);

        var reconciliationsGetDTO = await queryable.Select(r =>
            new ReconciliationGetDTO(
                r.Id,
                r.CustomerId,
                r.InvoiceId,
                r.Amount,
                r.ReconciliationStatus,
                r.DueDate,
                r.ReconciliationAt
            )).ToListAsync();

        double totalRegs = await queryableCount.CountAsync();
        double totalPages = totalRegs > 0 ? Math.Ceiling(totalRegs / take) : 0;

        var getWithPaginationGetDTO = new GetWithPaginationGetDTO<ReconciliationGetDTO>(reconciliationsGetDTO, (int)totalRegs, (int)totalPages);

        return getWithPaginationGetDTO;
    }
}
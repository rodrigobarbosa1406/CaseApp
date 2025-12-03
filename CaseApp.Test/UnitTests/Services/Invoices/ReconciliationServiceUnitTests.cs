using CaseApp.Data;
using CaseApp.Enums.Invoices;
using CaseApp.Models.Invoices;
using CaseApp.Services.Invoices;
using CaseApp.Utils.ErrorHandler.Commom;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CaseApp.Test.UnitTests.Services.Invoices;

public class ReconciliationServiceUnitTests
{
    private ReconciliationService CreateService(ApplicationDbContext context)
    {
        var loggerMock = new Mock<ILogger<ReconciliationService>>();
        return new ReconciliationService(context, loggerMock.Object);
    }

    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private void SeedData(ApplicationDbContext context)
    {
        var reconciliations = new List<Reconciliation>
        {
            new Reconciliation(Guid.NewGuid(), 1, "INV-001", 100m, ReconciliationStatus.PENDENTE, DateTime.UtcNow.AddDays(10), DateTime.UtcNow),
            new Reconciliation(Guid.NewGuid(), 1, "INV-002", 200m, ReconciliationStatus.CONCILIADO, DateTime.UtcNow.AddDays(5), DateTime.UtcNow),
            new Reconciliation(Guid.NewGuid(), 2, "INV-003", 300m, ReconciliationStatus.DIVERGENTE, DateTime.UtcNow.AddDays(3), DateTime.UtcNow),
            new Reconciliation(Guid.NewGuid(), 2, "INV-004", 400m, ReconciliationStatus.ANOMALIA, DateTime.UtcNow.AddDays(1), DateTime.UtcNow),
            new Reconciliation(Guid.NewGuid(), 3, "INV-005", 500m, ReconciliationStatus.FRAUDE, DateTime.UtcNow.AddDays(7), DateTime.UtcNow)
        };

        context.Reconciliation.AddRange(reconciliations);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetReconciliationsAsync_ShouldReturnPaginatedResults()
    {
        var context = CreateInMemoryContext();
        SeedData(context);
        var service = CreateService(context);

        var result = await service.GetReconciliationsAsync(0, 2);

        Assert.Equal(2, result.ObjectClass.Count);
        Assert.Equal(5, result.TotalRegs);
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public async Task GetReconciliationByIdAsync_ShouldReturnDTO_WhenExists()
    {
        var context = CreateInMemoryContext();
        var reconciliation = new Reconciliation(Guid.NewGuid(), 1, "INV-010", 150m, ReconciliationStatus.CONCILIADO, DateTime.UtcNow, DateTime.UtcNow);
        context.Reconciliation.Add(reconciliation);
        context.SaveChanges();

        var service = CreateService(context);
        var result = await service.GetReconciliationByIdAsync(reconciliation.Id);

        Assert.Equal(reconciliation.Id, result.Id);
        Assert.Equal("Conciliado", result.ReconciliationStatus);
    }

    [Fact]
    public async Task GetReconciliationByIdAsync_ShouldThrowNotFound_WhenNotExists()
    {
        var context = CreateInMemoryContext();
        var service = CreateService(context);

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetReconciliationByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetReconciliationsByCustomerIdAsync_ShouldReturnOnlyCustomerRecords()
    {
        var context = CreateInMemoryContext();
        SeedData(context);
        var service = CreateService(context);

        var result = await service.GetReconciliationsByCustomerIdAsync(1, 0, 10);

        Assert.All(result.ObjectClass, r => Assert.Equal(1, r.CustomerId));
        Assert.Equal(2, result.TotalRegs);
    }

    [Fact]
    public async Task GetReconciliationsByInvoiceIdAsync_ShouldReturnOnlyMatchingInvoice()
    {
        var context = CreateInMemoryContext();
        SeedData(context);
        var service = CreateService(context);

        var result = await service.GetReconciliationsByInvoiceIdAsync("INV-003", 0, 10);

        Assert.Single(result.ObjectClass);
        Assert.Equal("INV-003", result.ObjectClass[0].InvoiceId);
    }

    [Fact]
    public async Task GetReconciliationsByStatusAsync_ShouldReturnOnlyMatchingStatus()
    {
        var context = CreateInMemoryContext();
        SeedData(context);
        var service = CreateService(context);

        var result = await service.GetReconciliationsByStatusAsync(ReconciliationStatus.FRAUDE, 0, 10);

        Assert.Single(result.ObjectClass);
        Assert.Equal("Fraude", result.ObjectClass[0].ReconciliationStatus);
    }

}
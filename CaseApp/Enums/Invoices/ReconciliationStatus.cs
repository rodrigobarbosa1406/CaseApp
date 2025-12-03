using System.ComponentModel;

namespace CaseApp.Enums.Invoices;

public enum ReconciliationStatus
{
    [Description("Pendente")]
    PENDENTE = 0,

    [Description("Conciliado")]
    CONCILIADO = 1,

    [Description("Divergente")]
    DIVERGENTE = 2,

    [Description("Anomalia")]
    ANOMALIA = 3,

    [Description("Fraude")]
    FRAUDE = 4
}
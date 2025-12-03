namespace CaseApp.DataTransferObjects.Common;

public class GetWithPaginationGetDTO<T>
{
    public List<T> ObjectClass { get; set; }
    public int TotalRegs { get; set; }
    public int TotalPages { get; set; }

    public GetWithPaginationGetDTO() { }

    public GetWithPaginationGetDTO(List<T> objectClass, int totalRegs, int totalPages)
    {
        ObjectClass = objectClass;
        TotalRegs = totalRegs;
        TotalPages = totalPages;
    }
}
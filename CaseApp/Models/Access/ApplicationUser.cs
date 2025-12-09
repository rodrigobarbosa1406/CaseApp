namespace CaseApp.Models.Access;

public class ApplicationUser
{
    public Guid AppId { get; set; }

    public string AppName { get; set; }

    public string AppPasswordHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public ApplicationUser() { }

    public ApplicationUser(string appName, string appPasswordHash)
    {
        AppId = Guid.NewGuid();
        AppName = appName;
        AppPasswordHash = appPasswordHash;
        CreatedAt = DateTime.UtcNow;
    }
}
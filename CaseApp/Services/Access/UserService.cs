using CaseApp.Data;
using CaseApp.Interfaces.Access;
using CaseApp.Utils.ErrorHandler.Commom;
using Microsoft.EntityFrameworkCore;

namespace CaseApp.Services.Access;

public class UserService(ApplicationDbContext context) : IUserService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<dynamic> FindAsync(string userName)
    {
        var queryable = _context.Users.AsNoTracking().Where(u => u.UserName == userName);
        var user = await queryable.FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException($"O usuário {userName} não existe");
        else
            return user;
    }
}
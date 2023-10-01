using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Enums;
using TestTask.Models;
using TestTask.Services.Interfaces;
namespace TestTask.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context) => _context = context;

    public async Task<User> GetUser()
    {
        var userWithOrdersCount = await _context.Orders
            .GroupBy(o => o.UserId)
            .Select(u => new
            {
                UserId = u.Key,
                OrdersCount = u.Count()
            })
            .OrderByDescending(u => u.OrdersCount)
            .FirstOrDefaultAsync();

        var user = await _context.Users
            .Where(u => u.Id.Equals(userWithOrdersCount.UserId))
            .FirstOrDefaultAsync();
        
        return user;
    }

    public async Task<List<User>> GetUsers() =>
        await _context.Users
            .Where(u => u.Status.Equals(UserStatus.Inactive))
            .ToListAsync();
}
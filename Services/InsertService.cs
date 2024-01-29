using InsertUser;
using Data;

public class InsertUserService : IinsertUserService
{
    // private readonly UserManager<User> _userManager;

    public InsertUserService()
    {
        // _userManager = userManager;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<bool> InsertUserAsync(string username)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var user = new User { Username = username };
        // var result = await _userManager.CreateAsync(user);

        return true;
    }
}
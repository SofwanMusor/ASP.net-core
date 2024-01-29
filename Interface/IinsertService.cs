namespace InsertUser;
public interface IinsertUserService
{
    Task<bool> InsertUserAsync(string username);
}

namespace AuthInterfaces;
public interface IAuthService
{
   string GenerateToken(string boyRow);

   Task<bool> InsertUserAsync(string username);
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Data;
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }
    public string? Username { get; set; }
}

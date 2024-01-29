using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Product;


public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<ProductCD> Products { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductCD>().ToTable("Products"); // ตั้งชื่อตารางเป็น "Products" ถ้าคุณต้องการชื่อแตกต่าง

        // อื่น ๆ ตามความต้องการ เช่น กำหนด Primary Key, Foreign Key, Index และอื่น ๆ
    }
}


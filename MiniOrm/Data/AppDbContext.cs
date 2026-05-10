using MiniOrm.Data;
using MiniOrm.Models;

namespace MiniOrm;

/*
zero logic here we simply list the tables we want (DbSet<Product>, DbSet<Order>) and pass the connection string to the engine
*/
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(string connStr) : base(connStr)
    {
     
    }
}
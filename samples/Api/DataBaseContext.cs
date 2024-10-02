namespace WebApi;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataBaseContext : DbContext
{
	public DataBaseContext(DbContextOptions<DataBaseContext> options)
		: base(options)
	{
	}

	public DbSet<Product> Products { get; set; }
}

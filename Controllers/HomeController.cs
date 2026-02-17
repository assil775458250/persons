using Microsoft.AspNetCore.Mvc;
using Npgsql;

public class HomeController : Controller
{

    private readonly IConfiguration _configuration;

    public HomeController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var products = new List<Product>();

        var connString = _configuration.GetConnectionString("DefaultConnection");

        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand("SELECT * FROM students", conn);
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Price = reader.GetString(2)
            });
        }

        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string name, string email)
    {
        var connString = _configuration.GetConnectionString("DefaultConnection");

        using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(
     "INSERT INTO students (full_name, email) VALUES (@full_name, @email)", conn);

        cmd.Parameters.AddWithValue("full_name", name);
        cmd.Parameters.AddWithValue("email", email);

        await cmd.ExecuteNonQueryAsync();

        return RedirectToAction("Index");
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Price { get; set; }
}

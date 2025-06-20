using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KindergartenAPI.Data;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    // Indica que esta instancia corre en el entorno 'Test'
    builder.UseEnvironment("Test");

    builder.ConfigureServices(services =>
    {
        // Asegurando que la DB en memoria esté limpia
        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<KindergartenContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    });
}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebJobChollometro.Data;
using WebJobChollometro.Repositories;

string connectionString = "Data Source=sqlluismi.database.windows.net;Initial Catalog=AZURETAJAMAR;Persist Security Info=True;User ID=adminsql;Password=Admin123;Encrypt=True;Trust Server Certificate=True";

Console.WriteLine("Bienvenido al Chollometro WebJob!");
//necesitamos la inyeccion de dependencias
var provider = new ServiceCollection()
    .AddDbContext<ChollometroContext>(options => options.UseSqlServer(connectionString))
    .AddTransient<RepositoryChollometro>()
    .BuildServiceProvider();
//recuperamos el repository de la inyeccion
RepositoryChollometro repository = provider.GetService<RepositoryChollometro>();

await repository.PopulateChollosAzureAsync();

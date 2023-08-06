using MongoDB.Driver;
using Newtonsoft.Json;
using Webscrapper.API;

namespace Webscrapper.Database;

public class DatabaseInitializer
{
    private DBContext _context;

    public DatabaseInitializer()
    {
        var appSettings =
            JsonConvert.DeserializeObject<WebscrapperAppSettings>(File.ReadAllText("appsettings.json"));
        _context = new DBContext(appSettings.ConnectionString);
    }

    public DBContext GetContext() => _context;
}
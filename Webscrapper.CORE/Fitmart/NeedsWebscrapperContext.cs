using Webscrapper.Database;

namespace Webscrapper.CORE.Fitmart;

public class NeedsWebscrapperContext
{
    public DBContext _context;
    public NeedsWebscrapperContext(DatabaseInitializer databaseInitializer)
    {
        _context = databaseInitializer.GetContext();
    }
}
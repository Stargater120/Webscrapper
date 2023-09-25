using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR;

public class NeedsDBContext
{
    protected readonly DBContext _dbContext;
    public NeedsDBContext(DatabaseInitializer databaseInitializer)
    {
        _dbContext = databaseInitializer.GetContext();
    }
}
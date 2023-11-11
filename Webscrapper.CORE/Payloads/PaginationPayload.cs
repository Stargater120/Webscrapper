namespace Webscrapper.CORE.Payloads;

public class PaginationPayload<T>
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public List<T> List { get; set; }
}
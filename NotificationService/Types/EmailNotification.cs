namespace NotificationService.Types;

public class EmailNotification
{
    public List<byte[]> Banner { get; set; } = new List<byte[]>();
    public List<Item> Items { get; set; } = new List<Item>();
}

public class Item
{
    public string Name { get; set; }
    public double NewPrice { get; set; }
    public double OldPrice { get; set; }
}
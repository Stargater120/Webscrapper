﻿namespace Webscrapper.Database.Models;

public class Subscription
{
    public Guid Id { get; set; }
    public string SearchTerm { get; set; }
    public string SiteName { get; set; }
    public string Url { get; set; }
}
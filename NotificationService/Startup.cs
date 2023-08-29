using Microsoft.Extensions.DependencyInjection;
using NotificationService.Email;

namespace NotificationService;

public static class Startup
{
    public static IServiceCollection RegisterNotificationServices(this IServiceCollection service)
    {
        var noti = new EmailService();
        // noti.SendEmail();
        return service;
    }
}
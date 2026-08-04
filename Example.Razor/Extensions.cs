using Avae.Razor;
using Avae.Services;
using Example.Razor.Pages;
using Example.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Razor
{
    public static class Extensions
    {
        public static void ConfigureProject(this IServiceCollection services,
            NotificationPosition position = NotificationPosition.BottomLeft,
            int maxDispayments = 5)
        {
            services.ConfigureBase(position, maxDispayments, container =>
            {
                container.Register(nameof(ModalViewModel), (sp, obj) =>
                {
                    return typeof(ModalView);
                });
            });
            services.AddSingleton<HomeViewModel>();
            services.AddTransient<ModalViewModel>();
        }
    }
}

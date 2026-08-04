using Avae.DAL;
using Avae.Razor;
using Avae.Services;
using Example.Models;
using Example.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Example.Razor
{
    public static class Extensions
    {
        public static void ConfigureProject(this IServiceCollection services,
            NotificationPosition position = NotificationPosition.BottomLeft,
            int maxDispayments = 5)
        {
            services.ConfigureBase(
                new ComponentView<NavMenu>(),
                position, maxDispayments, container =>
            {
                container.Register<ComponentView<ModalView, ModalViewModel>>();
                container.Register<ComponentView<FormView, FormViewModel>>();
            });
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<MenuViewModel>();
            services.AddTransient<ModalViewModel>();
            services.AddTransient<FormViewModel>();

            services.UseDbLayer<IDBLayer>(sp => new DBSqlLayer(sp));

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "database.db");
            var connectionString = $"Data Source={dbPath};Foreign Keys=True";
            services.AddTransient<IDbConnection>(_ => new SqliteConnection(connectionString));
        }
    }
}

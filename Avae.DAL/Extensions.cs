using Avae.DAL.Interfaces;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MagicOnion;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;

namespace Avae.DAL
{
    public static class Extensions
    {
        public static void  UseSqlMonitors<TDBConnection>(this IServiceCollection services,
            string connectionString, Action<SqlFactory<TDBConnection>>? action = null, bool isTransaction = false)
            where TDBConnection : DbConnection, new()
        {
            var factory = new SqlFactory<TDBConnection>(connectionString, isTransaction);

            action?.Invoke(factory);
            services.AddSingleton<IDbFactory>(sp => factory);
            services.AddTransient<IDbConnection>(_ => factory.CreateConnection()!);
        }

        public static void UseDbLayer<IDbLayer>(this IServiceCollection services, Func<IServiceProvider, IDbLayer> getLayer)
            where IDbLayer : class, IDataAccessLayer
        {
            services.AddSingleton<IDbLayer>(sp => getLayer(sp));
            services.AddSingleton<IDataAccessLayer>(sp => sp.GetRequiredService<IDbLayer>());
        }

        public static IGrpc GetMagicOnion<IGrpc>(this IServiceProvider provider, string url,
            HttpClient? httpClient = null) where IGrpc : IService<IGrpc>
        {
            var client = httpClient ?? new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler()))
            {
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
                DefaultRequestVersion = HttpVersion.Version20,
                Timeout = TimeSpan.FromSeconds(5)
            };
            var channel = GrpcChannel.ForAddress(
                url
                , new GrpcChannelOptions()
                {
                    HttpClient = client,
                });
            return MagicOnionClient.Create<IGrpc>(channel);
        }

        internal static string ReplaceWholeWord(this string s, string word, string bywhat)
        {
            char firstLetter = word[0];
            var sb = new StringBuilder();
            bool previousWasLetterOrDigit = false;
            int i = 0;
            while (i < s.Length - word.Length + 1)
            {
                bool wordFound = false;
                char c = s[i];
                if (c == firstLetter)
                    if (!previousWasLetterOrDigit)
                        if (s.Substring(i, word.Length).Equals(word))
                        {
                            wordFound = true;
                            bool wholeWordFound = true;
                            if (s.Length > i + word.Length)
                            {
                                if (char.IsLetterOrDigit(s[i + word.Length]))
                                    wholeWordFound = false;
                            }

                            if (wholeWordFound)
                                sb.Append(bywhat);
                            else
                                sb.Append(word);

                            i += word.Length;
                        }

                if (!wordFound)
                {
                    previousWasLetterOrDigit = char.IsLetterOrDigit(c);
                    sb.Append(c);
                    i++;
                }
            }

            if (s.Length - i > 0)
                sb.Append(s.AsSpan(i));

            return sb.ToString();
        }
    }
}

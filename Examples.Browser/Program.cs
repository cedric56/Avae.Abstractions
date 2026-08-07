using Avalonia;
using Avalonia.Browser;
using Example;
using Example.Models;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

internal sealed partial class Program
{
    private static Task Main(string[] args)
    {
        try
        {
            Console.WriteLine("=== Application START ===");

            // Initialize MessagePack FIRST
            InitializeMessagePack();
            Console.WriteLine("MessagePack initialized");

            // Initialize JSON serializer
            InitializeJsonSerializer();
            Console.WriteLine("JSON serializer initialized");

            // Build and run the app
            return BuildAvaloniaApp()
                .StartBrowserAppAsync("out");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FATAL ERROR: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }
        //BuildAvaloniaApp().WithInterFont().StartBrowserAppAsync("out");

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<BrowserApp>();
        //.UseReactiveUI(() => { });


    public class BrowserApp : App
    {
        protected override string Logs => string.Empty;

        public override void Configure(IServiceCollection services)
        {
            base.Configure(services);

            services.UseDBOnionLayer(out _, out _);
        }
    }

    private static void InitializeMessagePack()
    {
        try
        {
            Console.WriteLine("MessagePackConfig: Initializing...");

            // Create a resolver that works in WebAssembly
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                // Use the contractless resolver for dynamic types
                MessagePack.Resolvers.ContractlessStandardResolver.Instance,
                // Use standard resolver for known types
                MessagePack.Resolvers.StandardResolver.Instance,
                // Built-in resolver
                MessagePack.Resolvers.BuiltinResolver.Instance
            );

            MessagePackSerializer.DefaultOptions = MessagePackSerializerOptions.Standard
                .WithResolver(resolver)
                .WithCompression(MessagePackCompression.Lz4Block)
                .WithOmitAssemblyVersion(true); // Important for WASM

            Console.WriteLine("MessagePackConfig: Initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MessagePackConfig: ERROR - {ex.Message}");
            throw;
        }
    }

    private static void InitializeJsonSerializer()
    {
        try
        {
            Console.WriteLine("JsonConfig: Initializing...");

            // Configure System.Text.Json for WebAssembly
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false,
                // Use reflection-based serializer
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };

            // Register any custom converters
            options.Converters.Add(new JsonStringEnumConverter());

            Console.WriteLine("JsonConfig: Initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JsonConfig: ERROR - {ex.Message}");
            throw;
        }
    }
}

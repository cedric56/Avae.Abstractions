using Avae.DAL;

namespace Example.DAL;

public static class Constants
{
    public static string ServerUrl = "https://88.165.230.223:17001";
    public static string MagicHubUrl = $"{ServerUrl}/recordHubOfPerson";
    public static string SignalHubUrl = $"{ServerUrl}/PersonHub";
    public static string OnionUrl = $"{ServerUrl}/{typeof(IMagicOnionLayer).Name}/";
}

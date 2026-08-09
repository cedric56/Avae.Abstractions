using System.Runtime.InteropServices.JavaScript;

namespace Microsoft.Maui.Essentials
{
    public static class Platform
    {
        public static Task UseEssentials(string projectName)
        {
            if (OperatingSystem.IsBrowser())
                return JSHost.ImportAsync("essentials", $"/_content/{projectName}/essentials.js");
            return Task.CompletedTask;
        }
    }
}

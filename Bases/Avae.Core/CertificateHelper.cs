using System.Reflection;
using System.Security.Cryptography.X509Certificates;
namespace Avae.Core;
public static class CertificateHelper
{
    /// <summary>
    /// Charge un certificat depuis une ressource embarquée
    /// </summary>
    public static X509Certificate2 LoadCertificateFromResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Récupérer toutes les ressources pour déboguer
        var allResources = assembly.GetManifestResourceNames();
        // Le nom de la ressource est généralement : "NomAssembly.NomDossier.NomFichier.extension"
        // Exemple: "MonApp.Assets.Certificats.server.crt"

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"La ressource '{resourceName}' est introuvable.");
        }

        // Charger le certificat depuis le flux
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var certBytes = memoryStream.ToArray();

        // Créer un certificat X509 (sans clé privée pour un .crt)
        return X509CertificateLoader.LoadCertificate(certBytes);
    }

    /// <summary>
    /// Obtient le nom complet de la ressource en fonction de son chemin
    /// </summary>
    public static string GetResourceName(string filePath)
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        return $"{assemblyName}.{filePath.Replace("/", ".").Replace("\\", ".")}";
    }
}

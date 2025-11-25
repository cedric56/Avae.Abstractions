using Avae.Abstractions;
using System.Runtime.CompilerServices;

namespace Example.Models
{
    public static class ModelInitialization
    {
#pragma warning disable CA2255 // L’attribut ’ModuleInitializer’ ne doit pas être utilisé dans les bibliothèques
        [ModuleInitializer]
#pragma warning restore CA2255 // L’attribut ’ModuleInitializer’ ne doit pas être utilisé dans les bibliothèques
        public static void Init()
        {
            InputValidation<Person>.Init();
        }
    }
}

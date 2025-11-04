using Avae.Abstractions;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Example.ViewModels
{
    internal partial class FormPage1ViewModel : CloseableViewModelBase<string>
    {
        public string Title => "Welcome to page 1";
    }
}

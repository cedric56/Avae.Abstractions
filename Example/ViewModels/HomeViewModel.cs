using Avae.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Example.ViewModels
{
    internal class HomeViewModel : ObservableObject, IViewModelBase
    {
        public string Title => "Welcome to home";
    }
}

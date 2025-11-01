using CommunityToolkit.Mvvm.ComponentModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to represent a page in the application.
    /// </summary>
    /// <param name="viewModelType"></param>
    /// <param name="displayName"></param>
    /// <param name="icon"></param>
    public class PageViewModelBase(Type viewModelType, string displayName, string icon) : ObservableObject, IViewModelBase
    {
        public Type ViewModelType { get; } = viewModelType;
        public string DisplayName { get; } = displayName;
        public string Icon { get; } = icon;

        public bool SetDataContext { get; set; } = true;

        public object[] ViewParameters { get; set; } = [];

        public object[] ViewModelParameters { get; set; } = [];
    }

    public class PageViewModelBase<T>(string displayName, string icon) : 
        PageViewModelBase(typeof(T), displayName, icon), IViewModelBase where T : IViewModelBase
    {
        
    }
}


namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to represent a page in the application.
    /// </summary>
    /// <param name="viewModelType"></param>
    /// <param name="displayName"></param>
    /// <param name="icon"></param>
    public class PageViewModelBase(Type viewModelType, string displayName, string icon) : IViewModelBase
    {
        public Func<IViewModelBase, Task>? Launched { get; set; }
        public IViewModelBase? ViewModel { get; protected set; }
        public Type ViewModelType { get; } = viewModelType;
        public string DisplayName { get; } = displayName;
        public string Icon { get; } = icon;

        public IParameter[] FactoryParameters { get; set; } = [];
        public IParameter[] ViewParameters { get; set; } = [];
        public IParameter[] ViewModelParameters { get; set; } = [];

        public IParameter[]  Parameters
        {
            get
            {
                var parameters = new  List<IParameter>();
                parameters.AddRange(FactoryParameters);
                parameters.AddRange(ViewParameters);
                parameters.AddRange(ViewModelParameters);
                return [.. parameters];
            }
        }

        public virtual Task OnLaunched(IViewModelBase viewModel)
        {
            if (Launched == null)
                return Task.CompletedTask;

            return Launched(viewModel);
        }
    }

    public class PageViewModelBase<T> : PageViewModelBase where T : IViewModelBase
    {
        public new Func<T, Task>? Launched { get; set; }

        public PageViewModelBase(string displayName, string icon)
            : base(typeof(T), displayName, icon)
        {

        }

        public PageViewModelBase(T viewModel, string displayName, string icon)
            : base(typeof(T), displayName, icon)
        {
            ViewModel = viewModel;
        }

        public override Task OnLaunched(IViewModelBase viewModel)
        {
            if (Launched == null)
                return Task.CompletedTask;

            return Launched((T)viewModel);
        }
    }
}

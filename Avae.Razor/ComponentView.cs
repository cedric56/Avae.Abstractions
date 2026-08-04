using Avae.Abstractions;

namespace Avae.Razor
{
    public abstract class ComponentView
    {       
        public abstract Type Type { get; }

        public IDictionary<string, object>? Parameters { get; protected set; }
    }

    public class ComponentView<TView> : ComponentView
    {
        public override Type Type => typeof(TView);
    }

    public class ComponentView<TView, TViewModel> : ComponentView, IContextFor<TViewModel> where TViewModel : class, IViewModelBase
    {
        private object? _context;
        public object? Context { get => _context; set { _context = value; OnContextChanged(_context); } }


        public ComponentView()
        {
            
        }

        public ComponentView(IServiceProvider sp, NavigationContext? context = null, Dictionary<string, object>? parameters = null)
        {
            var viewModel = sp.GetViewModel<TViewModel>(context);            
            Parameters = new Dictionary<string, object>(parameters ?? [])
            {
                { "ViewModel", viewModel }
            };
        }

        public override Type Type => typeof(TView);

        protected void OnContextChanged(object? context)
        {
            if (context is null || Parameters is not null)
                return;

            Parameters = new Dictionary<string, object>()
            {
                { "ViewModel", (TViewModel)context! }
            };
        }
    }
}

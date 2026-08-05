using Avae.Abstractions;

namespace Avae.Razor.Components
{
    public abstract class ComponentView
    {
        public virtual bool IsCenter => false;

        public abstract Type Type { get; }

        public IDictionary<string, object>? Parameters { get; set; }
    }

    public class ComponentView<TView> : ComponentView
    {
        public override Type Type => typeof(TView);
    }

    public class CenteredComponentView<TView> : ComponentView
    {
        public override Type Type => typeof(TView);

        public override bool IsCenter => true;
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

    public class CenteredComponentView<TView, TViewModel> : 
        ComponentView<TView, TViewModel> 
        where TViewModel : class, IViewModelBase
    {
        public CenteredComponentView()
        {

        }

        public CenteredComponentView(IServiceProvider sp, NavigationContext? context = null, Dictionary<string, object>? parameters = null)
            : base(sp, context, parameters)
        {
        }

        public override bool IsCenter => true;
    }
}

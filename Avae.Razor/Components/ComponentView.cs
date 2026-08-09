using Avae.Abstractions;
using Microsoft.AspNetCore.Components;

namespace Avae.Razor.Components
{
    public abstract class ComponentView
    {
        public virtual string Class => string.Empty;

        public abstract Type Type { get; }

        public IDictionary<string, object>? Parameters { get; set; }
    }

    public class ComponentView<TView> : ComponentView
    {
        public ComponentView()
        {

        }

        public ComponentView(object content)
        {
            var fragment = new RenderFragment(tree => tree.AddContent(0, content));
            Parameters = new Dictionary<string, object>() { { "ChildContent", fragment } };
        }

        public override Type Type => typeof(TView);
    }

    public class CenteredComponentView<TView> : ComponentView<TView>
    {
        public CenteredComponentView(object content)
            : base(content)
        {

        }

        public override string Class => "center";
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

        public override string Class => "center";
    }
}

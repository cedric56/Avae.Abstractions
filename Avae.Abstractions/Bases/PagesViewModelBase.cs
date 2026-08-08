using Avae.Abstractions.Bases;
using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : RouterViewModelBase, IViewModelBase 
    {
        public EventHandler<IContextFor>? ContextForChanged;

        protected override void OnViewModelChanged(IViewModelBase viewModel)
        {
            var type = viewModel.GetType();
            _selectedPage = Pages.First(p => p.ViewModelType == type);
            if (dico.TryGetValue(_selectedPage, out var context))
            {
                _contextFor = context.Key;
            }
            NotifyPropertyChanged(nameof(SelectedPage));
            NotifyPropertyChanged(nameof(ContextFor));
            ContextForChanged?.Invoke(this, _contextFor);
            base.OnViewModelChanged(viewModel);
        }

        protected abstract void NotifyPropertyChanged(string propertyName);


        /// <summary>
        /// A dictionary to store the context for each page.
        /// </summary>
        private readonly Dictionary<PageViewModelBase, KeyValuePair<IContextFor, IViewModelBase>> dico = [];

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        private IContextFor _contextFor = null!;
        public IContextFor ContextFor 
        { 
            get { return _contextFor; } 
            set 
            { 
                _contextFor = value;
                NotifyPropertyChanged(nameof(ContextFor));
                ContextForChanged?.Invoke(this, _contextFor);
            } 
        }

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        private PageViewModelBase? _selectedPage;
        public PageViewModelBase? SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                _selectedPage = value;
                OnSelectedPageChanged(value);
                NotifyPropertyChanged(nameof(SelectedPage));
            }
        }

        public PagesViewModelBase(Router router, bool initialize = true)
            : base(router)
        {
            if (initialize)
            {
                SelectedPage = Pages.FirstOrDefault();
            }
        }

        private ObservableCollection<PageViewModelBase>? _pages;
        /// <summary>
        /// The list of pages to be displayed in the menu.
        /// </summary>
        public ObservableCollection<PageViewModelBase> Pages { get {return _pages ??= GetPages(); } }

        protected abstract ObservableCollection<PageViewModelBase> GetPages();

        /// <summary>
        /// This method is called when the selected page changes.
        /// </summary>
        /// <param name="value"></param>
        protected async void OnSelectedPageChanged(PageViewModelBase? value)
        {
            if (value == null)
                return;

            if (dico.TryGetValue(value, out var context))
            {
                ContextFor = context.Key;
                _router.AddHistory(context.Value);
            }
            else
            {
                var contextFor = GoTo(value, out var viewModel);                  
                dico.Add(value, new KeyValuePair<IContextFor, IViewModelBase>(contextFor, viewModel));
                await value.OnLaunched(viewModel);
                ContextFor = contextFor;
            }

            RaiseCanExecuteChanged();
        }

        protected virtual IContextFor GoTo(PageViewModelBase value, out IViewModelBase viewModel)
        {
            IContextFor contextFor;
            if (value.ViewModel != null)
            {
                contextFor = _router.GoTo(viewModel = value.ViewModel, value.NavigationContext);
            }
            else
            {
                contextFor = _router.GoTo(value.ViewModelType, out viewModel, value.NavigationContext);
            }

            return contextFor;
        }
    }
}

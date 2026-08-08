using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : IViewModelBase 
    {
        public EventHandler<IContextFor>? CurrentPageChanged;

        protected virtual void OnViewModelChanged(IViewModelBase viewModel)
        {
            var type = viewModel.GetType();
            _selectedPage = Pages.First(p => p.ViewModelType == type);
            if (dico.TryGetValue(_selectedPage, out var context))
            {
                _currentPage = context.Key;
            }
            NotifyPropertyChanged(nameof(SelectedPage));
            NotifyPropertyChanged(nameof(CurrentPage));
            CurrentPageChanged?.Invoke(this, _currentPage);
            RaiseCanExecuteChanged();
        }

        protected abstract void RaiseCanExecuteChanged();

        protected abstract void NotifyPropertyChanged(string propertyName);

        public virtual void GoBack()
        {
            if (CanGoBack())
            {
                var viewModel = _router.Back()!;
                OnViewModelChanged(viewModel);
            }
        }

        public bool CanGoBack()
        {
            return _router.CanGoBack;
        }

        public virtual void GoForward()
        {
            if (CanGoForward())
            {
                var viewModel = _router.Forward()!;
                OnViewModelChanged(viewModel);
            }            
        }

        public bool CanGoForward()
        {
            return _router.CanGoForward;
        }


        /// <summary>
        /// A dictionary to store the context for each page.
        /// </summary>
        private readonly Dictionary<PageViewModelBase, KeyValuePair<IContextFor, IViewModelBase>> dico = [];

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        private IContextFor _currentPage = null!;
        public IContextFor CurrentPage 
        { 
            get { return _currentPage; } 
            set 
            { 
                _currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
                CurrentPageChanged?.Invoke(this, _currentPage);
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

        /// <summary>
        /// The router used to navigate between pages.
        /// </summary>
        protected Router _router;

        public PagesViewModelBase(Router router, bool initialize = true)
        {
            _router = router;

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
                CurrentPage = context.Key;
                _router.AddHistory(context.Value);
            }
            else
            {
                var page = GoTo(value, out var viewModel);  
                await value.OnLaunched(viewModel);                
                dico.Add(value, new KeyValuePair<IContextFor, IViewModelBase>(page, viewModel));

                CurrentPage = page;
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

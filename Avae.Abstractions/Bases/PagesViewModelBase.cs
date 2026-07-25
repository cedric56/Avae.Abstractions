using Avae.Abstractions.Commands;
using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : IViewModelBase 
    {
        private void OnViewModelChanged(IViewModelBase viewModel)
        {
            var type = viewModel.GetType();
            _selectedPage = Pages.First(p => p.ViewModelType == type);
            if (dico.TryGetValue(_selectedPage, out var context))
            {
                _currentPage = context.Key;
            }
            NotifyPropertyChanged(nameof(SelectedPage));
            NotifyPropertyChanged(nameof(CurrentPage));
            BackCommand.RaiseCanExecuteChanged();
            ForwardCommand.RaiseCanExecuteChanged();
        }
        protected abstract void NotifyPropertyChanged(string propertyName);

        private AsyncRelayCommand? _backCommand;


        public AsyncRelayCommand BackCommand
        {
            get
            {
                return _backCommand ??= new AsyncRelayCommand(() =>
                {
                    var viewModel = _router.Back()!;
                    OnViewModelChanged(viewModel);
                    return Task.CompletedTask;

                }, () => _router.CanGoBack);
            }            
        }

        private AsyncRelayCommand? _forwardCommand;


        public AsyncRelayCommand ForwardCommand
        {
            get
            {
                return _forwardCommand ??= new AsyncRelayCommand(() =>
                {
                    var viewModel = _router.Forward()!;
                    OnViewModelChanged(viewModel);
                    return Task.CompletedTask;

                }, () => _router.CanGoForward);
            }
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
                dico.Add(value, new KeyValuePair<IContextFor, IViewModelBase>(CurrentPage = GoTo(value, out var viewModel), viewModel));
                await value.OnLaunched(viewModel);
            }

            BackCommand.RaiseCanExecuteChanged();
            ForwardCommand.RaiseCanExecuteChanged();
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

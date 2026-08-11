using Avae.Abstractions.Bases;
using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : RouterViewModelBase, IViewModelBase 
    {
        public EventHandler<IViewFor>? CurrentViewChanged;

        protected override void OnViewModelChanged(IViewModelBase viewModel)
        {
            var type = viewModel.GetType();
            _selectedViewModel = ViewModels.First(p => p.ViewModelType == type);
            if (dico.TryGetValue(_selectedViewModel, out var context))
            {
                _currentView = context.Key;
            }
            NotifyPropertyChanged(nameof(SelectedViewModel));
            NotifyPropertyChanged(nameof(CurrentView));
            CurrentViewChanged?.Invoke(this, _currentView);
            base.OnViewModelChanged(viewModel);
        }

        protected abstract void NotifyPropertyChanged(string propertyName);


        /// <summary>
        /// A dictionary to store the context for each page.
        /// </summary>
        private readonly Dictionary<PageViewModelBase, KeyValuePair<IViewFor, IViewModelBase>> dico = [];

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        private IViewFor _currentView = null!;
        public IViewFor CurrentView 
        { 
            get { return _currentView; } 
            set 
            { 
                _currentView = value;
                NotifyPropertyChanged(nameof(CurrentView));
                CurrentViewChanged?.Invoke(this, _currentView);
            } 
        }

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        private PageViewModelBase? _selectedViewModel;
        public PageViewModelBase? SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnSelectedViewModelChanged(value);
                NotifyPropertyChanged(nameof(SelectedViewModel));
            }
        }

        public PagesViewModelBase(Router router, bool initialize = true)
            : base(router)
        {
            if (initialize)
            {
                SelectedViewModel = ViewModels.FirstOrDefault();
            }
        }

        private ObservableCollection<PageViewModelBase>? _viewModels;
        /// <summary>
        /// The list of pages to be displayed in the menu.
        /// </summary>
        public ObservableCollection<PageViewModelBase> ViewModels { get {return _viewModels ??= GetViewModels(); } }

        protected abstract ObservableCollection<PageViewModelBase> GetViewModels();

        /// <summary>
        /// This method is called when the selected page changes.
        /// </summary>
        /// <param name="value"></param>
        protected async void OnSelectedViewModelChanged(PageViewModelBase? value)
        {
            if (value == null)
                return;

            if (dico.TryGetValue(value, out var view))
            {
                CurrentView = view.Key;
                _router.AddHistory(view.Value);
            }
            else
            {
                var viewFor = GoTo(value, out var viewModel);                  
                dico.Add(value, new KeyValuePair<IViewFor, IViewModelBase>(viewFor, viewModel));
                await value.OnLaunched(viewModel);
                CurrentView = viewFor;
            }

            RaiseCanExecutesChanged();
        }

        protected virtual IViewFor GoTo(PageViewModelBase value, out IViewModelBase viewModel)
        {
            IViewFor viewFor;
            if (value.ViewModel != null)
            {
                viewFor = _router.GoTo(viewModel = value.ViewModel, value.NavigationContext);
            }
            else
            {
                viewFor = _router.GoTo(value.ViewModelType, out viewModel, value.NavigationContext);
            }

            return viewFor;
        }
    }
}

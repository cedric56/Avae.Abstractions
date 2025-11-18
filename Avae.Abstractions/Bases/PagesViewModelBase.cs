#nullable disable
using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : ViewModelBase 
    {
        protected abstract void NotifyPropertyChanged(string propertyName);

        /// <summary>
        /// A dictionary to store the context for each page.
        /// </summary>
        private readonly Dictionary<PageViewModelBase, IContextFor> dico = [];

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
        private PageViewModelBase _selectedPage;
        public PageViewModelBase SelectedPage
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
                var page = Pages.FirstOrDefault();
                if (page != null)
                {
                    OnSelectedPageChanged(page);
                }
            }
        }

        /// <summary>
        /// The list of pages to be displayed in the menu.
        /// </summary>
        public abstract ObservableCollection<PageViewModelBase> Pages { get; }

        /// <summary>
        /// This method is called when the selected page changes.
        /// </summary>
        /// <param name="value"></param>
        protected async void OnSelectedPageChanged(PageViewModelBase value)
        {
            if (value == null)
                return;

            if (dico.TryGetValue(value, out var context))
            {
                CurrentPage = context;
            }
            else
            {
                dico.Add(value, CurrentPage = await GoTo(value));
            }
        }

        protected virtual async Task<IContextFor> GoTo(PageViewModelBase value)
        {
            IContextFor contextFor = null;
            if (value.ViewModel != null)
            {
                contextFor = await _router.GoTo(value.ViewModel, value.Parameters);
            }
            else
            {
                contextFor = (await _router.GoTo(value.ViewModelType, value.Parameters)).context;
            }

            return contextFor;
        }
    }
}

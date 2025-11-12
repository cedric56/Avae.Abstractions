#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Avae.Abstractions
{
    /// <summary>
    /// This class is used to manage the pages in the application.
    /// </summary>
    public abstract partial class PagesViewModelBase : ObservableObject, IViewModelBase
    {
        /// <summary>
        /// A dictionary to store the context for each page.
        /// </summary>
        private readonly Dictionary<PageViewModelBase, IContextFor> dico = [];

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        [ObservableProperty]
        private IContextFor _currentPage = null!;

        /// <summary>
        /// The currently selected page in the menu.
        /// </summary>
        [ObservableProperty]
        private PageViewModelBase _selectedPage;

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
        partial void OnSelectedPageChanged(PageViewModelBase value)
        {
            if (value == null)
                return;

            if (dico.TryGetValue(value, out var context))
            {
                CurrentPage = context;
            }
            else
            {
                dico.Add(value, CurrentPage = GoTo(value));
            }
        }

        protected virtual IContextFor GoTo(PageViewModelBase value)
        {
            IContextFor contextFor = null;
            if (value.ViewModel != null)
            {
                contextFor = _router.GoTo(value.ViewModel, value.Parameters);
            }
            else
            {
                contextFor = _router.GoTo(value.ViewModelType, value.Parameters);
            }

            return contextFor;
        }

        /// <summary>
        /// This method is called when the user scrolls the menu.
        /// </summary>
        /// <param name="delta"></param>
        [RelayCommand]
        public void OnScrollChanged(object delta)
        {
            if (delta is ValueTuple<double, double> tuple)
            {
                var index = (int)tuple.Item2;

                if (index < 0)
                    index = 0;
                else if (index >= Pages.Count)
                    index = Pages.Count - 1;

                SelectedPage = Pages[index];
            }
        }
    }
}

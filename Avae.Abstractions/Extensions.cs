using System.Text;

namespace Avae.Abstractions
{
    public static class Extensions
    {
        public static Task<TResult?> ShowDialogAsync<TViewModel, TResult>(this IViewModelBase viewModelBase, params IParameter[] parameters) where TViewModel : class, IViewModelBase
        {
            var viewModel = SimpleProvider.GetViewModel<TViewModel>(parameters);
            var container = SimpleProvider.GetService<IIocConfiguration>();
            var view = container?.GetModalFor<TViewModel, TResult>(parameters) ?? throw new InvalidOperationException($"Unable to create view for {typeof(TViewModel).Name}.  Ensure that it is registered in the container.");
            view.DataContext = viewModel;
            return view.ShowDialogAsync();
        }

        public static string ReplaceWholeWord(this string s, string word, string bywhat)
        {
            char firstLetter = word[0];
            var sb = new StringBuilder();
            bool previousWasLetterOrDigit = false;
            int i = 0;
            while (i < s.Length - word.Length + 1)
            {
                bool wordFound = false;
                char c = s[i];
                if (c == firstLetter)
                    if (!previousWasLetterOrDigit)
                        if (s.Substring(i, word.Length).Equals(word))
                        {
                            wordFound = true;
                            bool wholeWordFound = true;
                            if (s.Length > i + word.Length)
                            {
                                if (char.IsLetterOrDigit(s[i + word.Length]))
                                    wholeWordFound = false;
                            }

                            if (wholeWordFound)
                                sb.Append(bywhat);
                            else
                                sb.Append(word);

                            i += word.Length;
                        }

                if (!wordFound)
                {
                    previousWasLetterOrDigit = char.IsLetterOrDigit(c);
                    sb.Append(c);
                    i++;
                }
            }

            if (s.Length - i > 0)
                sb.Append(s.AsSpan(i));

            return sb.ToString();
        }

        public static void Update<X, Y>(this IList<Y> items, IList<X> selectedItems, Func<X, Y, bool> predicate, Func<X, Y> add)
        {
            foreach (var x in selectedItems)
                if (!items.Any(y => predicate(x, y)))
                    items.Add(add(x));

            var deleted = new List<Y>();
            foreach (var item in items)
                if (!selectedItems.Any(x => predicate(x, item)))
                    deleted.Add(item);

            foreach (var item in deleted)
                items.Remove(item);
        }

        public static ViewModelParameter<T> ForViewModel<T>(this T obj)
        {
            return new ViewModelParameter<T>(obj);
        }

        public static ViewParameter<T> ForView<T>(this T obj)
        {
            return new ViewParameter<T>(obj);
        }

        public static FactoryParameter<T> ForFactory<T>(this T obj)
        {
            return new FactoryParameter<T>(obj);
        }
    }
}

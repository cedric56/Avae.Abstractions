using Avae.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Avae.Abstractions;

public abstract class ViewFor
{
    public string Class { get; set; } = string.Empty;

    public abstract Type Type { get; }

    public IDictionary<string, object>? Parameters { get; set; }
}

public class ViewFor<TView> : ViewFor
{
    public ViewFor()
    {

    }

    public ViewFor(object content)
    {
        var fragment = new RenderFragment(tree => tree.AddContent(0, content));
        Parameters = new Dictionary<string, object>() { { "ChildContent", fragment } };
    }

    public override Type Type => typeof(TView);
}

public class ViewFor<TView, TViewModel> : ViewFor, IViewFor<TViewModel> where TViewModel : class, IViewModelBase
{
    private object? _context;
    public object? Context { get => _context; set { _context = value; OnContextChanged(_context); } }


    public ViewFor()
    {

    }

    public ViewFor(IServiceProvider sp, NavigableContext? context = null, Dictionary<string, object>? parameters = null)
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

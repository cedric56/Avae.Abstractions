using Avae.ViewModels.Tests.TestDoubles;
using Microsoft.Extensions.DependencyInjection;

namespace Avae.ViewModels.Tests;

/// <summary>
/// Note on the <c>Register&lt;TContextFor, TArg1, ...&gt;</c> overloads: their bodies call the
/// single-argument <c>Register(Func&lt;IServiceProvider, NavigableContext, TContextFor&gt;)</c> overload
/// (it's the only one whose type parameter is inferable from that call site), so the positional
/// arguments they expose are actually read positionally off a single <see cref="NavigableContext"/>
/// via <see cref="NavigableContext.Get{T}"/> - not off a raw <c>object[]</c>. That's why these tests
/// call <c>GetView</c> with a one-element array containing a populated <see cref="NavigableContext"/>,
/// mirroring what <see cref="IocContainer.GetContextFor(string, NavigableContext)"/> does internally.
/// </summary>
public class IocContainerTests
{
    private static IServiceProvider EmptyProvider() => new ServiceCollection().BuildServiceProvider();

    [Fact]
    public void GetView_throws_when_no_view_is_registered_under_the_key()
    {
        var container = new IocContainer(EmptyProvider());

        var ex = Assert.Throws<Exception>(() => container.GetView("missing", []));
        Assert.Contains("missing", ex.Message);
    }

    [Fact]
    public void Register_with_a_raw_factory_round_trips_through_GetView()
    {
        var container = new IocContainer(EmptyProvider());
        var expected = new FakeViewFor();
        container.Register("my-key", (sp, args) => expected);

        var result = container.GetView("my-key", []);

        Assert.Same(expected, result);
    }

    [Fact]
    public void Register_by_viewmodel_type_is_keyed_by_its_name_and_receives_the_NavigableContext()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<FakeViewModel>((sp, context) => new FakeViewFor { Context = context });

        var navContext = new NavigableContext();
        var result = container.GetContextFor(nameof(FakeViewModel), navContext);

        Assert.NotNull(result);
        Assert.Same(navContext, result!.Context);
    }

    [Fact]
    public void Register_parameterless_creates_a_new_instance_via_its_default_constructor()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<FakeViewFor<FakeViewModel>>();

        var result = container.GetView(nameof(FakeViewModel), [new NavigableContext()]);

        Assert.IsType<FakeViewFor<FakeViewModel>>(result);
    }

    [Fact]
    public void Register_with_one_positional_argument_reads_it_from_the_context()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<FakeViewFor<FakeViewModel>, string>((sp, arg1) => new FakeViewFor<FakeViewModel> { Context = arg1 });

        var navContext = NavigableContext.Create().WithViewModelParameters("hello");
        var result = (FakeViewFor<FakeViewModel>)container.GetView(nameof(FakeViewModel), [navContext]);

        Assert.Equal("hello", result.Context);
    }

    [Fact]
    public void Register_with_two_positional_arguments_reads_them_in_order()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<FakeViewFor<FakeViewModel>, string, int>((sp, arg1, arg2) => new FakeViewFor<FakeViewModel> { Context = $"{arg1}-{arg2}" });

        var navContext = NavigableContext.Create().WithViewModelParameters("a", 1);
        var result = (FakeViewFor<FakeViewModel>)container.GetView(nameof(FakeViewModel), [navContext]);

        Assert.Equal("a-1", result.Context);
    }

    [Fact]
    public void GetContextFor_throws_when_the_resolved_view_does_not_implement_IViewFor()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register("not-a-view", (sp, args) => new object());

        Assert.Throws<InvalidOperationException>(() => container.GetContextFor("not-a-view", new NavigableContext()));
    }

    [Fact]
    public void GetContextFor_generic_resolves_the_strongly_typed_view()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<FakeViewModel>((sp, context) => new FakeViewFor<FakeViewModel> { Context = context });

        var result = container.GetContextFor<FakeViewModel>(new NavigableContext());

        Assert.NotNull(result);
        Assert.IsType<FakeViewFor<FakeViewModel>>(result);
    }

    [Fact]
    public void GetContextFor_generic_returns_null_when_the_resolved_view_has_the_wrong_type()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<FakeViewModel>((sp, context) => new FakeViewFor<OtherFakeViewModel>());

        var result = container.GetContextFor<FakeViewModel>(new NavigableContext());

        Assert.Null(result);
    }

    [Fact]
    public void GetModal_returns_the_registered_view_when_the_result_type_matches()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<TestCloseableViewModel>((sp, context) => new FakeModalFor<TestCloseableViewModel, string>());

        var modal = container.GetModal<TestCloseableViewModel, string>(new NavigableContext());

        Assert.IsType<FakeModalFor<TestCloseableViewModel, string>>(modal);
    }

    //[Fact]
    //public void GetModal_throws_when_the_registered_result_type_does_not_match_the_requested_type()
    //{
    //    var container = new IocContainer(EmptyProvider());
    //    container.Register<TestCloseableViewModel>((sp, context) => new FakeModalFor<TestCloseableViewModel, string>());

    //    Assert.Throws<InvalidOperationException>(() => container.GetModal<TestCloseableViewModel, int>(new NavigableContext()));
    //}

    [Fact]
    public void GetModal_throws_when_the_registered_view_is_not_a_modal()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<TestCloseableViewModel>((sp, context) => new FakeViewFor<TestCloseableViewModel>());

        Assert.Throws<InvalidOperationException>(() => container.GetModal<TestCloseableViewModel, string>(new NavigableContext()));
    }

    [Fact]
    public void GetModalFor_delegates_to_GetModal()
    {
        var container = new IocContainer(EmptyProvider());
        container.Register<TestCloseableViewModel>((sp, context) => new FakeModalFor<TestCloseableViewModel, string>());

        var modal = container.GetModalFor<TestCloseableViewModel, string>(new NavigableContext());

        Assert.IsType<FakeModalFor<TestCloseableViewModel, string>>(modal);
    }
}

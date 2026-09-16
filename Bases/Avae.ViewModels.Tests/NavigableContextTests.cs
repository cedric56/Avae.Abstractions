namespace Avae.ViewModels.Tests;

public class NavigableContextTests
{
    [Fact]
    public void Create_returns_context_with_no_parameters()
    {
        var context = NavigableContext.Create();

        Assert.Empty(context.Parameters);
        Assert.Empty(context.FactoryParameters);
        Assert.Empty(context.ViewParameters);
        Assert.Empty(context.ViewModelParameters);
    }

    [Fact]
    public void Parameters_combines_factory_view_and_viewmodel_parameters_in_order()
    {
        var context = NavigableContext.Create()
            .WithFactoryParameters("factory1")
            .WithViewParameters("view1", "view2")
            .WithViewModelParameters(42);

        Assert.Equal(["factory1", "view1", "view2", 42], context.Parameters);
    }

    [Fact]
    public void With_methods_return_the_same_instance_for_fluent_chaining()
    {
        var context = new NavigableContext();

        var afterFactory = context.WithFactoryParameters("a");
        var afterView = afterFactory.WithViewParameters("b");
        var afterViewModel = afterView.WithViewModelParameters("c");

        Assert.Same(context, afterFactory);
        Assert.Same(context, afterView);
        Assert.Same(context, afterViewModel);
    }

    [Fact]
    public void With_methods_overwrite_previously_set_parameters_of_the_same_kind()
    {
        var context = NavigableContext.Create()
            .WithViewModelParameters(1, 2)
            .WithViewModelParameters(3);

        Assert.Equal([3], context.ViewModelParameters);
    }

    [Fact]
    public void Get_returns_the_typed_parameter_at_the_given_index()
    {
        var context = NavigableContext.Create().WithViewModelParameters(1, "two", 3.0);

        Assert.Equal(1, context.Get<int>(0));
        Assert.Equal("two", context.Get<string>(1));
        Assert.Equal(3.0, context.Get<double>(2));
    }

    [Fact]
    public void Get_reads_across_factory_view_and_viewmodel_parameters_by_combined_index()
    {
        var context = NavigableContext.Create()
            .WithFactoryParameters("factory")
            .WithViewParameters("view")
            .WithViewModelParameters("viewmodel");

        Assert.Equal("factory", context.Get<string>(0));
        Assert.Equal("view", context.Get<string>(1));
        Assert.Equal("viewmodel", context.Get<string>(2));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void Get_throws_ArgumentOutOfRangeException_for_an_invalid_index(int index)
    {
        var context = NavigableContext.Create().WithViewModelParameters(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => context.Get<int>(index));
    }

    [Fact]
    public void Get_throws_InvalidCastException_when_the_parameter_type_does_not_match()
    {
        var context = NavigableContext.Create().WithViewModelParameters("not an int");

        Assert.Throws<InvalidCastException>(() => context.Get<int>(0));
    }
}

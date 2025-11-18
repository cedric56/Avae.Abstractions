namespace Avae.Abstractions
{
    public class ViewModelBase : IViewModelBase
    {
        public virtual Task OnLaunched()
        {
            return Task.CompletedTask;
        }
    }
}

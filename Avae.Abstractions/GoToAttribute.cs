namespace Avae.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GoToAttribute : Attribute
    {
        // Optional: Customize overload prefix (default: "GoTo")
        public string MethodName { get; set; } = "GoTo";
    }
}

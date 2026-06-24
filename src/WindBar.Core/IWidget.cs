namespace WindBar.Core
{
    public interface IWidget
    {
        string DisplayName { get; }
        bool Horizontal { get; }
        bool Vertical { get; }
        object CreateView(string orientation);
        object CreateFallbackView();
    }
}

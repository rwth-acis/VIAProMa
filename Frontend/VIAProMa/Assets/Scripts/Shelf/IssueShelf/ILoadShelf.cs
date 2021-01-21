using i5.VIAProMa.UI.MessageBadge;

namespace i5.VIAProMa.Shelves.IssueShelf
{
    public interface ILoadShelf
    {
        void LoadContent();

        MessageBadge MessageBadge { get; }
    }
}
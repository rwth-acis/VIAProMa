using i5.VIAProMa.DataModel.API;

namespace i5.VIAProMa.Shelves.IssueShelf
{
    public interface IShelfConfiguration
    {
        bool IsValidConfiguration { get; }

        DataSource SelectedSource { get; }
    }
}
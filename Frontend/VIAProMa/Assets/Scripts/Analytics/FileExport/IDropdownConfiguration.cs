using i5.VIAProMa.DataModel.API;

namespace i5.VIAProMa.Shelves.IssueShelf
{
    public interface IDropdownConfiguration
    {
        bool IsValidConfiguration { get; }

        ExportSelection SelectedFormat { get; }
    }
}

using i5.VIAProMa.DataModel.API;

namespace i5.VIAProMa.Analytics.FileExport
{
    public interface IDropdownConfiguration
    {
        bool IsValidConfiguration { get; }

        ExportSelection SelectedFormat { get; }
    }
}

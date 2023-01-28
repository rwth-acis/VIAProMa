using System.ComponentModel;

namespace i5.VIAProMa.DataModel.API
{
    /// <summary>
    /// The data source of the issues
    /// The order/indices of the entries is important and should be synchronized with the corresponding enum of the backend
    /// </summary>
    public enum ExportSelection
    {
        [Description("JSON")] JSON,
        [Description("SQLite")] SQLITE,
        [Description("CSV")] CSV
    }
}
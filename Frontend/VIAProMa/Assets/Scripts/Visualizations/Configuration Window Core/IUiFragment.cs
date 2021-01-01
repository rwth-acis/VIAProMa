namespace i5.VIAProMa.Visualizations.ColorConfigWindow
{
    public interface IUiFragment
    {
        bool UIEnabled { get; set; }

        void Setup(Visualization visualization);
    }
}
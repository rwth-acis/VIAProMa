using i5.VIAProMa.Visualizations.BuildingProgressBar;

public class BuildingProgressBarTester : ProgressBarTester
{
    // you can find the rest of the functionality in the ProgressBarTester
    // as both share the same code for the core progress bar functionality

    public int buildingIndex = 0;

    private void Start()
    {
        progressBar.GetComponent<BuildingProgressBarVisuals>().BuildingModelIndex = buildingIndex;
    }
}

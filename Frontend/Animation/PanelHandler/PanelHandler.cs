using System.Collections.Generic;
using System.Windows.Forms;

public class PanelHandler
{
    private readonly List<Panel> panels = new List<Panel>();
    private string CurrentPanel;

    // Add a panel to the manager
    public void AddPanel(Panel panel)
    {
        if (!panels.Contains(panel))
            panels.Add(panel);
    }

    // Add multiple panels at once
    public void AddPanels(params Panel[] panelArray)
    {
        foreach (var panel in panelArray)
            AddPanel(panel);
    }

    // Show only the specified panel, hide the rest
    public void ShowOnly(Panel panelToShow)
    {
        foreach (var panel in panels)
            panel.Visible = (panel == panelToShow);

        // Send shown panel to back so slider buttons stay on top
        if (panelToShow != null)
        {
            panelToShow.SendToBack();
        }

        CurrentPanel = panelToShow.Name; // Use Name instead of ToString()
    }

    // Check if current panel is not the rent_panel
    public bool IsCurrentPanel(string panelName)
    {
        return CurrentPanel != panelName;
    }

    // Alternative: Get the current panel name
    public string GetCurrentPanelName()
    {
        return CurrentPanel;
    }
}

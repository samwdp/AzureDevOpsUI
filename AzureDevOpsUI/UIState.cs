using System.Numerics;
using System.Reflection;
using ZeroElectric.Vinculum;
using Font = ZeroElectric.Vinculum.Font;
using Rectangle = ZeroElectric.Vinculum.Rectangle;

namespace AzureDevOpsUi;
public class UIState
{

    public Dictionary<string, WorkItemList> stateToTitles { get; set; } = new(){
        {"New", new()},
        {"Active", new()},
        {"Ready For Test", new()},
    };
    public MinimalWorkItem? selectedWorkItem { get; set; } = null;
    public int FONT_SPACING = 1;
    public int GUI_FONT_SIZE = 18;
    public int PADDING = 5;
    public string REFRESH = "Refresh";
    public Font Font { get; set; }
    public WindowToShow? OpenWindow { get; set; } = null;
    public Vector2 RefreshTextWidth { get; set; }
    public Vector2 DropdownWidth { get; set; }
    public string[] Queries { get; set; } = [];
    public string SelectedQuery { get; set; } = "";
    public int SelectedQueryId { get; set; } = 0;

    // constructor
    public UIState(Config config)
    {
        Queries = config.Queries;
        SelectedQuery = Queries[0] ?? "";
        string? baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        GUI_FONT_SIZE = config.FontSize;
        Font = Raylib.LoadFontEx(Path.Combine(baseDirectory, $"resources/{config.Font}"), GUI_FONT_SIZE, 250);
        RefreshTextWidth = Raylib.MeasureTextEx(Font, REFRESH, GUI_FONT_SIZE, FONT_SPACING);
        // get the longest string from Queries
        var longest = Queries.OrderByDescending(q => q.Length).First();
        DropdownWidth = Raylib.MeasureTextEx(Font, longest, GUI_FONT_SIZE, FONT_SPACING);
    }

}
public class MinimalWorkItem()
{
    public int? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
public class WorkItemList()
{
    public List<MinimalWorkItem> WorkItems { get; set; } = [];
    public int Active { get; set; } = -1;
    public int Focused { get; set; } = -1;
    public int Scroll { get; set; } = 0;

}

public class WindowToShow()
{
    public bool IsOpen { get; set; }
    public bool Dragging { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Vector2 PanOffset { get; set; }
    public Vector2 Scroll { get; set; } = new(0, 0);
    public Rectangle WindowRect { get; set; }
}

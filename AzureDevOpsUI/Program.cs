using System.Text.Json;
using AzureDevOpsUi;
using ZeroElectric.Vinculum;

Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE
        | ConfigFlags.FLAG_VSYNC_HINT);

Raylib.InitWindow(800, 450, "Dev Ops UI");
Config c = JsonSerializer.Deserialize<Config>(File.ReadAllText("./config.json"), AppJsonSerializerContext.Default.Config);
UIState uiState = new(c);
DataGetter data = new(c);

RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_SIZE, uiState.GUI_FONT_SIZE);
RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.BACKGROUND_COLOR, Raylib.ColorToInt(Raylib.GRAY));
RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiControlProperty.TEXT_COLOR_NORMAL, Raylib.ColorToInt(Raylib.LIGHTGRAY));
RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_ALIGNMENT_VERTICAL, (int)GuiTextAlignmentVertical.TEXT_ALIGN_TOP);
RayGui.GuiSetStyle((int)GuiControl.LISTVIEW, (int)GuiControlProperty.TEXT_ALIGNMENT, (int)GuiTextAlignment.TEXT_ALIGN_LEFT);
RayGui.GuiSetStyle((int)GuiControl.LISTVIEW, (int)GuiListViewProperty.LIST_ITEMS_HEIGHT, (int)uiState.RefreshTextWidth.Y);
RayGui.GuiSetStyle((int)GuiControl.LISTVIEW, (int)GuiControlProperty.TEXT_PADDING, uiState.PADDING);
RayGui.GuiSetStyle((int)GuiControl.TEXTBOX, (int)GuiTextBoxProperty.TEXT_READONLY, 1);
RayGui.GuiSetStyle((int)GuiControl.BUTTON, (int)GuiControlProperty.BASE_COLOR_NORMAL, Raylib.ColorToInt(Raylib.GRAY));
RayGui.GuiSetStyle((int)GuiControl.STATUSBAR, (int)GuiControlProperty.BASE_COLOR_NORMAL, Raylib.ColorToInt(Raylib.DARKGRAY));
RayGui.GuiSetStyle((int)GuiControl.STATUSBAR, (int)GuiControlProperty.TEXT_ALIGNMENT, (int)GuiTextAlignment.TEXT_ALIGN_CENTER);
RayGui.GuiSetStyle((int)GuiControl.STATUSBAR, (int)GuiControlProperty.TEXT_PADDING, 5);
RayGui.GuiSetStyle((int)GuiControl.STATUSBAR, (int)GuiDefaultProperty.TEXT_ALIGNMENT_VERTICAL, (int)GuiTextAlignmentVertical.TEXT_ALIGN_MIDDLE);
RayGui.GuiSetStyle((int)GuiControl.DROPDOWNBOX, (int)GuiControlProperty.TEXT_COLOR_NORMAL, Raylib.ColorToInt(Raylib.LIGHTGRAY));
RayGui.GuiSetStyle((int)GuiControl.DROPDOWNBOX, (int)GuiControlProperty.BASE_COLOR_NORMAL, Raylib.ColorToInt(Raylib.GRAY));

RayGui.GuiSetFont(uiState.Font);
int selectedQueryId = 0;
bool editMode = false;

while (!Raylib.WindowShouldClose())
{
    // every 10 seconds ShowWorkItems should be called
    if (Raylib.GetTime() % 60 < 0.1)
    {
        data.ShowWorkItems(uiState).Wait();
    }
    if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL))
    {
        uiState.GUI_FONT_SIZE += (int)(Raylib.GetMouseWheelMove() * 8.0);
        if (uiState.GUI_FONT_SIZE < 10) uiState.GUI_FONT_SIZE = 10;
        uiState.Font = Raylib.LoadFontEx("C:\\Windows\\Fonts\\LilexNerdFont-Regular.ttf", uiState.GUI_FONT_SIZE, 250);
        RayGui.GuiSetFont(uiState.Font);
        RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_SIZE, uiState.GUI_FONT_SIZE);
        RayGui.GuiSetStyle((int)GuiControl.LISTVIEW, (int)GuiListViewProperty.LIST_ITEMS_HEIGHT, (int)uiState.RefreshTextWidth.Y);
        uiState.RefreshTextWidth = Raylib.MeasureTextEx(uiState.Font, uiState.REFRESH, uiState.GUI_FONT_SIZE, uiState.FONT_SPACING);
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Raylib.DARKGRAY);
    if (RayGui.GuiButton(new(10, 10, uiState.RefreshTextWidth.X, uiState.RefreshTextWidth.Y), uiState.REFRESH) == 1)
    {
        data.ShowWorkItems(uiState).Wait();
    }

    Renderer.RenderWorkItems(uiState);
    Renderer.RenderClickedWindow(uiState);
    // combine uiState.Queries into a single string with ; as a delimiter
    var q = string.Join(";", uiState.Queries);
    uiState.DropdownWidth = Raylib.MeasureTextEx(uiState.Font,
            uiState.Queries.OrderByDescending(q => q.Length).First(),
            uiState.GUI_FONT_SIZE,
            uiState.FONT_SPACING);
    if (RayGui.GuiDropdownBox(new(200, 10, uiState.DropdownWidth.X, uiState.DropdownWidth.Y), q, ref selectedQueryId, editMode) == 1)
    {
        editMode = !editMode;
        uiState.SelectedQuery = uiState.Queries[selectedQueryId];
        if (!editMode)
        {
            data.ShowWorkItems(uiState).Wait();
        }
    }
    Raylib.EndDrawing();
}
Raylib.UnloadFont(uiState.Font);
Raylib.CloseWindow();



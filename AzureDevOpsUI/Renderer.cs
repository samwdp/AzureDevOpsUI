using System.Numerics;
using ZeroElectric.Vinculum;

namespace AzureDevOpsUi;
public static class Renderer
{
    public static void RenderClickedWindow(UIState uiState)
    {
        if (uiState.OpenWindow is not null && uiState.OpenWindow.IsOpen)
        {
            var result = RayGui.GuiWindowBox(uiState.OpenWindow.WindowRect, uiState.OpenWindow.Title);

            // Content area within the window
            Rectangle contentArea = new(
                uiState.OpenWindow.WindowRect.x + uiState.PADDING,
                uiState.OpenWindow.WindowRect.y + (uiState.PADDING * 5),
                uiState.OpenWindow.WindowRect.width - (uiState.PADDING * 2),
                uiState.OpenWindow.WindowRect.height - (uiState.PADDING * 6));

            // Calculate content text size to determine if scrolling is needed
            string content = uiState.OpenWindow.Content ?? string.Empty;
            Vector2 textSize = Raylib.MeasureTextEx(uiState.Font, content, uiState.GUI_FONT_SIZE, uiState.FONT_SPACING);

            // Create view area and content area for scrolling

            Rectangle view = contentArea;
            Vector2 scroll = uiState.OpenWindow.Scroll;
            Rectangle content_rec = new(0, 0, Math.Max(contentArea.width - 4, (textSize.X / 2)), Math.Max(contentArea.height, textSize.Y));

            // Use GuiScrollPanel to create scrollable content
            Console.WriteLine($"Content Area: {content_rec.width}");
            RayGui.GuiScrollPanel(contentArea, null, content_rec, ref scroll, ref view);
            uiState.OpenWindow.Scroll = scroll;

            Raylib.BeginScissorMode(
                (int)contentArea.x,
                (int)contentArea.y,
                (int)contentArea.width,
                (int)contentArea.height);

            // Adjust the position for rendering the actual content inside the scroll panel
            Rectangle textRect = new(
                contentArea.x + scroll.X,
                contentArea.y + scroll.Y, // Apply scroll offset to Y position
                content_rec.width,
                content_rec.height);

            // Draw the content text inside the scroll panel
            RayGui.GuiLabel(textRect, content);

            // End scissor mode
            Raylib.EndScissorMode();
            if (result == 1) uiState.OpenWindow.IsOpen = false;
        }

    }

    public static void RenderWorkItems(UIState uiState)
    {
        int numKeys = uiState.stateToTitles.Keys.Count;
        if (numKeys > 0)
        {
            int columnWidth = Raylib.GetScreenWidth() / numKeys;
            int columnIndex = 0;
            foreach (var key in uiState.stateToTitles.Keys)
            {
                var wi = uiState.stateToTitles[key];
                // Position each key label across the screen
                int x = columnIndex * columnWidth;
                int y = 50; // Position below the refresh button
                var keyTextSize = Raylib.MeasureTextEx(uiState.Font, key, uiState.GUI_FONT_SIZE, uiState.FONT_SPACING);
                int centeredX = x + (columnWidth - (int)keyTextSize.X) / 2;
                RayGui.GuiLabel(new(centeredX, y, keyTextSize.X, keyTextSize.Y), key);

                int scroll = wi.Scroll;
                int active = wi.Active;
                int focus = wi.Focused;
                // Center the text in its column
                Rectangle rect = new(x + uiState.PADDING, y + keyTextSize.Y, columnWidth - (uiState.PADDING * 2),
                        (Raylib.GetScreenHeight() / 2) - (uiState.RefreshTextWidth.Y + keyTextSize.Y + (uiState.PADDING * 5)));
                RayGui.GuiListViewEx(
                        rect,
                        wi.WorkItems.Select(x => x.Title).ToArray(),
                        wi.WorkItems.Count,
                        ref scroll, ref active, ref focus);
                wi.Active = active;
                wi.Focused = focus;
                wi.Scroll = scroll;

                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    Vector2 mousePos = Raylib.GetMousePosition();
                    if (Raylib.CheckCollisionPointRec(mousePos, rect) && wi.Focused != -1 && wi.Active != -1)
                    {
                        uiState.OpenWindow = new()
                        {
                            IsOpen = true,
                            Title = $"{wi.WorkItems[wi.Focused].State} - {wi.WorkItems[wi.Focused].Title}",
                            Content = wi.WorkItems[wi.Focused].Description,
                            WindowRect = new(
                                    uiState.PADDING,
                                    (Raylib.GetScreenHeight() / 2) + (uiState.RefreshTextWidth.Y + keyTextSize.Y + uiState.PADDING),
                                    Raylib.GetScreenWidth() - uiState.PADDING * 2,
                                    (Raylib.GetScreenHeight() / 2) - (uiState.RefreshTextWidth.Y + keyTextSize.Y + (uiState.PADDING * 5))
                                    )
                        };
                    }
                }
                columnIndex++;
            }
        }
    }
}

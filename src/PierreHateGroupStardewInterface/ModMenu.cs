using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace PierreHateGroupStardewInterface;

class ModMenu : IClickableMenu {
    private List<ClickableComponent> Labels = new();
    private ClickableTextureComponent? Ok;

    public ModMenu() : base((int)GetMenuPosition().X, (int)GetMenuPosition().Y, menuWidth, menuHeight)
    {
        setUpPositions();
    }

    public static int menuWidth = 632 + borderWidth * 2;
    public static int menuHeight = 600 + borderWidth * 2 + Game1.tileSize;

    public static Vector2 GetMenuPosition()
    {
        Vector2 defaultPosition = new Vector2(Game1.viewport.Width / 2 - menuWidth / 2, (Game1.viewport.Height / 2 - menuHeight / 2));

        //Force the viewport into a position that it should fit into on the screen???
        if (defaultPosition.X + menuWidth > Game1.viewport.Width)
        {
            defaultPosition.X = 0;
        }

        if (defaultPosition.Y + menuHeight > Game1.viewport.Height)
        {
            defaultPosition.Y = 0;
        }
        return defaultPosition;

    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
        xPositionOnScreen = (int)GetMenuPosition().X;
        yPositionOnScreen = (int)GetMenuPosition().Y;
        setUpPositions();
    }

    private void setUpPositions()
    {
        Ok = new("OK", new Rectangle(this.xPositionOnScreen + this.width - borderWidth - spaceToClearSideBorder - Game1.tileSize,
        yPositionOnScreen + height - borderWidth - spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize),
        "",
        null,
        Game1.mouseCursors,
        Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);


        Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4 + spaceToClearSideBorder + borderWidth + Game1.tileSize * 3 + 8, yPositionOnScreen + borderWidth + spaceToClearTopBorder - Game1.tileSize / 8, 1, 1),
        $"Connection Status: {(ModEntry.s_connectionStatus ? "CONNECTED!" : "NOT CONNECTED")}"));
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);

        if(Ok is null) {
            return;
        }

        if(Ok.containsPoint(x, y)) {
            Game1.exitActiveMenu();
        }
    }

    public override void performHoverAction(int x, int y)
    {
        if(Ok is null) return;
        Ok.scale = Ok.containsPoint(x, y)
            ? Math.Min(Ok.scale + 0.02f, Ok.baseScale + 0.1f)
            : Math.Max(Ok.scale - 0.02f, Ok.baseScale);
    }

    public override void draw(SpriteBatch b)
    {
        Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

        Labels.ForEach(x => {
            Utility.drawTextWithShadow(b, x.name, Game1.smallFont, new Vector2(x.bounds.X, x.bounds.Y), Color.Violet);
        });

        if (Ok is not null) {
            Ok.draw(b);
        }

        drawMouse(b);
    }
}
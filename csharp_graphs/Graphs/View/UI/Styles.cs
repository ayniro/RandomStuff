using System.Drawing;
using SpaceVIL;
using SpaceVIL.Common;
using SpaceVIL.Core;
using SpaceVIL.Decorations;

namespace Graphs.View.UI
{
    public static class Styles
    {
        public static Style GetMainLayoutStyle()
        {
            var style = Style.GetVerticalSplitAreaStyle();
            style.Margin = new Indents(top: UiElements.GetTitleBar().GetHeight());
            style.Padding = new Indents(3, 3, 3, 3);
            style.Spacing = new Spacing(5);
            style.Background = Color.Gray;
            style.WidthPolicy = SizePolicy.Expand;
            return style;
        }

        public static Style GetButtonsListStyle()
        {
            var style = new Style
            {
                MinWidth = 220,
                WidthPolicy = SizePolicy.Expand,
                Padding = new Indents(3, 3, 3, 3),
                Spacing = new Spacing(0, 5),
                Background = Color.DarkGray
            };
            return style;
        }

        public static Style GetTabViewStyle()
        {
            var style = Style.GetTabViewStyle();

            style.WidthPolicy = SizePolicy.Expand;
            style.Alignment = ItemAlignment.HCenter | ItemAlignment.VCenter;
            style.MinWidth = 200;
            style.HeightPolicy = SizePolicy.Expand;

            return style;
        }

        public static Style GetLeftLayoutItemStyle()
        {
            return Style.GetVerticalStackStyle();
        }

        public static Style GetToolbarStyle()
        {
            var style = new Style
            {
                Height = 50,
                MaxHeight = 50,
                Background = Color.DarkGray,
                Padding = new Indents(3, 3, 3, 3),
                Spacing = new Spacing(0, 5),
                WidthPolicy = SizePolicy.Expand,
                HeightPolicy = SizePolicy.Fixed
            };
            return style;
        }

        public static Style GetButtonStyle()
        {
            var style = Style.GetButtonCoreStyle();

            style.Background = Color.DimGray;
            style.Foreground = Color.Lavender;
            style.Width = 200;
            style.Height = 45;
            style.BorderRadius = new CornerRadius();
            style.Font = new Font(DefaultsService.GetDefaultFont().FontFamily, 14, FontStyle.Bold);
            style.WidthPolicy = SizePolicy.Fixed;
            style.HeightPolicy = SizePolicy.Fixed;
            style.Alignment = ItemAlignment.HCenter;
            style.TextAlignment = ItemAlignment.HCenter | ItemAlignment.VCenter;
            
            style.AddItemState(ItemStateType.Hovered,
                new ItemState(Color.DarkSlateGray));
            style.AddItemState(ItemStateType.Pressed,
                new ItemState(Color.FromArgb(50, Color.DarkSlateGray)));
            return style;
        }

        public static Style GetLabelStyle()
        {
            var style = Style.GetLabelStyle();

            style.Foreground = Color.FromArgb(255, 210, 210, 210);
            style.Font = new Font(DefaultsService.GetDefaultFont().FontFamily, 14, FontStyle.Bold);
            style.Height = 30;
            style.HeightPolicy = SizePolicy.Fixed;
            style.Alignment = ItemAlignment.VCenter | ItemAlignment.Left;
            style.TextAlignment = ItemAlignment.VCenter | ItemAlignment.HCenter;
            
            return style;
        }

        public static Style GetDialogLayoutStyle(int titleHeight)
        {
            var style = Style.GetVerticalStackStyle();
            style.SetMargin(top: titleHeight);
            style.SetPadding(6, 15, 6, 6);
            style.SetSpacing(vertical: 5);
            style.SetBackground(255, 255, 255, 20);
            return style;
        }

        public static Style GetDialogButtonStyle()
        {
            var style = new Style
            {
                Foreground = Color.Black,
                BorderRadius = new CornerRadius(),
                Font = new Font(DefaultsService.GetDefaultFont().FontFamily, 14, FontStyle.Bold),
                Width = 150,
                Height = 30,
                WidthPolicy = SizePolicy.Fixed,
                HeightPolicy = SizePolicy.Fixed,
                Alignment = ItemAlignment.HCenter | ItemAlignment.Top,
                TextAlignment = ItemAlignment.HCenter | ItemAlignment.VCenter,
                Background = Color.DarkGray,//Color.FromArgb(255, 255, 181, 111),
                ShadowColor = Color.FromArgb(150, 0, 0, 0),
                ShadowRadius = 5,
                ShadowXOffset = 0,
                ShadowYOffset = 4,
                IsShadowDrop = true
            };
            
            style.AddItemState(
                ItemStateType.Hovered, 
                new ItemState(Color.FromArgb(30, 255, 255, 255)));
            return style;
        }

        public static Style GetHorizontalButtonLayoutStyle()
        {
            var style = Style.GetHorizontalStackStyle();
            //style.MinWidth = 300;
            //style.SetSizePolicy(SizePolicy.Expand, SizePolicy.Fixed);
            return style;
        }
    }
}
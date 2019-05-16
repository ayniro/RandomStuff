using System.Drawing;
using SpaceVIL;
using SpaceVIL.Common;
using SpaceVIL.Core;

namespace Graphs.View.UI
{
    public static class UiElements
    {
        public static TitleBar GetTitleBar()
        {
            var title = new TitleBar("Graphs");
            return title;
        }
        public static VerticalSplitArea GetMainLayout()
        {
            var layout = new VerticalSplitArea();
            layout.SetStyle(Styles.GetMainLayoutStyle());
            layout.SetSplitPosition(750);
            layout.SetSplitThickness(2);
            return layout;
        }

        public static VerticalStack GetButtonsList()
        {
            var buttonsList = new VerticalStack();
            buttonsList.SetStyle(Styles.GetButtonsListStyle());
            return buttonsList;
        }

        public static TabView GetTabView()
        {
            var tabView = new TabView();
            tabView.SetStyle(Styles.GetTabViewStyle());
            return tabView;
        }
        
        public static VerticalStack GetLeftLayoutItem()
        {
            var verticalStack = new VerticalStack();
            //verticalStack.SetStyle(Styles.GetLeftLayoutItemStyle());
            return verticalStack;
        }

        public static HorizontalStack GetToolbar()
        {
            var toolbar = new HorizontalStack();
            toolbar.SetStyle(Styles.GetToolbarStyle());
            toolbar.SetContentAlignment(ItemAlignment.Bottom);
            return toolbar;
        }

        public static ButtonCore GetButton(string name)
        {
            var button = new ButtonCore(name);
            button.SetStyle(Styles.GetButtonStyle());
            button.SetShadow(5, 2, 2, Color.Black);
            return button;
        }

        public static VerticalStack GetDialogWindowLayout(int titleHeight)
        {
            var layout = new VerticalStack();
            layout.SetStyle(Styles.GetDialogLayoutStyle(titleHeight));
            layout.SetContentAlignment(ItemAlignment.HCenter);
            return layout;
        }

        public static TitleBar GetDialogWindowTitleBar(string name)
        {
            var title = new TitleBar(name);
            title.SetFont(DefaultsService.GetDefaultFont(14));
            title.GetMinimizeButton().SetVisible(false);
            title.GetMaximizeButton().SetVisible(false);
            return title;
        }

        public static TextArea GetLogTextArea()
        {
            var textArea = new TextArea();
            textArea.SetEditable(false);
            return textArea;
        }

        public static HorizontalStack GetHorizontalButtonLayout()
        {
            var layout = new HorizontalStack();
            //layout.SetStyle(Styles.GetHorizontalButtonLayoutStyle());
            layout.SetMinWidth(300);
            layout.SetMaxHeight(50);
            layout.SetContentAlignment(ItemAlignment.HCenter);
            layout.SetSizePolicy(SizePolicy.Expand, SizePolicy.Fixed);
            layout.SetHeight(60);
            layout.SetSpacing(20);
            return layout;
        }
    }
}
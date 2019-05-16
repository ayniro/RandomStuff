using System;
using System.Drawing;
using Graphs.Presenter;
using SpaceVIL;
using SpaceVIL.Core;
using SpaceVIL.Decorations;

namespace Graphs.View.UI.Dialogs
{
    public class ShimbelDialog : DialogItem, IPresenterConnectedDialog
    {
        private readonly Label _info = new Label();
        private readonly Label _info2 = new Label();
        private readonly Label _output = new Label();

        private readonly NamedLabel _edgesAmount =
            new NamedLabel("Edges Amount: ", 1.ToString());

        private readonly ButtonCore _shortestButton = new ButtonCore("Shortest");
        private readonly ButtonCore _longestButton = new ButtonCore("Longest");
        private readonly int _vertices;

        public event EventHandler<GotCorrectShimbelEventArgs> GotCorrectShimbel;

        public ShimbelDialog(int vertices)
        {
            SetItemName("ShimbelDialog_Unique");

            _vertices = vertices;
            _info.SetStyle(Styles.GetLabelStyle());
            _info2.SetStyle(Styles.GetLabelStyle());
            _output.SetStyle(Styles.GetLabelStyle());
            _output.SetBorder(new Border(Color.LightGray, new CornerRadius(2f), 2));
            
            _shortestButton.SetStyle(Styles.GetDialogButtonStyle());
            _longestButton.SetStyle(Styles.GetDialogButtonStyle());
        }

        public override void InitElements()
        {
            base.InitElements();

            Window.SetBackground(45, 45, 45);
            Window.SetHeight(_vertices * 16 + 500);
            Window.SetWidth(_vertices * 16 + 300);
            //Window.IsLocked = true;

            var title = UiElements.GetDialogWindowTitleBar("Shimbel's Algorithm");
            var layout = UiElements.GetDialogWindowLayout(title.GetHeight());
            var buttonsLayout = UiElements.GetHorizontalButtonLayout();

            _info.SetText("Compute paths of given length:");
            _info2.SetText("Edges: 1 <= x <= Graph Vertices");
            _shortestButton.SetShadow(5, 0, 4, Color.FromArgb(150, 0, 0, 0));
            _shortestButton.EventMouseClick += OnShortestButtonClick;
            _shortestButton.SetWidth(100);
            _longestButton.SetShadow(5, 0, 4, Color.FromArgb(150, 0, 0, 0));
            _longestButton.EventMouseClick += OnLongestButtonClick;
            _longestButton.SetWidth(100);
            
            _output.SetPadding(top: 12);
            _output.SetTextAlignment(ItemAlignment.Top, ItemAlignment.HCenter);
            
            Window.AddItems(title, layout);
            
            layout.AddItems(
                _info,
                _info2,
                _edgesAmount,
                buttonsLayout,
                _output);

            buttonsLayout.AddItems(_shortestButton, _longestButton);

            title.GetCloseButton().EventMouseClick = (sender, args) => { Close(); };
        }

        public override void Show(CoreWindow handler)
        {
            _edgesAmount.SetText(string.Empty);
            base.Show(handler);
            _edgesAmount.SetFocus();
        }

        public override void Close()
        {
            OnCloseDialog?.Invoke();
            base.Close();
        }

        public void SetData(string data)
        {
            _output.SetText(data);
            _output.SetHeight(CalculateNewLines(data) * 24);
        }

        private bool ValidateInput()
        {
            if (!Int32.TryParse(_edgesAmount.GetText(), out int edges))
                return false;

            return edges >= 1 && edges <= _vertices;
        }

        protected virtual void OnGotCorrectInput(GotCorrectShimbelEventArgs e)
        {
            var handler = GotCorrectShimbel;
            handler?.Invoke(this, e);
        }

        private void OnShortestButtonClick(IItem sender, MouseArgs args)
        {
            OnButtonClick(true);
        }

        private void OnLongestButtonClick(IItem sender, MouseArgs args)
        {
            OnButtonClick(false);
        }

        private void OnButtonClick(bool shortest)
        {
            if (ValidateInput())
            {
                var eventArgs = new GotCorrectShimbelEventArgs
                {
                    EdgesAmount = Int32.Parse(_edgesAmount.GetText()),
                    ShortestPaths = shortest
                };
                _output.SetText(string.Empty);
                OnGotCorrectInput(eventArgs);
            }
            else
            {
                _output.SetText("Incorrect Input");
                _output.SetHeight(50);
            }
        }

        private int CalculateNewLines(string str)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c.Equals('\n'))
                    count++;
            }

            return count;
        }
    }

    public class GotCorrectShimbelEventArgs : EventArgs
    {
        public int EdgesAmount { get; set; }
        public bool ShortestPaths { get; set; }
    }
}
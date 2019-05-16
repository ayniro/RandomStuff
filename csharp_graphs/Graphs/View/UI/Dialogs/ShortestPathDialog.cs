using System;
using System.Drawing;
using Graphs.Presenter;
using SpaceVIL;
using SpaceVIL.Core;

namespace Graphs.View.UI.Dialogs
{
    public class ShortestPathDialog : DialogItem, IPresenterConnectedDialog
    {
        private readonly Label _info = new Label();
        private readonly Label _output = new Label();
        private readonly ButtonCore _dijkstra = new ButtonCore("Dijkstra");
        private readonly ButtonCore _bellmanFord = new ButtonCore("Bellman-Ford");
        private readonly ButtonCore _floyd = new ButtonCore("Floyd");

        private readonly NamedLabel _firstVertex =
            new NamedLabel("First Vertex: ", 1.ToString());
        private readonly NamedLabel _secondVertex =
            new NamedLabel("Second Vertex: ", 1.ToString());
        
        private readonly int _vertices;

        public event EventHandler<GotCorrectFindPathEventArgs> GotCorrectFindPath;
        
        public ShortestPathDialog(int vertices)
        {
            SetItemName("ShortestPathDialog_Unique");
            _vertices = vertices;
            
            _info.SetStyle(Styles.GetLabelStyle());
            _output.SetStyle(Styles.GetLabelStyle());
            _dijkstra.SetStyle(Styles.GetDialogButtonStyle());
            _bellmanFord.SetStyle(Styles.GetDialogButtonStyle());
            _floyd.SetStyle(Styles.GetDialogButtonStyle());
        }

        public override void InitElements()
        {
            base.InitElements();
            
            Window.SetBackground(45, 45, 45);
            Window.SetHeight(300);
            Window.SetWidth(250);
            
            var title = UiElements.GetDialogWindowTitleBar("Find Shortest Path");
            var layout = UiElements.GetDialogWindowLayout(title.GetHeight());

            _info.SetText("Enter vertices to find path between:");
            _dijkstra.EventMouseClick += OnDijkstraButtonClick;
            _bellmanFord.EventMouseClick += OnBellmanFordButtonClick;
            _floyd.EventMouseClick += OnFloydButtonClick;
            
            Window.AddItems(title, layout);

            layout.AddItems(
                _info,
                _firstVertex,
                _secondVertex,
                _dijkstra,
                _bellmanFord,
                _floyd,
                _output);

            title.GetCloseButton().EventMouseClick = (sender, args) => { Close(); };
        }

        public override void Show(CoreWindow handler)
        {
            _firstVertex.SetText(string.Empty);
            _secondVertex.SetText(string.Empty);
            base.Show(handler);
            _firstVertex.SetFocus();
        }

        public override void Close()
        {
            OnCloseDialog?.Invoke();
            base.Close();
        }

        public void SetData(string data)
        {
            _output.SetText(data);
        }

        private void OnDijkstraButtonClick(IItem sender, MouseArgs args)
        {
            OnButtonClick(true, false, false);
        }
        
        private void OnBellmanFordButtonClick(IItem sender, MouseArgs args)
        {
            OnButtonClick(false, true, false);
        }

        private void OnFloydButtonClick(IItem sender, MouseArgs args)
        {
            OnButtonClick(false, false, true);
        }

        private void OnButtonClick(bool dijkstra, bool bellmanFord, bool floyd)
        {
            if (ValidateInput())
            {
                var eventArgs = new GotCorrectFindPathEventArgs
                {
                    FirstVertex = Int32.Parse(_firstVertex.GetText()),
                    SecondVertex = Int32.Parse(_secondVertex.GetText()),
                    Dijkstra = dijkstra,
                    BellmanFord = bellmanFord,
                    Floyd = floyd
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
        
        private bool ValidateInput()
        {
            if (!(Int32.TryParse(_firstVertex.GetText(), out int first) &&
                  Int32.TryParse(_secondVertex.GetText(), out int second)))
                return false;
            return first >= 1 && first <= _vertices && second >= 1 && second <= _vertices;
        }
        
        protected virtual void OnGotCorrectInput(GotCorrectFindPathEventArgs e)
        {
            var handler = GotCorrectFindPath;
            handler?.Invoke(this, e);
        }
    }
    
    public class GotCorrectFindPathEventArgs : EventArgs
    {
        public int FirstVertex { get; set; }
        public int SecondVertex { get; set; }
        public bool Dijkstra { get; set; }
        public bool BellmanFord { get; set; }
        public bool Floyd { get; set; }
    }
}
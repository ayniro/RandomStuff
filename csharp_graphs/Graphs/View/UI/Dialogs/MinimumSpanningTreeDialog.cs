using System;
using Graphs.Model;
using Graphs.Presenter;
using SpaceVIL;
using SpaceVIL.Core;

namespace Graphs.View.UI.Dialogs
{
    public class MinimumSpanningTreeDialog : DialogItem, IPresenterConnectedDialog
    {
        private readonly Label _info = new Label();
        private readonly Label _output = new Label();
        private readonly ButtonCore _prim = new ButtonCore("Prim");
        private readonly ButtonCore _kruskal = new ButtonCore("Kruskal");
        private readonly ButtonCore _total = new ButtonCore("Total Amount");

        //private readonly int _vertices;

        public event EventHandler GotPrimRequest;
        public event EventHandler GotKruskalRequest;
        public event EventHandler GotTotalRequest;
        
        public MinimumSpanningTreeDialog()
        {
            SetItemName("ShortestPathDialog_Unique");
            //_vertices = vertices;
            
            _info.SetStyle(Styles.GetLabelStyle());
            _output.SetStyle(Styles.GetLabelStyle());
            _prim.SetStyle(Styles.GetDialogButtonStyle());
            _kruskal.SetStyle(Styles.GetDialogButtonStyle());
            _total.SetStyle(Styles.GetDialogButtonStyle());
        }

        public override void InitElements()
        {
            base.InitElements();
            
            Window.SetBackground(45, 45, 45);
            Window.SetHeight(250);
            Window.SetWidth(450);
            
            var title = UiElements.GetDialogWindowTitleBar("Find Shortest Path");
            var layout = UiElements.GetDialogWindowLayout(title.GetHeight());

            _info.SetText("Find SST in undirected graph made from generated one:");
            _prim.EventMouseClick += OnPrimButtonClick;
            _kruskal.EventMouseClick += OnKruskalButtonClick;
            _total.EventMouseClick += OnTotalButtonClick;
            
            Window.AddItems(title, layout);

            layout.AddItems(
                _info,
                _prim,
                _kruskal,
                _total,
                _output);

            title.GetCloseButton().EventMouseClick = (sender, args) => { Close(); };
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

        private void OnPrimButtonClick(IItem sender, MouseArgs args)
        {
            OnGotPrimRequest();
            Close();
        }
        
        private void OnKruskalButtonClick(IItem sender, MouseArgs args)
        {
            OnGotKruskalRequest();
            Close();
        }

        private void OnTotalButtonClick(IItem sender, MouseArgs args)
        {
            OnGotTotalRequest();
            Close();
        }

        protected virtual void OnGotPrimRequest()
        {
            var handler = GotPrimRequest;
            handler?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnGotKruskalRequest()
        {
            var handler = GotKruskalRequest;
            handler?.Invoke(this, EventArgs.Empty);
        }
        
        protected virtual void OnGotTotalRequest()
        {
            var handler = GotTotalRequest;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
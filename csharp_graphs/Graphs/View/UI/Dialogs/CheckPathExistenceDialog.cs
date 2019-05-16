using System;
using System.Drawing;
using Graphs.Presenter;
using SpaceVIL;
using SpaceVIL.Core;

namespace Graphs.View.UI.Dialogs
{
    public class CheckPathExistenceDialog : DialogItem, IPresenterConnectedDialog
    {
        private readonly Label _info = new Label();
        private readonly Label _output = new Label();
        private readonly NamedLabel _firstVertex =
            new NamedLabel("First Vertex: ", 1.ToString());
        private readonly NamedLabel _secondVertex =
            new NamedLabel("Second Vertex: ", 1.ToString());
        private readonly ButtonCore _check = new ButtonCore("Check");
        
        private readonly int _vertices;
        
        
        public event EventHandler<GotCorrectCheckPathEventArgs> GotCorrectCheckPath;
        
        public CheckPathExistenceDialog(int vertices)
        {
            SetItemName("CheckPathDialog_Unique");
            _vertices = vertices;
            
            _info.SetStyle(Styles.GetLabelStyle());
            _output.SetStyle(Styles.GetLabelStyle());
            _check.SetStyle(Styles.GetButtonStyle());
        }
        
        public override void InitElements()
        {
            base.InitElements();

            Window.SetBackground(45, 45, 45);
            Window.SetHeight(300);
            Window.SetWidth(250);
            //Window.IsLocked = true;

            var title = UiElements.GetDialogWindowTitleBar("Check Path Existence");
            var layout = UiElements.GetDialogWindowLayout(title.GetHeight());

            _info.SetText("Check if there is path between vertices:");
            _check.SetShadow(5, 0, 4, Color.FromArgb(150, 0, 0, 0));
            _check.EventMouseClick += OnCheckButtonClick;
            _check.SetWidth(100);
            
            Window.AddItems(title, layout);

            layout.AddItems(
                _info,
                _firstVertex,
                _secondVertex,
                _check,
                _output);

            title.GetCloseButton().EventMouseClick = (sender, args) => { Close(); };
        }
        
        public void SetData(string data)
        {
            _output.SetText(data);
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
        
        private void OnCheckButtonClick(IItem sender, MouseArgs args)
        {
            if (ValidateInput())
            {
                var eventArgs = new GotCorrectCheckPathEventArgs
                {
                    FirstVertex = Int32.Parse(_firstVertex.GetText()),
                    SecondVertex = Int32.Parse(_secondVertex.GetText())
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
        
        protected virtual void OnGotCorrectInput(GotCorrectCheckPathEventArgs e)
        {
            var handler = GotCorrectCheckPath;
            handler?.Invoke(this, e);
        }
    }
    
    public class GotCorrectCheckPathEventArgs : EventArgs
    {
        public int FirstVertex { get; set; }
        public int SecondVertex { get; set; }
    }
}
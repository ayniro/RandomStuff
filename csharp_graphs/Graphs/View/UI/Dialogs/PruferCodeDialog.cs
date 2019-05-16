using System;
using System.Collections.Generic;
using System.Linq;
using SpaceVIL;
using SpaceVIL.Core;

namespace Graphs.View.UI.Dialogs
{
    public class PruferCodeDialog : DialogItem
    {
        private readonly Label _info = new Label();
        private readonly NamedLabel _pruferCode =
            new NamedLabel("Prufer Code: ", String.Empty);
        
        private readonly ButtonCore _generate = new ButtonCore("Generate");
        private readonly Label _output = new Label();
        
        public event EventHandler<GotCorrectPruferEventArgs> GotCorrectPruferInput;

        public PruferCodeDialog()
        {
            SetItemName("PruferCodeDialog_Unique");

            _info.SetStyle(Styles.GetLabelStyle());
            _generate.SetStyle(Styles.GetDialogButtonStyle());
            _output.SetStyle(Styles.GetLabelStyle());
        }
        
        public override void InitElements()
        {
            base.InitElements();
            
            Window.SetBackground(45, 45, 45);
            Window.SetHeight(250);
            Window.SetWidth(400);
            
            var title = UiElements.GetDialogWindowTitleBar("Decode Prufer's Code");
            var layout = UiElements.GetDialogWindowLayout(title.GetHeight());
            layout.SetSpacing(vertical: 15);

            _info.SetText("Enter Prufer's code or 'Empty':");
            _generate.EventMouseClick += OnGenerateButtonClick;
            
            Window.AddItems(title, layout);

            layout.AddItems(
                _info,
                _pruferCode,
                _generate,
                _output);

            title.GetCloseButton().EventMouseClick = (sender, args) => { Close(); };
        }

        public override void Close()
        {
            OnCloseDialog?.Invoke();
            base.Close();
        }
        
        protected virtual void OnGotCorrectPruferInput(GotCorrectPruferEventArgs e)
        {
            var handler = GotCorrectPruferInput;
            handler?.Invoke(this, e);
        }

        private bool ValidateInput()
        {
            if (_pruferCode.GetText().Equals("Empty")) return true;
            var values = _pruferCode.GetText().Split(" ");
            int elementsAmount = values.Length;

            if (elementsAmount == 0 || elementsAmount > 25) return false;
            
            foreach (string value in values)
            {
                if (!Int32.TryParse(value, out int number)) return false;
                if (number < 1 || number > elementsAmount + 2) return false;
            }

            return true;
        }

        private List<int> GetCode()
        {
            if (_pruferCode.GetText().Equals("Empty")) return new List<int>();
            var values = _pruferCode.GetText().Split(" ");
            var code = values.Select(Int32.Parse).ToList();
            return code;
        }
        
        private void OnGenerateButtonClick(IItem sender, MouseArgs args)
        {
            if (ValidateInput())
            {
                var eventArgs = new GotCorrectPruferEventArgs
                {
                    Code = GetCode()
                };
                OnGotCorrectPruferInput(eventArgs);
                Close();
            }
            else
            {
                _output.SetText("Incorrect Input");
            }
        }
    }

    public class GotCorrectPruferEventArgs
    {
        public List<int> Code { get; set; }
    }
}
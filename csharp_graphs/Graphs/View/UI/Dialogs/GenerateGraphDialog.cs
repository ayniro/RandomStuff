using System;
using System.Drawing;
using System.Globalization;
using SpaceVIL;
using SpaceVIL.Core;
using SpaceVIL.Decorations;

namespace Graphs.View.UI.Dialogs
{
    public class GenerateGraphDialog : DialogItem
    {
        private readonly int _maxVertices;
        private readonly Label _info = new Label();
        private readonly Label _verticesInfo = new Label();
        private readonly Label _densityInfo = new Label();
        private readonly Label _weightInfo = new Label();
        private readonly Label _errorOutput = new Label();

        private readonly NamedLabel _verticesAmount =
            new NamedLabel("Vertices Amount: ", 2.ToString());

        private readonly NamedLabel _density =
            new NamedLabel("Density: ", 0.5.ToString(CultureInfo.CurrentCulture));
        
        private readonly NamedLabel _minWeight = 
            new NamedLabel("Min Weight: ", 0.ToString());
        
        private readonly NamedLabel _maxWeight = 
            new NamedLabel("Max Weight: ", 20.ToString());

        private readonly ButtonCore _generate = new ButtonCore("Generate");


        public event EventHandler<GotCorrectInputEventArgs> GotCorrectInput;

        public GenerateGraphDialog(int maxVertices)
        {
            _maxVertices = maxVertices;
            SetItemName("GenerateGraphDialog_Unique");

            _info.SetStyle(Styles.GetLabelStyle());
            _verticesInfo.SetStyle(Styles.GetLabelStyle());
            _densityInfo.SetStyle(Styles.GetLabelStyle());
            _weightInfo.SetStyle(Styles.GetLabelStyle());
            _errorOutput.SetStyle(Styles.GetLabelStyle());
            _generate.SetStyle(Styles.GetDialogButtonStyle());
        }

        public override void InitElements()
        {
            base.InitElements();

            Window.SetBackground(45, 45, 45);
            Window.SetHeight(450);
            Window.SetWidth(400);
            //Window.IsLocked = true;

            var title = UiElements.GetDialogWindowTitleBar("Generate Graph");
            var layout = UiElements.GetDialogWindowLayout(title.GetHeight());

            _info.SetText("Enter Graph Parameters:");
            _verticesInfo.SetText($"Vertices: 2 <= x <= {_maxVertices}");
            _densityInfo.SetText("Density: 0 <= x <= 1 ");
            _weightInfo.SetText("-100 <= Min Weight <= Max Weight <= 100");
            _generate.SetShadow(5, 0, 4, Color.FromArgb(150, 0, 0, 0));
            _generate.EventMouseClick += OnGenerateButtonClick;

            Window.AddItems(title, layout);
            layout.AddItems(
                _info,
                _verticesInfo,
                _densityInfo,
                _weightInfo,
                _verticesAmount,
                _density,
                _minWeight,
                _maxWeight,
                _errorOutput,
                _generate);

            title.GetCloseButton().EventMouseClick = (sender, args) => { Close(); };
        }

        public override void Show(CoreWindow handler)
        {
            _verticesAmount.SetText(string.Empty);
            _density.SetText(string.Empty);
            _minWeight.SetText(string.Empty);
            _maxWeight.SetText(string.Empty);
            base.Show(handler);
            _verticesAmount.SetFocus();
        }

        public override void Close()
        {
            OnCloseDialog?.Invoke();
            base.Close();
        }

        private bool ValidateInput()
        {
            if (!(Int32.TryParse(_verticesAmount.GetText(), out int vertices) &&
                  Double.TryParse(_density.GetText(), out double density) &&
                  Int32.TryParse(_minWeight.GetText(), out int minWeight) &&
                  Int32.TryParse(_maxWeight.GetText(), out int maxWeight)))
                return false;

            return vertices >= 2 && 
                   vertices <= _maxVertices && 
                   density >= 0 && 
                   density <= 1 &&
                   minWeight >= -100 &&
                   minWeight <= 100 && 
                   maxWeight >= -100 && 
                   maxWeight <= 100 && 
                   minWeight <= maxWeight;
        }

        protected virtual void OnGotCorrectInput(GotCorrectInputEventArgs e)
        {
            var handler = GotCorrectInput;
            handler?.Invoke(this, e);
        }

        private void OnGenerateButtonClick(IItem sender, MouseArgs args)
        {
            if (ValidateInput())
            {
                var eventArgs = new GotCorrectInputEventArgs
                {
                    VerticesAmount = Int32.Parse(_verticesAmount.GetText()),
                    Density = Double.Parse(_density.GetText()),
                    MinWeight = Int32.Parse(_minWeight.GetText()),
                    MaxWeight = Int32.Parse(_maxWeight.GetText())
                };
                OnGotCorrectInput(eventArgs);
                Close();
            }
            else
            {
                _errorOutput.SetText("Incorrect Input");
            }
        }
    }

    public class GotCorrectInputEventArgs : EventArgs
    {
        public int VerticesAmount { get; set; }
        public double Density { get; set; }
        public int MinWeight { get; set; }
        public int MaxWeight { get; set; }
    }
}
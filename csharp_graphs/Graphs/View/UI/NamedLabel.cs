using System.Drawing;
using SpaceVIL;
using SpaceVIL.Core;
using SpaceVIL.Decorations;

namespace Graphs.View.UI
{
    public class NamedLabel : Prototype
    {
        private string Name { get; }
        private readonly TextEdit _text;

        public NamedLabel(string name, string initialValue = "")
        {
            Name = name;

            _text = new TextEdit();
            _text.SetText(initialValue);
        }

        public override void InitElements()
        {
            base.InitElements();

            SetBackground(0, 0, 0, 0);
            SetAlignment(ItemAlignment.HCenter);
            SetBorder(new Border(Color.DimGray, new CornerRadius(10), 1));
            SetSize(280, 30);
            
            var layout = new HorizontalStack();
            layout.SetBackground(Color.FromArgb(0, 0, 0, 0));
            
            var name = new Label(Name);
            name.SetStyle(Styles.GetLabelStyle());
            name.SetTextAlignment(ItemAlignment.Left, ItemAlignment.VCenter);
            name.SetPadding(5);
            name.SetWidth(Name.Length * 8);
            name.SetWidthPolicy(SizePolicy.Fixed);

            _text.SetAlignment(ItemAlignment.Right, ItemAlignment.VCenter);
            _text.SetPadding(10, 0, 5, 0);
            _text.SetMaxWidth(250);
            _text.SetBackground(255, 255, 255, 100);
            _text.SetForeground(Color.White);
            _text.SetBorder(new Border(Color.DimGray, new CornerRadius(10), 1));
            _text.SetWidthPolicy(SizePolicy.Expand);
            
            AddItem(layout);
            layout.AddItems(name, _text);
        }

        public void SetText(string s)
        {
            _text.SetText(s);
        }

        public string GetText()
        {
            return _text.GetText();
        }

        public override void SetFocus()
        {
            _text.SetFocus();
        }
    }
}
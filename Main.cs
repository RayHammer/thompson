using System;
using Godot;

namespace Thompson
{
    public partial class Main : PanelContainer
    {
        [Export]
        private CaptionTextLabel _preview;

        [Export]
        private TextEdit _textLog;

        [Export]
        private LineEdit _input;

        [Export]
        private Button _submitButton;

        public override void _Ready()
        {
            _input.GrabFocus();
        }

        private void OnInputTextSubmitted(String text)
        {
            if (_input.Text.Length == 0)
                return;
            _preview.DisplayText(text);
            _textLog.Text += $"{text}\n";
            _input.Clear();
        }

        private void OnSubmitButtonPressed()
        {
            _input.EmitSignal(LineEdit.SignalName.TextSubmitted, _input.Text);
        }
    }
}

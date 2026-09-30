using System;
using System.Collections.Generic;
using Godot;

namespace Thompson
{
    public struct CaptionNode
    {
        public int Position;
        public float Delay;

        public CaptionNode(int position, float delay = 0.5f)
        {
            Position = position;
            Delay = delay;
        }

        public override string ToString() => $"({Position}, {Delay})";
    }

    public partial class CaptionTextLabel : RichTextLabel
    {
        private Timer _typeTimer;
        private List<CaptionNode> _timeline = new();
        private IEnumerator<CaptionNode> _timelineEnum;

        private float _delayBetweenWords = 0.2f;
        private float _delayComma = 0.2f;
        private float _delayFullStop = 0.5f;

        public override void _Ready()
        {
            if (_typeTimer == null)
            {
                _typeTimer = new Timer();
                _typeTimer.WaitTime = 1;
                _typeTimer.OneShot = true;
                _typeTimer.Timeout += OnTypeTimerTimeout;
                AddChild(_typeTimer);
            }
        }

        private void OnTypeTimerTimeout()
        {
            if (_timelineEnum.MoveNext())
            {
                GD.Print($"Progressing to {_timelineEnum.Current}");
                VisibleCharacters = _timelineEnum.Current.Position;
                _typeTimer.WaitTime = _timelineEnum.Current.Delay;
                _typeTimer.Start();
            }
        }

        public void DisplayText(string text)
        {
            Text = text;
            ProcessString(Text);
            // GD.Print($"Sending {Text} to caption");
            _timelineEnum = _timeline.GetEnumerator();
            if (_timelineEnum.MoveNext())
            {
                VisibleCharacters = _timelineEnum.Current.Position;
                _typeTimer.WaitTime = _timelineEnum.Current.Delay;
                _typeTimer.Start();
            }
        }

        public void ProcessString(string s)
        {
            char[] timelineSymbols = [' ', ',', '.', '?', '!'];

            GD.Print("Processing Started...");
            _timeline.Clear();
            _timeline.Add(new CaptionNode(0, 0.1f));
            int i = 0;
            while ((i = s.IndexOfAny(timelineSymbols, i)) != -1)
            {
                int includedPunctuation = 1;
                float delay = _delayBetweenWords;
                switch (s[i])
                {
                    case ' ':
                        includedPunctuation = 0;
                        break;
                    case ',':
                        delay = _delayComma;
                        break;
                    case '.':
                    case '?':
                    case '!':
                        delay = _delayFullStop;
                        break;
                    default:
                        break;
                }
                _timeline.Add(new CaptionNode(i + includedPunctuation, delay));
                i++;
            }
            _timeline.Add(new CaptionNode(s.Length, _delayBetweenWords));
            GD.Print("Processing Ended");
        }
    }
}

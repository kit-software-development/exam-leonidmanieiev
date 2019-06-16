using Net.TCP;
using System;
using System.Text;

namespace Net.Messaging
{
    [Serializable]
    public class TextMessage : IMessage
    {
        public string _text { get; }

        public TextMessage(string text)
        {
            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }
    }
}

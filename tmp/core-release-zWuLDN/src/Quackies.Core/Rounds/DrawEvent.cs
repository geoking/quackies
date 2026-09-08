using System;

namespace Quackies.Core.Rounds
{
    public sealed class DrawEvent
    {
        public DrawEvent(DrawEventType type, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Type = type;
            Message = message;
        }

        public DrawEventType Type { get; }

        public string Message { get; }

        public override string ToString()
        {
            return Message;
        }
    }
}

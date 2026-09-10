using UnityEngine;

namespace Quackies.Unity.Presentation
{
    public static class TableTheme
    {
        public static readonly Color Background = Hex(0x122220);
        public static readonly Color Panel = Hex(0x203532);
        public static readonly Color Raised = Hex(0x2B4440);
        public static readonly Color Ink = Hex(0xF4EBD7);
        public static readonly Color Muted = Hex(0xA7B7A9);
        public static readonly Color Gold = Hex(0xEDCB82);
        public static readonly Color Teal = Hex(0x70C8B0);
        public static readonly Color Red = Hex(0xE9907B);

        private static Color Hex(int value) => new Color(((value >> 16) & 255) / 255f,
            ((value >> 8) & 255) / 255f, (value & 255) / 255f);
    }
}

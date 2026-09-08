using System;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules
{
    /// <summary>Immutable component data for one chip stack in the shared supply.</summary>
    public sealed class ShopChipDefinition
    {
        public ShopChipDefinition(TokenColor color, int value, int price, int stock, int availableFromRound)
        {
            if (color == TokenColor.White) throw new ArgumentException("White chips are not sold in the ingredient market.", nameof(color));
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            if (price <= 0) throw new ArgumentOutOfRangeException(nameof(price));
            if (stock < 0) throw new ArgumentOutOfRangeException(nameof(stock));
            if (availableFromRound < 1 || availableFromRound > 9) throw new ArgumentOutOfRangeException(nameof(availableFromRound));

            Color = color;
            Value = value;
            Price = price;
            Stock = stock;
            AvailableFromRound = availableFromRound;
        }

        public TokenColor Color { get; }
        public int Value { get; }
        public int Price { get; }
        public int Stock { get; }
        public int AvailableFromRound { get; }
    }
}

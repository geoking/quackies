using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Quackies.Core.Rules.Ingredients;
using Quackies.Core.Tokens;

namespace Quackies.Core.Rules
{
    /// <summary>
    /// Immutable rules and component data. MatchSession copies stock counts into
    /// match-owned state, so one RuleSet may safely be reused for many matches.
    /// </summary>
    public sealed class RuleSet
    {
        private readonly IReadOnlyDictionary<TokenColor, IIngredientRule> _ingredients;
        private readonly IReadOnlyList<ShopChipDefinition> _shopChips;
        private readonly IReadOnlyList<IRoundEventRule> _roundEvents;

        public RuleSet(
            BoardTrack track,
            IEnumerable<IIngredientRule> ingredients,
            IEnumerable<ShopChipDefinition> shopChips,
            IEnumerable<IRoundEventRule>? roundEvents = null)
        {
            Track = track ?? throw new ArgumentNullException(nameof(track));
            if (ingredients == null) throw new ArgumentNullException(nameof(ingredients));
            if (shopChips == null) throw new ArgumentNullException(nameof(shopChips));

            var ingredientMap = ingredients.ToDictionary(rule => rule.Color);
            foreach (TokenColor color in Enum.GetValues(typeof(TokenColor)))
                if (!ingredientMap.ContainsKey(color))
                    throw new ArgumentException($"No ingredient rule was registered for {color}.", nameof(ingredients));

            var chips = shopChips.ToList();
            if (chips.GroupBy(chip => (chip.Color, chip.Value)).Any(group => group.Count() != 1))
                throw new ArgumentException("Each shop color/value pair must be registered exactly once.", nameof(shopChips));

            var events = (roundEvents ?? Array.Empty<IRoundEventRule>()).ToList();
            if (events.Any(card => string.IsNullOrWhiteSpace(card.Id)))
                throw new ArgumentException("Every round event needs a stable ID.", nameof(roundEvents));
            if (events.GroupBy(card => card.Id, StringComparer.Ordinal).Any(group => group.Count() != 1))
                throw new ArgumentException("Round event IDs must be unique.", nameof(roundEvents));

            _ingredients = new ReadOnlyDictionary<TokenColor, IIngredientRule>(ingredientMap);
            _shopChips = new ReadOnlyCollection<ShopChipDefinition>(chips);
            _roundEvents = new ReadOnlyCollection<IRoundEventRule>(events);
        }

        public BoardTrack Track { get; }
        public IReadOnlyDictionary<TokenColor, IIngredientRule> Ingredients => _ingredients;
        public IReadOnlyList<ShopChipDefinition> ShopChips => _shopChips;
        public IReadOnlyList<IRoundEventRule> RoundEvents => _roundEvents;

        public static RuleSet SetOne(IEnumerable<IRoundEventRule>? roundEvents = null)
        {
            // Stocks are the publisher's base-box quantities after removing the
            // two orange 1 and two green 1 chips placed in starting bags.
            var chips = new[]
            {
                new ShopChipDefinition(TokenColor.Orange, 1, 3, 18, 1),
                new ShopChipDefinition(TokenColor.Green, 1, 4, 13, 1),
                new ShopChipDefinition(TokenColor.Green, 2, 8, 10, 1),
                new ShopChipDefinition(TokenColor.Green, 4, 14, 13, 1),
                new ShopChipDefinition(TokenColor.Blue, 1, 5, 14, 1),
                new ShopChipDefinition(TokenColor.Blue, 2, 10, 10, 1),
                new ShopChipDefinition(TokenColor.Blue, 4, 19, 10, 1),
                new ShopChipDefinition(TokenColor.Red, 1, 6, 12, 1),
                new ShopChipDefinition(TokenColor.Red, 2, 10, 8, 1),
                new ShopChipDefinition(TokenColor.Red, 4, 16, 10, 1),
                new ShopChipDefinition(TokenColor.Yellow, 1, 8, 13, 2),
                new ShopChipDefinition(TokenColor.Yellow, 2, 12, 6, 2),
                new ShopChipDefinition(TokenColor.Yellow, 4, 18, 10, 2),
                new ShopChipDefinition(TokenColor.Purple, 1, 9, 15, 3),
                new ShopChipDefinition(TokenColor.Black, 1, 10, 18, 1)
            };
            return new RuleSet(BoardTrack.Standard(), SetOneIngredients.Create(), chips, roundEvents);
        }
    }
}

using System;
using System.Linq;
using Quackies.Core.Tokens;
using UnityEngine;

namespace Quackies.Unity.Art
{
    /// <summary>
    /// Presentation assets for a particular board and ingredient-book selection.
    /// Rules never depend on this catalog. Future editions can supply another catalog
    /// without changing either the game model or the views that consume these lookups.
    /// </summary>
    [CreateAssetMenu(menuName = "Quackies/Art Catalog")]
    public sealed class QuackiesArtCatalog : ScriptableObject
    {
        [Serializable]
        public sealed class TokenArt
        {
            public TokenColor Color;
            public int Value;
            public Sprite Sprite;
        }

        [Serializable]
        public sealed class BookArt
        {
            public TokenColor Color;
            public Sprite Sprite;
        }

        [Serializable]
        public sealed class FortuneArt
        {
            public string Title;
            public Sprite Sprite;
        }

        [Serializable]
        public sealed class PlayerArt
        {
            public Sprite Cauldron;
            public Sprite Counter;
            public Sprite Droplet;
            public Sprite Rat;
            public Sprite FullFlask;
            public Sprite EmptyFlask;
        }

        [SerializeField] private TokenArt[] tokens = Array.Empty<TokenArt>();
        [SerializeField] private BookArt[] books = Array.Empty<BookArt>();
        [SerializeField] private FortuneArt[] fortunes = Array.Empty<FortuneArt>();
        [SerializeField] private PlayerArt human = new PlayerArt();
        [SerializeField] private PlayerArt opponent = new PlayerArt();
        [SerializeField] private Sprite roundBoard;
        [SerializeField] private Sprite roundMarker;
        [SerializeField] private Sprite cardBack;
        [SerializeField] private Sprite fortuneCardsAtlas;
        [SerializeField] private Sprite die;
        [Tooltip("Physical spaces in draw order, measured from the cropped sprite's bottom-left. The last entry is the spoon scoring space.")]
        [SerializeField] private Vector2[] potSpaces = Array.Empty<Vector2>();
        [Tooltip("Chip diameter relative to the cauldron sprite's width.")]
        [SerializeField] private float potChipWidth = 84f / 1593f;

        public Sprite HumanCauldron => human.Cauldron;
        public Sprite OpponentCauldron => opponent.Cauldron;
        public Sprite HumanCounter => human.Counter;
        public Sprite OpponentCounter => opponent.Counter;
        public Sprite HumanDroplet => human.Droplet;
        public Sprite OpponentDroplet => opponent.Droplet;
        public Sprite HumanRat => human.Rat;
        public Sprite OpponentRat => opponent.Rat;
        public Sprite HumanFlask => human.FullFlask;
        public Sprite OpponentFlask => opponent.FullFlask;
        public Sprite HumanEmptyFlask => human.EmptyFlask;
        public Sprite OpponentEmptyFlask => opponent.EmptyFlask;
        public Sprite RoundBoard => roundBoard;
        public Sprite RoundMarker => roundMarker;
        public Sprite CardBack => cardBack;
        public Sprite FortuneCardsAtlas => fortuneCardsAtlas;
        public Sprite Die => die;
        public float PotChipWidth => potChipWidth;

        /// <summary>A copy protects the authored alignment data from a view accidentally changing it.</summary>
        public Vector2[] PotSpaces => (Vector2[])potSpaces.Clone();
        public Sprite[] FortuneCardSprites => fortunes.Select(entry => entry.Sprite).ToArray();

        public Sprite GetTokenSprite(TokenColor color, int value)
        {
            foreach (var entry in tokens)
                if (entry.Color == color && entry.Value == value)
                    return entry.Sprite;
            return null;
        }

        public Sprite GetBookSprite(TokenColor color)
        {
            foreach (var entry in books)
                if (entry.Color == color)
                    return entry.Sprite;
            return null;
        }

        /// <summary>Matches printed titles irrespective of spaces, punctuation, and capitalization.</summary>
        public Sprite GetFortuneCardSprite(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return null;
            var key = NormalizeTitle(title);
            foreach (var entry in fortunes)
                if (NormalizeTitle(entry.Title) == key)
                    return entry.Sprite;
            return null;
        }

        /// <summary>
        /// Returns an anchor in the cropped pot artwork, with (0,0) at bottom-left.
        /// Index 0 is the starting droplet; 1..52 are chip spaces; 53 is the spoon.
        /// The index is a physical space, never the coin value printed in that space.
        /// </summary>
        public Vector2 GetPotAnchor(int physicalPosition)
        {
            if (physicalPosition < 0 || physicalPosition >= potSpaces.Length)
                throw new ArgumentOutOfRangeException(nameof(physicalPosition), physicalPosition,
                    "The physical position is outside this board's artwork.");
            return potSpaces[physicalPosition];
        }

        /// <summary>
        /// Use when the containing Image has preserveAspect=true. Pass its RectTransform.rect;
        /// place each marker at the returned localPosition. This includes any letterboxing.
        /// Alternatively, fit the entire marker parent to GetFittedPotRect and use GetPotAnchor.
        /// </summary>
        public Vector2 GetPotLocalPosition(int physicalPosition, Rect container, Sprite cauldron = null)
        {
            var fitted = GetFittedPotRect(container, cauldron);
            return fitted.min + Vector2.Scale(GetPotAnchor(physicalPosition), fitted.size);
        }

        public Rect GetFittedPotRect(Rect container, Sprite cauldron = null)
        {
            var sprite = cauldron != null ? cauldron : HumanCauldron;
            if (sprite == null) return container;
            var scale = Mathf.Min(container.width / sprite.rect.width, container.height / sprite.rect.height);
            var size = sprite.rect.size * scale;
            return new Rect(container.center - size * 0.5f, size);
        }

        private static string NormalizeTitle(string title)
        {
            return new string(title.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor import entry point. The builder writes serialized sprite references once;
        /// a player build needs no AssetDatabase, raw paths, or runtime texture slicing.
        /// </summary>
        public void Configure(TokenArt[] tokenArt, BookArt[] bookArt, FortuneArt[] fortuneArt,
            PlayerArt humanArt, PlayerArt opponentArt, Sprite board, Sprite marker,
            Sprite back, Sprite atlas, Sprite dice, Vector2[] anchors)
        {
            tokens = tokenArt;
            books = bookArt;
            fortunes = fortuneArt;
            human = humanArt;
            opponent = opponentArt;
            roundBoard = board;
            roundMarker = marker;
            cardBack = back;
            fortuneCardsAtlas = atlas;
            die = dice;
            potSpaces = anchors;
        }
#endif
    }
}

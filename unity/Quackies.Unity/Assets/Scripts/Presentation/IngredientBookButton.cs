using Quackies.Core.Tokens;
using Quackies.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Quackies.Unity.Presentation
{
    /// <summary>Serialized ingredient-reference binding that survives generated scene reloads.</summary>
    [RequireComponent(typeof(Button))]
    public sealed class IngredientBookButton : MonoBehaviour
    {
        [SerializeField] private MatchPresenter presenter;
        [SerializeField] private TokenColor color;
        [SerializeField] private Button button;

        public void Configure(MatchPresenter owner, TokenColor ingredientColor, Button target)
        {
            presenter = owner;
            color = ingredientColor;
            button = target;
        }

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            button.onClick.AddListener(OpenBook);
        }

        private void OpenBook()
        {
            if (presenter != null) presenter.ShowIngredientBook(color);
        }
    }
}

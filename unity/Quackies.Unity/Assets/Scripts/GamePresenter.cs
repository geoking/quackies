using UnityEngine;
using TMPro;

// Replace these with your real Core namespaces.
using Quackies.Core.Game;
using Quackies.Core.Randomness;

public sealed class GamePresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private QuackiesGame game;

    private void Awake()
    {
        var random = new SeededRandomSource(12345);

        // Replace this with your real Core creation method.
        game = QuackiesGame.CreateSinglePlayer(random);

        Refresh();
    }

    public void DrawToken()
    {
        // Replace with your real method.
        game.DrawToken();

        Refresh();
    }

    public void StopRound()
    {
        // Replace with your real method.
        game.StopRound();

        Refresh();
    }

    private void Refresh()
    {
        // Replace this with your actual Core state shape.
        var player = game.CurrentPlayer;
        var cauldron = player.Cauldron;

        statusText.text =
            $"Position: {cauldron.CurrentPosition}\n" +
            $"White total: {cauldron.WhiteChipTotal} / 7\n" +
            $"Exploded: {cauldron.HasExploded}\n" +
            $"Bag count: {player.Bag.Count}\n" +
            $"Victory points: {player.VictoryPoints}\n" +
            $"Rubies: {player.Rubies}\n" +
            $"Buying power: {player.BuyingPowerAvailableThisRound}";
    }
}
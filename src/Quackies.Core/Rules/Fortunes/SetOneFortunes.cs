using System.Collections.Generic;

namespace Quackies.Core.Rules.Fortunes
{
    public static partial class SetOneFortunes
    {
        /// <summary>All 24 base-game fortune cards in the supplied atlas order.</summary>
        public static IEnumerable<IRoundEventRule> CreateAll()
        {
            yield return new ASecondChance();
            yield return new JustInTime();
            yield return new WellStirred();
            yield return new LuckyDevil();
            yield return new ItsShiningExtraBright();
            yield return new SeasonedPerfectly();
            yield return new LessIsMore();
            yield return new ThePotIsFull();
            yield return new ChooseWisely();
            yield return new LivingInLuxury();
            yield return new AnOpportunisticMoment();
            yield return new RollTheDie();
            yield return new PumpkinPatchParty();
            yield return new YouOnlyGetToChooseOne();
            yield return new StrongIngredient();
            yield return new MagicPotion();
            yield return new AGoodStart();
            yield return new RatInfestation();
            yield return new RatsAreYourFriends();
            yield return new Charity();
            yield return new ThePotIsFillingUp();
            yield return new WheelAndDeal();
            yield return new BeginnersBonus();
            yield return new Schadenfreude();
        }
    }
}

namespace Models.Bonuses
{
    public class Bonus
    {
        public Bonus(BonusType type, int amount)
        {
            Type = type;
            Amount = amount;
        }

        public BonusType Type { get; }

        public int Amount { get; }
    }
}
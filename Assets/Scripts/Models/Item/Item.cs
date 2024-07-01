using System;
using Newtonsoft.Json;

namespace Demo.Models
{
    public class Item : IEquatable<Item>, IComparable<Item>
    {
        public static readonly Item Empty = new Item(ItemType.None, 0);

        public Item(ItemType type, int quantity)
        {
            Type = type;
            Quantity = quantity;
        }

        [JsonProperty("type")] public ItemType Type { get; }

        [JsonProperty("quantity")] public int Quantity { get; }

        public int CompareTo(Item other)
        {
            if (Type != other.Type)
            {
                throw new InvalidOperationException("Cannot compare items of different types");
            }

            return Quantity.CompareTo(other.Quantity);
        }

        public override string ToString() => $"{Type}={Quantity}";

        public bool Equals(Item other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Type == other.Type && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Quantity);
        }

        public static bool operator ==(Item a, Item b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a is not null && a.Equals(b);
        }

        public static bool operator !=(Item a, Item b) => !(a == b);

        public static bool operator <(Item a, Item b) => a.CompareTo(b) < 0;

        public static bool operator >(Item a, Item b) => a.CompareTo(b) > 0;

        public static bool operator <=(Item a, Item b) => a.CompareTo(b) <= 0;

        public static bool operator >=(Item a, Item b) => a.CompareTo(b) >= 0;

        public static Item operator -(in Item a, in Item b) =>
            new Item(a.Type, a.Quantity - b.Quantity);

        public static Item operator +(in Item a, in Item b) =>
            new Item(a.Type, a.Quantity + b.Quantity);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Demo.Models
{
    public class CompositeEntity : IEnumerable<Item>
    {
        [JsonProperty("elements")] private readonly Dictionary<ItemType, Item> _elements
            = new Dictionary<ItemType, Item>();

        [UsedImplicitly]
        private CompositeEntity()
        {
        }

        public CompositeEntity(params Item[] items)
        {
            var safeItems = items ?? Array.Empty<Item>();

            foreach (var item in safeItems)
            {
                _elements[item.Type] = item;
            }
        }

        public CompositeEntity(IEnumerable<Item> items)
        {
            var safeItems = items ?? Array.Empty<Item>();

            foreach (var item in safeItems)
            {
                _elements[item.Type] = item;
            }
        }

        public ICollection<Item> Items => _elements.Values;
        public IEnumerator<Item> GetEnumerator() => _elements.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _elements.Values.GetEnumerator();

        public int GetSafeAmount(ItemType type)
        {
            return _elements.TryGetValue(type, out var item)
                ? item.Quantity
                : 0;
        }

        public bool HasEnough(CompositeEntity others)
        {
            foreach (var item in others)
            {
                if (_elements.Any(c => c.Key == item.Type && c.Value < item))
                {
                    return false;
                }

                if (_elements.All(c => c.Key != item.Type && item.Quantity > 0))
                {
                    return false;
                }
            }

            return true;
        }

        public int CalculateDifference(CompositeEntity others, ItemType type)
        {
            var a = this.FirstOrDefault(c => c.Type == type)?.Quantity ?? 0;
            var b = others.FirstOrDefault(c => c.Type == type)?.Quantity ?? 0;

            return b - a;
        }

        public CompositeEntity CalculateDifference(CompositeEntity others)
        {
            var diffComposite = new List<Item>();

            foreach (var type in _elements.Keys.Union(others._elements.Keys).Distinct())
            {
                var a = this.FirstOrDefault(c => c.Type == type)?.Quantity ?? 0;
                var b = others.FirstOrDefault(c => c.Type == type)?.Quantity ?? 0;

                var diffAmount = b - a;
                diffComposite.Add(new Item(type, diffAmount));
            }

            return new CompositeEntity(diffComposite);
        }

        public CompositeEntity Increment(Item item) =>
            Increment(item.Type, item.Quantity);

        public CompositeEntity Increment(ItemType type, int amount)
        {
            var items = _elements.ToDictionary(p => p.Key, p => p.Value);

            if (items.TryGetValue(type, out var item))
            {
                items[type] = new Item(type, item.Quantity + amount);
            }
            else
            {
                items[type] = new Item(type, amount);
            }

            return new CompositeEntity(items.Values);
        }

        public CompositeEntity Increment(CompositeEntity others)
        {
            var items = _elements.ToDictionary(p => p.Key, p => p.Value);
            var accumulator = new CompositeEntity(items.Values);

            return others.Aggregate(accumulator, (current, item) =>
                current.Increment(item.Type, item.Quantity));
        }

        public CompositeEntity Decrement(Item item) =>
            Decrement(item.Type, item.Quantity);

        public CompositeEntity Decrement(ItemType type, int amount)
        {
            var items = _elements.ToDictionary(p => p.Key, p => p.Value);

            if (items.TryGetValue(type, out var item))
            {
                items[type] = new Item(type, Math.Max(item.Quantity - amount, 0));
            }

            return new CompositeEntity(items.Values);
        }

        public CompositeEntity Decrement(CompositeEntity others)
        {
            var items = _elements.ToDictionary(p => p.Key, p => p.Value);
            var accumulator = new CompositeEntity(items.Values);

            return others.Aggregate(accumulator, (current, item) =>
                current.Decrement(item.Type, item.Quantity));
        }
    }
}
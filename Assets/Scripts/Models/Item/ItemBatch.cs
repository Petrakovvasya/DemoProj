using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Demo.Models
{
    public class ItemBatch : IEquatable<ItemBatch>
    {
        public ItemBatch([CanBeNull] IEnumerable<Item> items)
        {
            Items = items;
        }

        [CanBeNull] public IEnumerable<Item> Items { get; }

        public bool Equals(ItemBatch other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return true;
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

            return Equals((ItemBatch)obj);
        }
    }
}
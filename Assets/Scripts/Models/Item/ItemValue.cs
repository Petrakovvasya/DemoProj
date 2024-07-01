using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Demo.Models
{
    public class ItemValue
    {
        [UsedImplicitly]
        private ItemValue()
        {
        }

        [JsonProperty("category")] public ItemType Category { get; private set; }
        [JsonProperty("group")] public string Group { get; private set; }
    }
}
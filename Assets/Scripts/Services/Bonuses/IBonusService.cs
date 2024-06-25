using Models;
using UnityEngine;

namespace Services.Bonuses
{
    public interface IBonusService
    {
        Sprite FindSprite(BonusType type);
        GameObject FindObject(BonusType type);
    }
}
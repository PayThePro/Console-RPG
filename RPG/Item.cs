using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG
{
    public class Item
    {
        public ItemType itemType;
        public bool Equipped = false;
        public string[] stats = new string[2];
    }

    public class Weapon : Item
    {
        public int dmg;
        public float hitChance;

        public Weapon(ItemType type, int Damage, float ChancheOfHitting)
        {
            dmg = Damage;
            hitChance = ChancheOfHitting;
            itemType = type;
            stats[0] = "Damage: " + dmg;
            stats[1] = "HitChance: " + hitChance;
        }
    }

    public class Armor : Item
    {
        public int deflectAmount;
        public float deflectChance;

        public Armor(ItemType type, int AmountOfDamageDeflected, float ChanceOfDeflecting)
        {
            deflectChance = ChanceOfDeflecting;
            deflectAmount = AmountOfDamageDeflected;
            itemType = type;
            stats[0] = "Chance: " + ChanceOfDeflecting;
            stats[1] = "Deflection: " + AmountOfDamageDeflected;
        }
    }

    public enum ItemType
    {
        //Weapons
        Sword,
        Bow,

        //Armor
        Helmet,
        Chest,
        Pants,
        Boots
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleRPG
{
    class Entity
    {
        public int hp;
        public int dmg;
        public string name;
    }

    class Player : Entity
    {
        public int healAmount;
        public int maxHealth;

        public Player(int HP, int HealAmount, string Name, int MaxHealth)
        {
            hp = HP;
            healAmount = HealAmount;
            name = Name;
            maxHealth = MaxHealth;
        }
    }

    class Enemy : Entity
    {
        public Enemy(int HP, int Dmg, string Name)
        {
            hp = HP;
            dmg = Dmg;
            name = Name;
        }
    }
}

using System;
using System.Collections.Generic;

namespace ConsoleRPG
{
    class Game
    {
        //Game variables
        static bool gameRunning = true;
        static List<string> promptMessages = new List<string>() {""};
        static Random random = new Random();
        public static Weapon weaponEquipped;
        public static Item bootsEquipped, pantsEquipped, chestEquipped, helmetEquipped;
        static Enemy currentEnemy = null;
        static Player player;
        public static int enemiesSlain = 0;

        //Inventory stuff
        static Item[] inventory = new Item[10];

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "An awesome RPG";

            //Gets player name
            GetPlayerInfo();

            //Sets starter loadout test
            Weapon weapon = new Weapon(ItemType.Sword, 2, 0.9f);
            inventory[0] = weapon;

            //Tutorial
            promptMessages.Add("available in combat");
            promptMessages.Add("Your inventory isn't");
            promptMessages.Add("new stuff.");
            promptMessages.Add("and play with your");
            promptMessages.Add("check the controls");
            promptMessages.Add("to start slaying or");
            promptMessages.Add("RPG, you can press P");
            promptMessages.Add("Welcome to an awesome");

            //Writes the scene
            WriteSceneBorders();
            WriteInventory();

            while (gameRunning) //Game loop
            {
                WritePrompt();
                GetInput();
                WriteInventory();
            }
            Console.Clear();
            Console.WriteLine("You were slain.\n\n" + "Total enemies killed: " + enemiesSlain);
            Console.ReadLine();
        }

        static void GetPlayerInfo()
        {
            Console.WriteLine("What is your name?");
            player = new Player(5, 5, Console.ReadLine(), 5);
            Console.Clear();
        }

        public static void WriteInventory()
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != null) //If item is there
                {
                    Write(9, 16 + i, inventory[i].Equipped ? "X" : " ", 1);
                    Write(17, 16 + i, inventory[i].itemType.ToString(), 7);
                }
                else //If item is not there clear item spot
                {
                    Write(9, 16 + i, " ", 1);
                    Write(17, 16 + i, "", 7);
                }
            }
        }

        static void Write(int left, int top, string message, int maxLength)
        {
            //Sets cursor location
            Console.SetCursorPosition(left, top);

            //Writes the message
            for (int i = 0; i < message.Length; i++)
            {
                if (i < maxLength)
                {
                    Console.Write(message[i]);
                }
            }

            //Writes the rest of the space to max lenght
            for (int n = 0; n < maxLength - message.Length; n++)
            {
                Console.Write(" ");
            }
        }

        static void Combat()
        {
            //Updates game screens
            WriteCombat();

            //Players turn
            bool gettingAction = true;
            while (gettingAction)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.A: //Attack
                        if (weaponEquipped == null)
                        {
                            promptMessages.Add("No weapon equipped");
                            WritePrompt();
                            break;
                        }
                        promptMessages.Add("Player attacks");
                        WritePrompt();
                        if (random.Next(10) / 10 < weaponEquipped.hitChance) // Compare against the currently equiped weapon
                        {
                            currentEnemy.hp -= weaponEquipped.dmg;
                        }
                        gettingAction = false;
                        break;
                    case ConsoleKey.H: //Heal
                        promptMessages.Add("Player heals");
                        player.hp += random.Next(0, player.healAmount);
                        if (player.hp > player.maxHealth) player.hp = player.maxHealth; 
                        gettingAction = false;
                        break;
                    case ConsoleKey.R: //Run
                        promptMessages.Add("Player ran");
                        currentEnemy = null;
                        WriteCombat();
                        gettingAction = false;
                        return;
                    default:
                        promptMessages.Add("Bad input");
                        break;
                }
            }

            if (currentEnemy.hp <= 0) //If enemy dead
            {
                promptMessages.Add("enemy died");
                WritePrompt();
                currentEnemy = null;
                WriteCombat();
                enemiesSlain++;

                player.hp = player.maxHealth;

                //Adds new item to inventory
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] == null)
                    {
                        if (random.Next(0, 2) == 1) //Give an armor piece
                        {
                            switch (random.Next(0, 4))
                            {
                                case (0):
                                    inventory[i] = new Armor(ItemType.Boots, random.Next(enemiesSlain), 1 - 1 / (random.Next(enemiesSlain) + 1));
                                    break;
                                case (1):
                                    inventory[i] = new Armor(ItemType.Pants, random.Next(enemiesSlain), 1 - 1 / (random.Next(enemiesSlain) + 1));
                                    break;
                                case (2):
                                    inventory[i] = new Armor(ItemType.Chest, random.Next(enemiesSlain), 1 - 1 / (random.Next(enemiesSlain) + 1));
                                    break;
                                case (3):
                                    inventory[i] = new Armor(ItemType.Helmet, random.Next(enemiesSlain), 1 - 1 / (random.Next(enemiesSlain) + 1));
                                    break;
                            }
                        }
                        else
                        {
                            switch (random.Next(0, 2))
                            {
                                case (0):
                                    inventory[i] = new Weapon(ItemType.Sword, random.Next(enemiesSlain), 1 - 1 / (random.Next(enemiesSlain) + 1));
                                    break;
                                case (1):
                                    inventory[i] = new Weapon(ItemType.Bow, random.Next(enemiesSlain), 1 - 1 / (random.Next(enemiesSlain) + 1));
                                    break;
                            }
                        }
                        return;
                    }
                }
                return;
            }

            //Enemies turn
            promptMessages.Add(currentEnemy.name + " attacks");
            WritePrompt();
            if (random.Next(10) / 10 < 0.7) // Compare against the currently equiped weapon
            {
                player.hp -= currentEnemy.dmg;
            }
            
            if (player.hp <= 0) //If player dead
            {
                gameRunning = false;
                promptMessages.Add("You died");
                WritePrompt();
                WriteCombat();
                return;
            }

            //Keep combat going
            Combat();
        }

        public static void WriteCombat()
        {
            if (currentEnemy != null || gameRunning == false)
            {
                Write(0, 1, currentEnemy.name, 24);
                Write(0, 2, "Health: " + currentEnemy.hp, 24);
                Write(0, 9, player.name, 24);
                Write(0, 10, "Health: " + player.hp, 24);
            }
            else
            {
                Write(0, 1, " ", 24);
                Write(0, 2, " ", 24);
                Write(0, 9, " ", 24);
                Write(0, 10, " ", 24);
            }
        }

        static void WriteSceneBorders()
        {
            Console.Write
            (
                "--------(Combat)--------|-------(Actions)-------|\n" +
                "                        | Actions: 	Button: |\n" +
                "                        | Attack:     	(A)  	|\n" +
                "                        | Heal:	     	(H)  	|\n" +
                "                        | Run:        	(R)  	|\n" +
                "                        | Next enemy:   (P)  	|\n" +
                "                        |                       |\n" +
                "                        |-----(Item Actions)----|\n" +
                "                        | Equip:       	(E)  	|\n" +
                "                        | Delete:      	(DEL)  	|\n" +
                "                        | Stats:        (S)  	|\n" +
                "------------------------|-----------------------|\n" +
                "\n" +
                "------(Inventory)-------|-------(Prompt)--------|\n" +
                "Press to		|			|\n" +
                "select:	Equiped: Name:  |			|\n" +
                "(1)     ( )             |			|\n" +
                "(2)     ( )             |			|\n" +
                "(3)     ( )             |			|\n" +
                "(4)     ( )             |			|\n" +
                "(5)     ( )             |			|\n" +
                "(6)     ( )             |			|\n" +
                "(7)     ( )             |			|\n" +
                "(8)     ( )             |			|\n" +
                "(9)     ( )             |			|\n" +
                "(0)     ( )             |			|\n" +
                "----(Max 10 items)------|-----------------------|" 
            );
        }

        static void WritePrompt()
        {
            for (int i = 1; i < promptMessages.Count; i++)
            {
                if (i < 13)
                {
                    Write(26, 13 + i, promptMessages[promptMessages.Count - i], 21);
                }
            }
            //Spacer after the message
            promptMessages.Add("");
        }
        
        static void GetInput()
        {
            //Sætter key til at være lige den spilleren trykker
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.P: //Next enemy / proceed
                    promptMessages.Add("Combat started");
                    currentEnemy = new Enemy(random.Next(Convert.ToInt32(player.hp/5) + 1, player.hp), random.Next(1, 3), "Angry orc");
                    WritePrompt();
                    Combat();
                    break;
                case ConsoleKey.D1: //item0 in inventory array
                    if (inventory[0] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (1) selected,");
                    WritePrompt();
                    ItemAction(0);
                    break;
                case ConsoleKey.D2: //item1 in inventory array
                    if (inventory[1] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (2) selected,");
                    WritePrompt();
                    ItemAction(1);
                    break;
                case ConsoleKey.D3: //item2 in inventory array
                    if (inventory[2] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (3) selected,");
                    WritePrompt();
                    ItemAction(2);
                    break;
                case ConsoleKey.D4: //item3 in inventory array 
                    if (inventory[3] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (4) selected,");
                    WritePrompt();
                    ItemAction(3);
                    break;
                case ConsoleKey.D5: //item4 in inventory array 
                    if (inventory[4] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (5) selected,");
                    WritePrompt();
                    ItemAction(4);
                    break;
                case ConsoleKey.D6: //item5 in inventory array 
                    if (inventory[5] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (6) selected,");
                    WritePrompt();
                    ItemAction(5);
                    break;
                case ConsoleKey.D7: //item6 in inventory array
                    if (inventory[6] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (7) selected,");
                    WritePrompt();
                    ItemAction(6);
                    break;
                case ConsoleKey.D8: //item7 in inventory array
                    if (inventory[7] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (8) selected,");
                    WritePrompt();
                    ItemAction(7);
                    break;
                case ConsoleKey.D9: //item8 in inventory array 
                    if (inventory[8] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (9) selected,");
                    WritePrompt();
                    ItemAction(8);
                    break;
                case ConsoleKey.D0: //item9 in inventory array
                    if (inventory[9] == null) break;
                    promptMessages.Add("choose an item action");
                    promptMessages.Add("Item (0) selected,");
                    WritePrompt();
                    ItemAction(9);
                    break;
                default:
                    promptMessages.Add("Bad input");
                    break;
            }
        }

        static void ItemAction(int itemNum)
        {
            bool inputFound = false;
            while (!inputFound)
            {
                //Sætter key til at være lige den spilleren trykker
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.E: //Equip
                        switch (inventory[itemNum].itemType) // Equipping / deequipping items, sørger for at man ikke kan equippe flere af samme type ting såsom 3 våben el. 2 sko
                        {
                            case ItemType.Sword:
                            case ItemType.Bow:
                                if (weaponEquipped == null) //Hvis man ikke har det equipped, equip
                                {
                                    inventory[itemNum].Equipped = true;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") equipped");
                                    weaponEquipped = (Weapon)inventory[itemNum];
                                    break;
                                }

                                if (weaponEquipped == inventory[itemNum]) //Hvis man har det equipped, deequip
                                {
                                    inventory[itemNum].Equipped = false;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") deequipped");
                                    weaponEquipped = null;
                                }
                                else //Hvis man har samme type equipped, skriv det
                                {
                                    promptMessages.Add("equip one weapon");
                                    promptMessages.Add("You can only");
                                }
                                break;
                            case ItemType.Boots:
                                if (bootsEquipped == null) //Hvis man ikke har det equipped, equip
                                {
                                    inventory[itemNum].Equipped = true;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") equipped");
                                    bootsEquipped = inventory[itemNum];
                                    break;
                                }

                                if (bootsEquipped == inventory[itemNum]) //Hvis man har det equipped, deequip
                                {
                                    inventory[itemNum].Equipped = false;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") deequipped");
                                    bootsEquipped = null;
                                }
                                else //Hvis man har samme type equipped, skriv det
                                {
                                    promptMessages.Add("equip 1 set boots");
                                    promptMessages.Add("You can only");
                                }
                                break;
                            case ItemType.Pants:
                                if (pantsEquipped == null) //Hvis man ikke har det equipped, equip
                                {
                                    inventory[itemNum].Equipped = true;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") equipped");
                                    pantsEquipped = inventory[itemNum];
                                    break;
                                }

                                if (pantsEquipped == inventory[itemNum]) //Hvis man har det equipped, deequip
                                {
                                    inventory[itemNum].Equipped = false;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") deequipped");
                                    pantsEquipped = null;
                                }
                                else //Hvis man har samme type equipped, skriv det
                                {
                                    promptMessages.Add("equip one weapon");
                                    promptMessages.Add("You can only");
                                }
                                break;
                            case ItemType.Chest:
                                if (chestEquipped == null) //Hvis man ikke har det equipped, equip
                                {
                                    inventory[itemNum].Equipped = true;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") equipped");
                                    chestEquipped = inventory[itemNum];
                                    break;
                                }

                                if (chestEquipped == inventory[itemNum]) //Hvis man har det equipped, deequip
                                {
                                    inventory[itemNum].Equipped = false;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") deequipped");
                                    chestEquipped = null;
                                }
                                else //Hvis man har samme type equipped, skriv det
                                {
                                    promptMessages.Add("equip one weapon");
                                    promptMessages.Add("You can only");
                                }
                                break;
                            case ItemType.Helmet:
                                if (helmetEquipped == null) //Hvis man ikke har det equipped, equip
                                {
                                    inventory[itemNum].Equipped = true;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") equipped");
                                    helmetEquipped = inventory[itemNum];
                                    break;
                                }

                                if (helmetEquipped == inventory[itemNum]) //Hvis man har det equipped, deequip
                                {
                                    inventory[itemNum].Equipped = false;
                                    promptMessages.Add("Item (" + (itemNum + 1) + ") deequipped");
                                    helmetEquipped = null;
                                }
                                else //Hvis man har samme type equipped, skriv det
                                {
                                    promptMessages.Add("equip one helmet");
                                    promptMessages.Add("You can only");
                                }
                                break;
                            default:
                                promptMessages.Add("Error: Itemtype not found");
                                break;
                        }
                        inputFound = true;
                        break;
                    case ConsoleKey.Delete: //Delete
                        promptMessages.Add("Item (" + (itemNum + 1) + ") deleted"); 
                        inventory[itemNum] = null;
                        WriteInventory();
                        inputFound = true;
                        break;
                    case ConsoleKey.S:
                        for (int i = 0; i < inventory[itemNum].stats.Length; i++)
                        {
                            if (inventory[itemNum].stats[i] != null)
                            {
                                promptMessages.Add(inventory[itemNum].stats[i]);
                            }
                        }
                        promptMessages.Add("Item (" + (itemNum + 1) + ") Stats:");
                        inputFound = true;
                        break;
                }
            }
        }
    }
}
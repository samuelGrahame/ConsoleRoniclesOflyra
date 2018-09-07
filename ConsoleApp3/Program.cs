using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static string Map =
@" !  T                    WW
 WW                        WW              T
 WW T                        WW                            T
  W  S  ";

        static string[] MapRows;

        static int HomeX = 1;
        static int HomeY = 0;

        static int playerPosX = HomeX;
        static int playerPosY = HomeY;
        static int MaxWidth;
        static int previousPostX = HomeX;
        static int previousPostY = HomeY;
        static bool HasABoat = false;
        static bool HasAHammer = false;
        static bool BoatIsDamaged = false;
        static bool HasAxe = false;
        static int Wood = 0;
        static int Branches = 0;
        static int Rocks = 0;
        static int Gold = 0;

        static string Name;
        static string Race;

        static bool CanGetSeeds = false;
        static int Seeds = 0;

        static bool HasSword = false;

        static int Age = 0;

        static int Month = 1;

        static Random EncounterRandom = new Random();

        static List<Tuple<int, int, int>> SmallTrees = new List<Tuple<int, int, int>>();

        static void Save()
        {
            var builder = new StringBuilder();
            builder.AppendLine(MapRows.Length.ToString());
            for (int i = 0; i < MapRows.Length; i++)
            {
                builder.AppendLine(MapRows[i]);
            }

            builder.AppendLine(playerPosX.ToString());
            builder.AppendLine(playerPosY.ToString());

            builder.AppendLine(HasABoat ? "1" : "0");
            builder.AppendLine(HasAHammer ? "1" : "0");
            builder.AppendLine(BoatIsDamaged ? "1" : "0");
            builder.AppendLine(HasAxe ? "1" : "0");
            builder.AppendLine(Wood.ToString());
            builder.AppendLine(Branches.ToString());
            builder.AppendLine(Rocks.ToString());

            builder.AppendLine(Name);
            builder.AppendLine(Race);

            builder.AppendLine(HomeX.ToString());
            builder.AppendLine(HomeY.ToString());


            builder.AppendLine(CanGetSeeds ? "1" : "0");
            builder.AppendLine(Seeds.ToString());
            builder.AppendLine(HasSword ? "1" : "0");
            builder.AppendLine(Gold.ToString());
            builder.AppendLine(Age.ToString());

            builder.AppendLine(SmallTrees.Count.ToString());

            for (int i = 0; i < SmallTrees.Count; i++)
            {
                builder.AppendLine(SmallTrees[i].Item1.ToString());
                builder.AppendLine(SmallTrees[i].Item2.ToString());
                builder.AppendLine(SmallTrees[i].Item3.ToString());
            }

            builder.AppendLine(Month.ToString());

            System.IO.File.WriteAllText("save.game", builder.ToString());
        }

        static bool Load()
        {
            try
            {
                

                string[] data = System.IO.File.ReadAllLines("save.game");

                int length = 0;

                if (int.TryParse(data[0], out length))
                {
                    MapRows = new string[length];

                    for (int i = 0; i < length; i++)
                    {
                        MapRows[i] = data[i + 1];
                    }
                }
                else
                {
                    return false;
                }

                int.TryParse(data[length + 1], out playerPosX);
                int.TryParse(data[length + 2], out playerPosY);

                HasABoat = data[length + 3] == "1";
                HasAHammer = data[length + 4] == "1";
                BoatIsDamaged = data[length + 5] == "1";
                HasAxe = data[length + 6] == "1";

                int.TryParse(data[length + 7], out Wood);
                int.TryParse(data[length + 8], out Branches);
                int.TryParse(data[length + 9], out Rocks);

                Name = data[length + 10];
                Race = data[length + 11];

                int.TryParse(data[length + 12], out HomeX);
                int.TryParse(data[length + 13], out HomeY);


                CanGetSeeds = data[length + 14] == "1";
                int.TryParse(data[length + 15], out Seeds);

                HasSword = data[length + 16] == "1";
                int.TryParse(data[length + 17], out Gold);
                int.TryParse(data[length + 18], out Age);

                int LengthOfSmallTreeTimers = 0;

                int.TryParse(data[length + 19], out LengthOfSmallTreeTimers);

                SmallTrees = new List<Tuple<int, int, int>>();

                int addSmallTree = 20;

                for (int i = 0; i < LengthOfSmallTreeTimers; i++)
                {
                    int a;
                    int b;
                    int c;

                    int.TryParse(data[length + addSmallTree], out a);
                    addSmallTree++;
                    int.TryParse(data[length + addSmallTree], out b);
                    addSmallTree++;
                    int.TryParse(data[length + addSmallTree], out c);
                    addSmallTree++;

                    SmallTrees.Add(new Tuple<int, int, int>(a, b, c));
                }
                int.TryParse(data[length + addSmallTree], out Month);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        static void OnDeath()
        {
            HasABoat = false;
            HasAHammer = false;
            BoatIsDamaged = false;
            HasAxe = false;
            Wood = 0;
            Gold = 0;
            playerPosX = HomeX;
            playerPosY = HomeX;
            Branches = 0;
            Rocks = 0;
            Age += 1;

            CanGetSeeds = false;
            Seeds = 0;

            HasSword = false;
        }
                

        static void TakeStep()
        {
            Month++;
            if(Month == 12)
            {
                Age++;
                Month = 1;
            }

            for (int i = SmallTrees.Count - 1; i >= 0; i--)
            {
                int count = SmallTrees[i].Item3 - 1;
                if(count == 0)
                {
                    SmallTrees.RemoveAt(i);
                    ChangeTile('T', SmallTrees[i].Item1, SmallTrees[i].Item2);
                }
                else
                {
                    SmallTrees[i] = new Tuple<int, int, int>(SmallTrees[i].Item1, SmallTrees[i].Item2, count);
                }                
            }
        }

        public static void RenderMap()
        {
            TakeStep();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            
            for (int i = 0; i < MaxWidth + 4; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();

            Console.ResetColor();
            for (int y = 0; y < MapRows.Length; y++)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;

                Console.Write("||");
                Console.ResetColor();
                

                for (int x = 0; x < MapRows[y].Length; x++)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    if (y == playerPosY && x == playerPosX)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        if(MapRows[y][x] == 'W')
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.ForegroundColor = ConsoleColor.White;
                        }else if (MapRows[y][x] == 'T')
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else if (MapRows[y][x] == 't')
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else if (MapRows[y][x] == 'S')
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }else if (MapRows[y][x] == 'B')
                        {
                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                        else if (MapRows[y][x] == '!')
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    if(MapRows[y][x] == '!')
                    {           
                        Console.Write("H");
                    }
                    else
                    {
                        Console.Write(MapRows[y][x]);
                    }
                    
                    
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("||");
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            for (int i = 0; i < MaxWidth + 4; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
            Console.ResetColor();

        }

        static void Main(string[] args)
        {
            bool loaded = false;
            if(System.IO.File.Exists("save.game"))
            {
                Console.WriteLine("Would you like to continue from your last save?");
                string res = Console.ReadLine().ToLower();
                if (res.StartsWith("yes") || "yes".StartsWith(res))
                {
                    if(!Load())
                    {
                        OnDeath();
                    }
                    else
                    {
                        loaded = true;
                    }
                }
            }
            
            if(!loaded)
            {
                MapRows = Map.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
            
            MaxWidth = 0;
            for (int i = 0; i < MapRows.Length; i++)
            {
                if(MapRows[i].Length > MaxWidth)
                {
                    MaxWidth = MapRows[i].Length;
                }
            }
            for (int i = 0; i < MapRows.Length; i++)
            {
                if (MapRows[i].Length < MaxWidth)
                {
                    int TOAddLength = MaxWidth - MapRows[i].Length;
                    for (int x = 0; x < TOAddLength; x++)
                    {
                        MapRows[i] += ' ';
                    }
                }
            }

             if(!loaded)
            {
                Console.WriteLine("Welcome To ronicles Of lyra");
                Console.WriteLine("What Is Your Name? ");
                Name = Console.ReadLine();
                Console.Clear();
                Console.Write("Choose Your Race? (Orc, Human, Elf) ");
                Race = Console.ReadLine().ToLower();

                while (Race != "orc" && Race != "human" && Race != "elf")
                {
                    Console.Clear();
                    Console.WriteLine("You are what, Not sure I understand...");
                    Console.Write("Choose Your Race? (Orc, Human, Elf) ");
                    Race = Console.ReadLine().ToLower();
                }


                Console.WriteLine($"Oh You Are An {Race}");
                Console.Clear();
                Console.WriteLine("How Old Are You? ");
                Age = 0;
                int.TryParse(Console.ReadLine(), out Age);

                if(Age < 12)
                    Age = 12;

                if (Age > 100)
                    Age = 100;

                Console.WriteLine($"Oh You Are {Age} Years Old");

            }
            
            Console.Clear();

            if(loaded)
            {
                Console.WriteLine($"Welcome back {Name} the {Race}");
            }

            string preAnswer = "";

            while(true)
            {
                preAnswer = "";
                RenderMap();

                char tile = MapRows[playerPosY][playerPosX];
                if(tile == ' ')
                {
                    if(EncounterRandom.Next(1, 10) == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("You see a stranger on the path...");
                        Console.ResetColor();
                        string res = Console.ReadLine().ToLower();
                        if (res.Contains("hello") || res.Contains("hey") || res.Contains("hi") || res.Contains("g'day") || res.Contains("good morning") || res.Contains("good afternoon") || res.Contains("hay"))
                        {
                            Console.WriteLine($"{res}... what do you want?");
                            while (true)
                            {
                                res = Console.ReadLine().ToLower();

                                if (res == "fuck you")
                                {
                                    Console.WriteLine($"You what cunt!");
                                    Console.WriteLine($"stranger walks away, turning his head making sure you don't hurt him....");
                                    break;
                                }
                                else if (res.Contains("help"))
                                {
                                    Console.WriteLine($"do you happen to have wood?, I would love to have a home of my own...");
                                    res = Console.ReadLine().ToLower();
                                    if(res.Contains("yes") || "yes".Contains(res))
                                    {
                                        Console.WriteLine($"I have {(Wood + (Wood != 1 ? " logs" : " log"))}...");
                                        if(Wood == 0)
                                        {                                            
                                            Console.WriteLine($"Thank's for nothing...");
                                            break;
                                        }
                                        else if (Wood < 30)
                                        {
                                            if(EncounterRandom.Next(1, 10) == 1)
                                            {
                                                Console.WriteLine($"Thank's for the wood, Cya haha");
                                                Wood = 0;
                                                Console.WriteLine($"That stranger just stole all your wood...");

                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine($"Sorry, You don't have enough for a house...");
                                            }
                                            
                                        }
                                        else if (Wood >= 30)
                                        {
                                            Console.WriteLine($"Thank you so much, I will be able to finaly ask that miller's daughter out...");
                                            Console.WriteLine($"Here have these coin's I found on the road, They have no used for me..");
                                            Wood -= 30;
                                            if (Age < 13)
                                            {
                                                Gold += 1;
                                            }
                                            else
                                            {
                                                Gold += EncounterRandom.Next(10, 25) + (Race == "human" ? 15 : 0);
                                            }
                                        }
                                    }
                                }
                                else if (res.Contains("seeds"))
                                {
                                    Console.WriteLine("Would you like to know about seeds (yes / no)?");
                                    res = Console.ReadLine().ToLower();
                                    if (res.StartsWith("yes") || "yes".StartsWith(res))
                                    {
                                        if (EncounterRandom.Next(1, 10) == 1)
                                        {
                                            Console.WriteLine("What you need to do is, Look at the bottom of the tree...");
                                            CanGetSeeds = true;
                                        }else if (EncounterRandom.Next(1, 4) == 1)
                                        {
                                            Console.WriteLine("Maybe it has something to do with a tree....");
                                            Console.WriteLine("I have some where to be, good bye...");
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("I have no fucking idea...");
                                            Console.WriteLine("Do you think I am a encyclopedia!");
                                            break;
                                        }
                                    }
                                }
                                else if (res == "attack")
                                {
                                    if (HasSword)
                                    {
                                        Console.WriteLine("You killed him in one blow...");
                                        Thread.Sleep(100);
                                        Console.WriteLine("He had nothing....");
                                        Thread.Sleep(100);
                                        Console.WriteLine("There is blood everywhere...");
                                        Thread.Sleep(100);
                                        Console.WriteLine("Maybe I should clean all the blood off me...");
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Are you trying to hit me, you are a FUCKING WANKER!...");
                                        break;
                                    }
                                }
                                else if (res == "bye" || res == "cya")
                                {
                                    Console.WriteLine("Okay, have a good one...");
                                    break;
                                }
                                else if (res == "run away")
                                {
                                    Console.WriteLine("Everything okay in your head...");
                                    break;
                                }
                                Console.WriteLine("Anything else??");
                            }

                        }
                        else
                        {
                            preAnswer = res;
                        }
                    }else if (EncounterRandom.Next(1, 20) == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("You see a strange object, would you like to inspect?");
                        Console.ResetColor();
                        string res = Console.ReadLine().ToLower();
                        if(res.StartsWith("yes") || "yes".StartsWith(res))
                        {
                            if(EncounterRandom.Next(1, 5) == 1)
                            {
                                Console.WriteLine("hmmm... it was nothing");
                            }
                            else if (EncounterRandom.Next(1, 10) == 1)
                            {
                                if(!HasAHammer)
                                {
                                    Console.WriteLine("You found a hammer...");
                                    HasAHammer = true;
                                }else
                                {
                                    Console.WriteLine("hmmm... its just another hammer...");
                                }
                            }else if (EncounterRandom.Next(1, 20) == 1)
                            {
                                if(!HasABoat)
                                {
                                    Console.WriteLine("You found a old boat... looks like it needs reparing...");

                                    if(!HasAHammer)
                                    {
                                        Console.WriteLine("Just if I had something to repair it with...");
                                        ChangeTile('B');
                                    }
                                    else
                                    {
                                        Console.WriteLine("Would you like to repair this old boat?");

                                        res = Console.ReadLine().ToLower();
                                        if(res.StartsWith("yes") || "yes".StartsWith(res))
                                        {
                                            if (Race == "orc")
                                            {
                                                if(EncounterRandom.Next(1, 10) == 1)
                                                {
                                                    BoatIsDamaged = true;
                                                }
                                            }
                                            if(BoatIsDamaged)
                                            {
                                                Console.WriteLine("... that should do it, hmmm maybe I should give it go.");
                                            }
                                            else
                                            {
                                                Console.WriteLine("... you have fixed that old boat, hmmm maybe I should give it go.");
                                            }
                                            
                                            HasABoat = true;
                                        }
                                        else
                                        {
                                            ChangeTile('B');
                                        }                                     
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Another boat, I have no need for that...");
                                }

                            }
                            else
                            {
                                if (EncounterRandom.Next(1, 3) == 1)
                                {
                                    Console.WriteLine("hmmm... it was just a bush, Would you like to gather branches?");
                                    res = Console.ReadLine().ToLower();
                                    if (res.StartsWith("yes") || "yes".StartsWith(res))
                                    {
                                        Branches += EncounterRandom.Next(1, 5);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("hmmm... it was just a pile of rocks, Would you like to gather some rocks?");
                                    res = Console.ReadLine().ToLower();
                                    if (res.StartsWith("yes") || "yes".StartsWith(res))
                                    {
                                        Rocks += EncounterRandom.Next(1, 3);

                                    }
                                    
                                }
                            }                            
                        }
                        else
                        {
                            preAnswer = res;
                        }
                    }
                    else
                    {
                        Console.WriteLine("you see nothing in sight...");

                        if (HasAHammer && Wood >= 30 || Seeds > 0)
                        {

                            Console.WriteLine("What would you like to do here? " + (HasAHammer && Wood >= 30 ? "build house, " : "") + (Seeds > 0 ? "plant a tree" : ""));
                            var res = Console.ReadLine().ToLower();
                            if (HasAHammer && Wood >= 30 && (res.StartsWith("build") || "build".StartsWith(res) || res.StartsWith("house") || "house".StartsWith(res)))
                            {
                                Wood -= 30;

                                ChangeTile('!');

                                HomeX = playerPosX;
                                HomeY = playerPosY;

                                Console.Clear();

                                continue;
                            }else if (Seeds > 0 && (res.StartsWith("plant") || "plant".StartsWith(res) || res.StartsWith("tree") || "tree".StartsWith(res)))
                            {
                                Seeds--;

                                ChangeTile('t');

                                SmallTrees.Add(new Tuple<int, int, int>(playerPosX, playerPosY, 100));

                                Console.Clear();

                                continue;
                            }
                            else
                            {
                                preAnswer = res;
                            }
                        }
                    }                    
                }
                else if(tile == 'B')
                {
                    if (!HasABoat)
                    {
                        Console.WriteLine("You found a old boat... looks like it needs reparing...");

                        if (!HasAHammer)
                        {
                            Console.WriteLine("Just if I had something to repair it with...");                            
                        }
                        else
                        {
                            Console.WriteLine("Would you like to repair this old boat?");

                            var res = Console.ReadLine().ToLower();
                            if (res.StartsWith("yes") || "yes".StartsWith(res))
                            {
                                if (Race == "orc")
                                {
                                    if (EncounterRandom.Next(1, 10) == 1)
                                    {
                                        BoatIsDamaged = true;
                                    }
                                }
                                if (BoatIsDamaged)
                                {
                                    Console.WriteLine("... that should do it, hmmm maybe I should give it go.");
                                }
                                else
                                {
                                    Console.WriteLine("... you have fixed that old boat, hmmm maybe I should give it go.");
                                }

                                HasABoat = true;
                                ChangeTile(' ');
                            }                            
                        }
                    }
                    else
                    {
                        Console.WriteLine("Another boat, I have no need for that...");
                    }
                }
                else if (tile == '!')
                {
                    Console.WriteLine("You are home...");

                    try
                    {
                        Save();
                        Console.WriteLine("You feel safe...");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("You don't feel safe....");
                    }

                    if(Rocks >= 2 && Branches >= 3 && !HasAxe)
                    {
                        Console.WriteLine("You have the resources to make a axe, Would you like to make one?");

                        string res = Console.ReadLine().ToLower();
                        if (res.StartsWith("yes") || "yes".StartsWith(res))
                        {
                            Rocks -= 2;
                            Branches -= 3;
                            HasAxe = true;
                        }
                        else
                        {
                            preAnswer = res;
                        }
                    }
                }
                else if (tile == 'T')
                {
                    Console.WriteLine("You see a large oak tree...");
                    if(HasAxe || CanGetSeeds)
                    {                                       
                        Console.WriteLine($"What would you like to do? ({((HasAxe ? "chop down tree, " : "") + (CanGetSeeds ? "gather seeds" : "") )})");

                        string res = Console.ReadLine().ToLower();
                        if (HasAxe && (res.StartsWith("chop") || "chop".StartsWith(res)))
                        {

                            ChangeTile(' ');                            

                            Wood += EncounterRandom.Next(1, 6) + (Race == "orc" ? 2 : 0);

                            Console.Clear();

                            continue;
                        }else if (CanGetSeeds && (res.StartsWith("seed") || "seed".StartsWith(res)))
                        {
                            Seeds += EncounterRandom.Next(1, 2) + (Race == "elf" ? 2 : 0);

                            Console.Clear();

                            continue;
                        }
                        else
                        {
                            
                            preAnswer = res;
                        }
                    }
                }                
                else if (tile == 'S')
                {
                    // Gold
                    // Ques would you like to talk to the shop owner?
                    // list of stuff to sell.
                    // each item has a price
                    // if you already have the item. dont let them buy. unless orc - orc loses money and doesnt get extra item

                    if (EncounterRandom.Next(5, 10) == 5)
                    {
                        Console.WriteLine("You see a large shop, but it seems to be closed now...");
                    }

                    else
                    {
                        Console.WriteLine("Would you like to talk to the shop owner?");

                        string res = Console.ReadLine().ToLower();

                        if (res.StartsWith("yes") || "yes".StartsWith(res))
                        {
                            if (Gold >= 100)
                            {
                                Console.WriteLine("Would you like to buy my Special sword?");
                                res = Console.ReadLine().ToLower();

                                if (res.StartsWith("yes") || "yes".StartsWith(res))
                                {
                                    Console.WriteLine("Here you go...");
                                    HasSword = true;
                                    Gold -= 100;
                                    Console.WriteLine("It's nice and shinny. Would you like to use it on the shop keeper?");
                                    res = Console.ReadLine().ToLower();

                                    if (res.StartsWith("yes") || "yes".StartsWith(res))
                                    {
                                        Gold += 100;

                                        ChangeTile(' ');

                                        Console.WriteLine("Hmmm maybe that was not the best idea. Hope I can find another shop...");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("You dont have enough money PEASANT! If you suck my dick ill give my special sword to you for free");
                            }
                        }
                        else
                        {

                            preAnswer = res;
                        }
                    }
                    
                }

                Console.WriteLine("Which Direction do you want to go? (north, east, south & west)");
                
                string result = string.IsNullOrWhiteSpace(preAnswer) || preAnswer == "no" ? Console.ReadLine().ToLower() : preAnswer;
                Console.Clear();

                if (result.StartsWith("north") || "north".StartsWith(result))
                {
                    if(playerPosY == 0)
                    {
                        Console.WriteLine("You are blocked by a mystical barrier!");
                    }
                    else
                    {
                        playerPosY -= 1;
                    }
                }
                else if (result.StartsWith("east") || "east".StartsWith(result))
                {
                    if(playerPosX == MapRows[playerPosY].Length - 1)
                    {
                        Console.WriteLine("You are blocked by a mystical barrier!");
                    }
                    else
                    {
                        playerPosX += 1;
                    }                   
                }
                else if (result.StartsWith("west") || "west".StartsWith(result))
                {
                    if (playerPosX == 0)
                    {
                        Console.WriteLine("You are blocked by a mystical barrier!");
                    }
                    else
                    {
                        playerPosX -= 1;
                    }
                }
                else if (result.StartsWith("south") || "south".StartsWith(result))
                {
                    if (playerPosY == MapRows.Length - 1)
                    {
                        Console.WriteLine("You are blocked by a mystical barrier!");
                    }
                    else
                    {
                        playerPosY += 1;
                    }
                }

                tile = MapRows[playerPosY][playerPosX];
                if (tile == 'W')
                {
                    if(!HasABoat)
                    {
                        // TODO - if we have a boat, then let them...
                        if(Race == "orc")
                        {
                            Console.WriteLine("Oh dear, you have drowned...");
                            Console.WriteLine("You wake up at home...");

                            OnDeath();
                        }
                        else
                        {
                            Console.WriteLine("Sorry you do not see that you have a boat...");

                            playerPosX = previousPostX;
                            playerPosY = previousPostY;
                        }                        
                    }
                    else
                    {
                        if(BoatIsDamaged)
                        {
                            if(EncounterRandom.Next(1, 5) == 1)
                            {
                                HasAHammer = false;
                                HasABoat = false;
                                BoatIsDamaged = false;

                                Console.WriteLine("Oh dear THERE IS A HOLE IN YOUR BOAT. you have drowned...");
                                Console.WriteLine("You wake up at home...");

                                OnDeath();
                            }
                            else
                            {
                                Console.WriteLine("You notice a hole in your boat...");
                            }
                        }
                        else
                        {
                            Console.WriteLine("You set sail...");
                        }                        
                    }
                }                

                previousPostX = playerPosX;
                previousPostY = playerPosY;
            }


            




        }

        static void ChangeTile(char newTile)
        {
            ChangeTile(newTile, playerPosX, playerPosY);            
        }

        static void ChangeTile(char newTile, int x, int y)
        {
            var builder = new StringBuilder();
            for (int k = 0; k < MapRows[y].Length; k++)
            {
                if (k == x)
                {
                    builder.Append(newTile);
                }
                else
                {
                    builder.Append(MapRows[y][k]);
                }

            }
            MapRows[y] = builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static string Map =
@" !  T  
 W     
 W T                                                      s
    S  ";

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

        static string Name;
        static string Race;

        static Random EncounterRandom = new Random();

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

            playerPosX = HomeX;
            playerPosY = HomeX;
            Branches = 0;
            Rocks = 0;
        }

        public static void RenderMap()
        {
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
                    if(y == playerPosY && x == playerPosX)
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
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else if (MapRows[y][x] == 'S')
                        {
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                    }
                    Console.Write(MapRows[y][x]);
                    
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
                int age = 0;
                int.TryParse(Console.ReadLine(), out age);
                Console.WriteLine($"Oh You Are {age} Years Old");
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
                        Console.WriteLine("You see a passing trader...");
                    }else if (EncounterRandom.Next(1, 20) == 1)
                    {
                        Console.WriteLine("You see a strange object, would you like to inspect?");
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

                        if (HasAHammer && Wood >= 30)
                        {
                            Console.WriteLine("You have the resources to build a house, Would you like to?");
                            var res = Console.ReadLine().ToLower();
                            if (res.StartsWith("yes") || "yes".StartsWith(res))
                            {
                                Wood -= 30;

                                ChangeTile('!');

                                HomeX = playerPosX;
                                HomeY = playerPosY;

                                continue;
                            }
                            else
                            {
                                preAnswer = res;
                            }
                        }
                    }                    
                }else if (tile == '!')
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
                    if(HasAxe)
                    {
                        Console.WriteLine("Would you like to chop down this tree?");

                        string res = Console.ReadLine().ToLower();
                        if (res.StartsWith("yes") || "yes".StartsWith(res))
                        {
                            ChangeTile(' ');                            

                            Wood += EncounterRandom.Next(1, 6) + (Race == "orc" ? 2 : 0);

                            continue;
                        }
                    }
                }                
                else if (tile == 'S')
                {
                    Console.WriteLine("You see a large shop, but it seems to be closed now...");
                }else if (tile == 'S')
                {
                    Console.WriteLine("You see a large shop, but it seems to be closed now...");
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
            var builder = new StringBuilder();
            for (int k = 0; k < MapRows[playerPosY].Length; k++)
            {
                if (k == playerPosX)
                {
                    builder.Append(newTile);
                }
                else
                {
                    builder.Append(MapRows[playerPosY][k]);
                }

            }
            MapRows[playerPosY] = builder.ToString();
        }
    }
}

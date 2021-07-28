using System;
using System.IO;
using DemoInfo;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Moneyshoot
{
	public static class Parser
	{
		public static void Main()
		{
            string folderPath = "demos/";
            IEnumerable<string> filenames = Directory.EnumerateFiles(folderPath, "*.dem");
            ParseDemo(filenames);
            Console.Read();
        }

        public static void ParseDemo(IEnumerable<string> args)
        {
            foreach (string fileName in args)
            {
                using (FileStream fileStream = File.OpenRead(fileName))
                {
                    Console.WriteLine("Parsing demo " + fileName);
                    using (DemoParser parser = new DemoParser(fileStream))
                    {
                        List<BasePlayer> playerArr = new List<BasePlayer>();
                        parser.ParseHeader();
                        int round = 0;
                        string map = parser.Map;
                        Console.WriteLine(map);

                        //parser.HeaderParsed += (sender, e) =>
                        //{

                        //};

                        //parser.TickDone += (sender, e) =>
                        //{

                        //};

                        // Reset every variable when RoundAnnounceMatchStarted is called
                        parser.RoundAnnounceMatchStarted += (sender, e) =>
                        {
                            round = 0;
                            playerArr = new List<BasePlayer>();
                            foreach (Player p in parser.PlayingParticipants)
                            {
                                BasePlayer player = new BasePlayer();
                                player.SteamID = p.SteamID;
                                player.Name = p.Name;
                                playerArr.Add(player);
                            }
                        };

                        parser.PlayerKilled += (sender, e) =>
                        {
                            Player killer = e.Killer;
                            Player victim = e.Victim;
                            Player assister = e.Assister;
                            int killerIndex = playerArr.FindIndex(f => f.SteamID == killer.SteamID);
                            int victimIndex = playerArr.FindIndex(f => f.SteamID == victim.SteamID);

                            if (killerIndex != -1)
                            {
                                playerArr[killerIndex].Kills++;
                                if (e.Headshot)
                                {
                                    playerArr[killerIndex].Headshots++;
                                }
                            }

                            if (victimIndex != -1)
                            {
                                playerArr[victimIndex].Deaths++;
                            }

                            if (assister != null)
                            {
                                int assisterIndex = playerArr.FindIndex(f => f.SteamID == assister.SteamID);
                                if (assisterIndex != -1)
                                {
                                    playerArr[assisterIndex].Assists++;
                                }
                            }
                        };

                        parser.PlayerHurt += (sender, e) =>
                        {
                            Player attacker = e.Attacker;
                            int HealthDamage = e.Player.HP < e.HealthDamage ? e.Player.HP : e.HealthDamage;             
                            if (attacker != null)
                            {
                                int index = playerArr.FindIndex(f => f.SteamID == attacker.SteamID);
                                if (index != -1)
                                {
                                    playerArr[index].Damage += HealthDamage;
                                
                                }
                            }
                        };

                        parser.BombPlanted += (sender, e) =>
                        {
                            Player planter = e.Player;
                            int index = playerArr.FindIndex(f => f.SteamID == planter.SteamID);
                            if (index != -1)
                            {
                                playerArr[index].BombPlants++;
                            }
                        };

                        parser.RoundMVP += (sender, e) =>
                        {
                            Player mvp = e.Player;
                            int index = playerArr.FindIndex(f => f.SteamID == mvp.SteamID);
                            if (index != -1)
                            {
                                playerArr[index].MVPs++;
                            }
                        };

                        parser.RoundEnd += (sender, e) =>
                        {
                            round++;
                            Console.WriteLine("Round " + round + " Ended. " + e.Winner + " won!");
                        };

                        parser.ParseToEnd();

                        foreach (BasePlayer p in playerArr)
                        {
                            Console.WriteLine();
                            Console.WriteLine(p.Name);
                            Console.WriteLine("Kills: " + p.Kills);
                            Console.WriteLine("Assists: " + p.Assists);
                            Console.WriteLine("ADR: " + p.Damage / round);
                            Console.WriteLine("Headshots: " + p.Headshots);
                            Console.WriteLine("Deaths: " + p.Deaths);
                        }

                        StoreData(ref playerArr, map);

                    }

                }
            }
        }

        private static void StoreData(ref List<BasePlayer> playerList, string map)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("playerDB");
            var collection = database.GetCollection<BsonDocument>("players");
            foreach (BasePlayer p in playerList)
            {
                BsonDocument player = new BsonDocument
                {
                    { "name", p.Name },
                    { "steamid", p.SteamID },
                    { "map", map },
                    { "kills", p.Kills },
                    { "headshots", p.Headshots },
                    { "assists", p.Assists },
                    { "damage", p.Damage },
                    { "deaths", p.Deaths },
                    { "plants", p.BombPlants },
                    { "mvps", p.MVPs }
                };
                collection.InsertOne(player);
            }
        }
    }
}
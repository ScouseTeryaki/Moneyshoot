using System;
using System.IO;
using DemoInfo;
using System.Collections.Generic;
using System.Linq;

namespace Moneyshoot
{
	public static class Analyser
	{
		public static void Main()
		{
			string[] filenames = { "game.dem" };
            ParseDemo(filenames);
            Console.Read();
        }

        public static void ParseDemo(string[] args)
        {
            foreach (var fileName in args)
            {
                using (var fileStream = File.OpenRead(fileName))
                {
                    Console.WriteLine("Parsing demo " + fileName);
                    using (var parser = new DemoParser(fileStream))
                    {
                        List<BasePlayer> playerArr = new List<BasePlayer>();
                        parser.ParseHeader();
                        int round = 0;
                        string map = parser.Map;
                        Console.WriteLine(map);

                        parser.HeaderParsed += (sender, e) =>
                        {

                        };

                        parser.TickDone += (sender, e) =>
                        {

                        };

                        // Reset every variable when RoundAnnounceMatchStarted is called
                        parser.RoundAnnounceMatchStarted += (sender, e) =>
                        {
                            round = 0;
                            playerArr = new List<BasePlayer>();
                            foreach (var p in parser.PlayingParticipants)
                            {
                                BasePlayer player = new BasePlayer();
                                player.SteamID = p.SteamID;
                                player.Name = p.Name;
                                playerArr.Add(player);
                            }
                        };

                        parser.PlayerKilled += (sender, e) =>
                        {
                            var killer = e.Killer;
                            int index = playerArr.FindIndex(f => f.SteamID == killer.SteamID);
                            if(index != -1)
                            {
                                playerArr[index].Kills++;
                            }
                        };

                        parser.PlayerHurt += (sender, e) =>
                        {
                            var attacker = e.Attacker;
                            var HealthDamage = e.Player.HP < e.HealthDamage ? e.Player.HP : e.HealthDamage;             
                            if (attacker != null)
                            {
                                int index = playerArr.FindIndex(f => f.SteamID == attacker.SteamID);
                                if (index != -1)
                                {
                                    // Need to get players health before damaged
                                    playerArr[index].Damage += HealthDamage;
                                
                                }
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
                            Console.WriteLine(p.Name);
                            Console.WriteLine("Kills: " + p.Kills);
                            Console.WriteLine("ADR: " + p.Damage / round);
                        }

                    }

                }
            }
        }
    }
}
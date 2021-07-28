using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneyshoot
{
    class BasePlayer
    {
        public long SteamID { get; set; } // Done
        public string Name { get; set; } // Done
        public float Damage { get; set; } // Done
        public int Kills { get; set; } // Done
        public int EntryKills { get; set; }
        public int Headshots { get; set; } // Done
        public int Deaths { get; set; } // Done
        public int Assists { get; set; } // Done
        public int BombPlants { get; set; } // Done
        public int MVPs { get; set; } // Done

        public BasePlayer()
        {

        }

        public BasePlayer Copy()
        {
            BasePlayer me = new BasePlayer();
            me.SteamID = SteamID;
            me.Name = Name;
            me.Kills = Kills;
            me.EntryKills = EntryKills;
            me.Headshots = Headshots;
            me.Deaths = Deaths;
            me.Assists = Assists;
            me.BombPlants = BombPlants;
            me.MVPs = MVPs;
            me.Damage = Damage;
            return me;
        }
    }
}

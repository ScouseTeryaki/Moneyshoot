using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneyshoot
{
    class BasePlayer
    {
        public long SteamID { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
        public int Kills { get; set; }
        public int EntryKills { get; set; }
        public int Headshots { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int BombPlants { get; set; }
        public int BombDefuses { get; set; }
        public int MVPs { get; set; }

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
            me.BombDefuses = BombDefuses;
            me.BombDefuses = BombDefuses;
            me.MVPs = MVPs;
            me.Damage = Damage;
            return me;
        }
    }
}

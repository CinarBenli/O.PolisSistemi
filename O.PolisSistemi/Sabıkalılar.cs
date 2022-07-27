using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace O.PolisSistemi
{
  public class Sabıkalılar
    {
        public String Oyuncuİsmi;
        public Steamworks.CSteamID SteamİD;
        public int KimlikNo;
        public List<Sabıka> Sabıkal = new List<Sabıka>();
        public List<KayıtlıAraçlar> Araçlar = new List<KayıtlıAraçlar>();
    }
}

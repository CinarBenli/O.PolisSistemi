using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace O.PolisSistemi
{
    public class Configuration : IRocketPluginConfiguration
    {
        public Vector3 HapishaneKonum;
        public Vector3 HapishaneÇıkışKonum;
        public List<Sabıkalılar> Sabıkalılar = new List<Sabıkalılar>();
        public List<CezaYiyenler> Cezalılar = new List<CezaYiyenler>();
        public void LoadDefaults()
        {

        }
    }
}

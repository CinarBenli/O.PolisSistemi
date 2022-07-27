using Rocket.API;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace O.PolisSistemi
{
    public class Class1 : RocketPlugin<Configuration>
    {
         List<Polisler> polis = new List<Polisler>();
        List<KayıtlıAraçlar> KayıtlıAraç = new List<KayıtlıAraçlar>();
        List<Aranan> Arananlar = new List<Aranan>();
        List<Hapishane> Hapis = new List<Hapishane>();
        
        protected override void Load()
        {
            EffectManager.onEffectButtonClicked += button;
            EffectManager.onEffectTextCommitted += yazı;
            U.Events.OnPlayerConnected += girdiğinde;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += hareket;
           StartCoroutine(Zaman());
            StartCoroutine(Zaman2());

            
            
        }

        private void hareket(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            bool kontrol = IRocketPlayerExtension.HasPermission(player, "O.Polis");
            if (kontrol)
            {
                if(gesture == UnturnedPlayerEvents.PlayerGesture.Arrest_Start)
                {
                    if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, 4, RayMasks.PLAYER_INTERACT) && hit.transform.TryGetComponent(out UnturnedPlayer oyuncu))
                    {
                        while (true)
                        {
                            if(gesture == UnturnedPlayerEvents.PlayerGesture.Arrest_Stop)
                            {
                                break;
                            }
                            oyuncu.Teleport(player);
                        }

                    }
                }

                if(gesture == UnturnedPlayerEvents.PlayerGesture.Point)
                {
                    if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit hit, 4, RayMasks.PLAYER_INTERACT) && hit.transform.TryGetComponent(out UnturnedPlayer oyuncu))
                    {
                        if(oyuncu.Player.animator.captorID == Steamworks.CSteamID.Nil)
                        {
                            oyuncu.Player.animator.captorID = player.CSteamID;
                            oyuncu.Player.animator.captorItem = 1195;
                            oyuncu.Player.animator.captorStrength = ushort.MaxValue;
                            oyuncu.Player.animator.sendGesture(EPlayerGesture.ARREST_START, true);
                            UnturnedChat.Say(player, oyuncu.CharacterName+" Adlı Oyuncuyu Kelepçeledin",Color.green);
                            UnturnedChat.Say(oyuncu, player.CharacterName + " Adlı Polis Seni Kelepçeledi");
                            return;
                        }
                        oyuncu.Player.animator.captorID = Steamworks.CSteamID.Nil;
                        oyuncu.Player.animator.captorItem = 0;
                        oyuncu.Player.animator.captorStrength = 0;
                        oyuncu.Player.animator.sendGesture(EPlayerGesture.ARREST_STOP, true);
                        UnturnedChat.Say(player, oyuncu.CharacterName + " Adlı Oyuncunun Kelepçesini Açtın", Color.green);
                        UnturnedChat.Say(oyuncu, player.CharacterName + " Adlı Polis Senin Kelepçeni Açtın");
                    }
                    }
            }
        }

        private void yazı(Player player, string buttonName, string text)
        {
            UnturnedPlayer pl = UnturnedPlayer.FromPlayer(player);

            if (buttonName == "kimlikin")
            {
                var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                var kontrol = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                if(kontrol == null)
                {
                    text = "";
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "uyarı", "Böyle Bir Kimlik Bulunamadı");
                    kontrol2.sayı1 = 0;
                    return;
                }
                else 
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "uyarı", "");

                   
                    kontrol2.yazı = kontrol.SteamİD;
                    kontrol2.sayı1 = 1;
                    kontrol2.sayı2 = 1;
                }
            }
            if(buttonName == "skimlikno")
            {
                var değer = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                if (değer == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "soyuncuisim", "Böyle Bir Kimlik Bulunmamakta");
                    kontrol.yazı = pl.CSteamID;
                        return;
                
                }

                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "soyuncuisim", "");
                kontrol.yazı = değer.SteamİD;
                

            }

            if(buttonName == "sabıkaneden") 
            {
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);

                if (text.Length > 100)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "suyarı", "En Fazla 100 Harf Yazılabilir");
                    kontrol.yazı2 = null;
                    return;
                }
                kontrol.yazı2 = text;

            }
            /*
            *
            *Ceza Kes
            */
            if(buttonName == "ckimlikno")
            {
                var değer = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                if (değer == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "cuyarı", "Böyle Bir Kimlik Bulunmamakta");
                    kontrol.yazı = pl.CSteamID;
                    return;

                }

                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "cuyarı", "");
                kontrol.yazı = değer.SteamİD;
            }
            if(buttonName == "cezanedeni")
            {
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol.yazı2 = text;

            }
            if (buttonName == "ctutar")
            {
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol.sayı1 = int.Parse(text);

            }
            if (buttonName == "czaman")
            {

                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol.sayı2 = int.Parse(text);

            }
            if(buttonName == "araçplaka")
            {
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                var kontrol2 = KayıtlıAraç.FirstOrDefault(e => e.AraçPlakası == text);
                if(kontrol2 == null)
                {
                  EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arabasahibiisim", "Böyle Bir Araç Bulunmamakta");
                    text = "";
                    return;

                }
                else
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arabasahibiisim", "");

                    kontrol.yazı = kontrol2.AraçSahibiSteamİD;
                    kontrol.yazı2 = text;
                    kontrol.sayı1 = 1;
                    return;
                }

            }
            if(buttonName == "araçplakaiki")
            {
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol.yazı2 = text;



            }
            if(buttonName == "araçkimlik")
            {
                var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                if(kontrol2 == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "aauyarı", "Geçersiz Kimlik Numarası");
                    text = "";
                    return;
                }
                else
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "aauyarı", "");
                    kontrol.yazı = kontrol2.SteamİD;
                    kontrol.sayı1 = 1;

                }
                }
            if(buttonName == "aranankimlik")
            {
                var kontrol = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                if(kontrol == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı", "Böyle Bir Kimlik Bulunamadı");
                    text = "";
                    return;
                }




                var kontrol1 = Arananlar.FirstOrDefault(e => e.KimlikNo == text);
                if(kontrol1  != null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı", "Bu Kişi Zaten Aranıyor");
                    text = "";
                    return;
                }
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı", "");
                var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol2.yazı = kontrol.SteamİD;
                    
                


            }
            if(buttonName== "aranmaneden")
            {
                var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol2.yazı2 = text;
            }
            if (buttonName == "aranankimlik2")
            {
                var kontrol = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                if (kontrol == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı (1)", "Böyle Bir Kimlik Bulunamadı");
                    text = "";
                    return;
                }
                var kontrol1 = Arananlar.FirstOrDefault(e => e.KimlikNo == text);
                if(kontrol1 == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı (1)", "Vatandaş Arananlar Listesinde Yok");
                    return;
                }
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı (1)", "");
                var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol2.yazı = kontrol.SteamİD;
                kontrol2.yazı2 = kontrol.KimlikNo.ToString();

            }
            if(buttonName== "hapiskimlik")
            {
                var kontrol = Arananlar.FirstOrDefault(e => e.KimlikNo == text);
                if(kontrol == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı", "Böyle Bir Oyuncu Hapiste Yok");
                    text = "";
                    return;

                }
                else
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı", "");

                    var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                    kontrol2.yazı = kontrol.SteamİD;
                }
            }
            if (buttonName == "hapiskimlik2")
            {
                var kontrol = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo.ToString() == text);
                if(kontrol == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı2", "Böyle Bir Kimlik Bulunmamakta");
                    text = "";
                    return;
                }
                else
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı2","");
                    var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                    kontrol2.yazı = kontrol.SteamİD;
                }



            }
            if (buttonName == "hapissüre")
            {
                var kontrol2 = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
                kontrol2.sayı1 = int.Parse(text);
            }
        }
        private IEnumerator<WaitForSeconds> Zaman2()
        {
            while (true)
            {
                foreach(var değer in Hapis)
                {
                    değer.Dakika -= 5;
                    if(değer.Dakika <= 0)
                    {
                        UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(değer.SteamİD);

                        if(pl == null)
                        {
                            değer.Dakika = 0;
                        }
                        else
                        {
                            Hapis.Remove(değer);
                            pl.Player.teleportToLocation(Configuration.Instance.HapishaneÇıkışKonum,1);
                        }
                        


                    }
                }
                yield return new WaitForSeconds(300);

            }
            
            }

            private IEnumerator<WaitForSeconds> Zaman()
        {
            while (true)
            {
                yield return new WaitForSeconds(3600);


                foreach (var değer in Configuration.Instance.Cezalılar)
                {
                    değer.CezaSüresi -= 1;
                    if(değer.CezaSüresi == 0)
                    {
                        UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(değer.SteamİD);
                        if(pl == null)
                        {
                            değer.cezaöde = true;

                        }
                        else
                        {
                            pl.Experience = pl.Experience =(uint) - değer.CezaÜcreti;
                            Configuration.Instance.Cezalılar.Remove(değer);
                            UnturnedChat.Say(pl, "Ceza Süreniz Dolduğu İçin Otomatik Ödendi",Color.yellow);
                        }
                    }
                }
            }
        }

        private void girdiğinde(UnturnedPlayer player)
        {
            var kontrol = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == player.CSteamID);
            if (kontrol == null)
            {
                int kimlikno = 0;
                System.Random rn = new System.Random();
                while (true)
                {
                    kimlikno = rn.Next(100000, 999999);
                    var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.KimlikNo == kimlikno);
                    if (kontrol2 == null)
                    {
                        break;
                    }

                }

                Configuration.Instance.Sabıkalılar.Add(new Sabıkalılar
                {
                    Oyuncuİsmi = player.CharacterName,
                    SteamİD = player.CSteamID,
                    KimlikNo = kimlikno,
                });
                Configuration.Save();

            }
            else
            {
                var kontrol2 = Configuration.Instance.Cezalılar.FirstOrDefault(e => e.SteamİD == player.CSteamID);
                if (kontrol2 == null)
                {
                    var kontrol3 = Hapis.FirstOrDefault(e => e.SteamİD == player.CSteamID);
                    if (kontrol3 == null)
                    {
                        return;
                    }
                    else
                    {
                        if(kontrol3.Dakika == 0)
                        {
                            player.Player.teleportToLocation(Configuration.Instance.HapishaneÇıkışKonum,1);
                            Hapis.Remove(kontrol3);
                            return;
                        }
                    }
                }
                else
                {
                  if(kontrol2.cezaöde == true)
                    {
                        player.Experience = player.Experience = (uint)-kontrol2.CezaÜcreti;
                        UnturnedChat.Say(player, "Ceza Süreniz Dolduğu İçin Ceza Otomatik Ödendi", Color.yellow);
                        Configuration.Instance.Cezalılar.Remove(kontrol2);
                        var kontrol3 = Hapis.FirstOrDefault(e => e.SteamİD == player.CSteamID);
                        if (kontrol3 == null)
                        {
                            return;
                        }
                        else
                        {
                            if (kontrol3.Dakika == 0)
                            {
                                player.Player.teleportToLocation(Configuration.Instance.HapishaneÇıkışKonum, 1);
                                Hapis.Remove(kontrol3);
                                return;
                            }
                        }
                       
                    }
                    else
                    {
                        var kontrol3 = Hapis.FirstOrDefault(e => e.SteamİD == player.CSteamID);
                        if (kontrol3 == null)
                        {
                            return;
                        }
                        else
                        {
                            if (kontrol3.Dakika == 0)
                            {
                                player.Player.teleportToLocation(Configuration.Instance.HapishaneÇıkışKonum, 1);
                                Hapis.Remove(kontrol3);
                                return;
                            }
                        }
                       
                    }
                }
            }
        }

        private void button(Player player, string buttonName)
        {
            UnturnedPlayer pl = UnturnedPlayer.FromPlayer(player);
            var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
            
            if (buttonName == "çıkış")
            {
                EffectKapat(pl);
                player.serversideSetPluginModal(false);
            }
            if (buttonName == "anasayfa")
            {
                EffectKapat(pl);
                EffectManager.sendUIEffect(6000, 1, pl.CSteamID, true);
               
                
            }
            if(buttonName == "kimlikadam")
            {
               


                if (kontrol.sayı1 == 0)
                {
                    return;

                }
                else
                {
                    EffectManager.sendUIEffect(6010, 1, pl.CSteamID, true);

                    UnturnedPlayer pll = UnturnedPlayer.FromCSteamID(kontrol.yazı);

                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaranmıyor", "");
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaranıyor", "");
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaranmasebebi", "");
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsabıka", "");


                    var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                       
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtisim", "Vatandaş ismi: " + kontrol2.Oyuncuİsmi);
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtkimlik", "Vatandaş Kimlik No: "+kontrol2.KimlikNo.ToString());
                        if(pll == null)
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtbakiye", "Vatandaş Bakiyesi Aktif Olmadığı İçin Gözükmüyor");
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsunucu", "Çevrimdışı");
                    }
                    else
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtbakiye", "Vatandaş Bakiyesi: " + pll.Experience);
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsunucu", "Çevrimiçi");
                    }
            
                        
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsteamid", "Vatandaş Ülke No(SteamİD): "+kontrol2.SteamİD);
                        var kontrol3 = kontrol2.Sabıkal.FirstOrDefault(e => e.SabıkaSayısı == 1);
                        var kontrol4 = kontrol2.Araçlar.FirstOrDefault(e => e.AraçSayısı == 1);
                    if(kontrol3 == null)
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsabıka", "<color=red>Sabıkası Bulunmamakta</color>");

                    }
                    else
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsabıka", kontrol3.SabıkaNedeni);

                    }
                    if(kontrol4 == null)
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçisim", "<color=red>Aracı Bulunmamakta</color>");
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçplaka", "");
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtekleyenpolis", "");

                    }
                    else
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçisim", "Araç İsmi: " + kontrol4.Araçİsmi);
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçplaka", "Araç Plakası: " + kontrol4.AraçPlakası);
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtekleyenpolis", "Aracı Ekleyen Polis İsmi: " + kontrol4.EkleyenPolisİsmi);
                    }
                   

                    var kontrol5 = Arananlar.FirstOrDefault(e => e.SteamİD == kontrol2.SteamİD);
                    if(kontrol5 == null) 
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaranmıyor", "<color=red>Aranmıyor</color>");


                    }
                    else
                    {
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaranıyor", "Aranıyor");
                        EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaranmasebebi", "Aranma Nedeni: "+kontrol5.AranmaSebebi);


                    }








  
                }
            }
            if(buttonName == "gbtsabıkaileri")
            {
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                bool değer = kontrol.sayı1 == kontrol2.Sabıkal.Count;
                if (değer)
                {

                    kontrol.sayı1 = 1;
                }
                else
                {
                   

                    kontrol.sayı1++;
                }
                var kontrol3 = kontrol2.Sabıkal.FirstOrDefault(e => e.SabıkaSayısı == kontrol.sayı1);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsabıka", kontrol3.SabıkaNedeni);



            }
            if (buttonName == "gbtaraçileri")
            {

                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                bool değer = kontrol.sayı1 == kontrol2.Araçlar.Count;
                if (değer)
                {
                    kontrol.sayı2 = 1;
                }
                else
                {
                    kontrol.sayı2++;
                }
                var kontrol3 = kontrol2.Araçlar.FirstOrDefault(e => e.AraçSayısı == kontrol.sayı2);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçisim", "Araç İsmi: " + kontrol3.Araçİsmi );
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçplaka", "Araç Plaka: " + kontrol3.AraçPlakası );

                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtekleyenpolis", "Ekleyen Polis: "+kontrol3.EkleyenPolisİsmi);
            }
            if(buttonName == "gbtaraçageri")
            {
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                kontrol.sayı2 -= 1;
                var kontrol3 = kontrol2.Araçlar.FirstOrDefault(e => e.AraçSayısı == kontrol.sayı2);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçisim", "Araç İsmi: " + kontrol3.Araçİsmi);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtaraçplaka", "Araç Plaka: " + kontrol3.AraçPlakası);

                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtekleyenpolis", "Ekleyen Polis: " + kontrol3.EkleyenPolisİsmi);
            }

            if (buttonName == "gbtsabıkageri")
            {
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                kontrol.sayı1 -= 1;
                var kontrol3 = kontrol2.Sabıkal.FirstOrDefault(e => e.SabıkaSayısı == kontrol.sayı1);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "gbtsabıka", kontrol3.SabıkaNedeni);

            }
            if (buttonName == "geri")
            {
                EffectManager.askEffectClearByID(6010, pl.CSteamID);
                kontrol.sayı2 = 0;
                kontrol.sayı1 = 0;
                EffectManager.sendUIEffect(6000, 1, pl.CSteamID, true);




            }
            if (buttonName == "kimlikbt")
            {
                UnturnedPlayer pll = UnturnedPlayer.FromCSteamID(kontrol.yazı);
                if(kontrol.sayı1 == 0)
                {
                    return;
                }
                  
                   
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "uyarı", pll.CharacterName);

                

            }
            if (buttonName == "sabıkaekle")
            {
                EffectKapat(pl);
              
                EffectManager.sendUIEffect(6002, 1, pl.CSteamID, true);
            }
            if(buttonName == "ssabıkaekle")
            {
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);

                if (kontrol.sayı1 == 2)
                {
                    
                    int değer = kontrol2.Sabıkal.Count;
                    kontrol2.Sabıkal.Add(new Sabıka
                    {
                        SabıkaNedeni = kontrol.yazı2,
                        SabıkayıVerenPolisİsmi = pl.CharacterName,
                        SabıkaSayısı = değer+=1,
                    });
                    
                    Configuration.Save();
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "sonay", "Sabıka Başarıyla Eklendi");
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "soyuncuisim", "");
                    kontrol.SteamİD = pl.CSteamID;
                    kontrol.yazı2 = null;
                    kontrol.sayı1 = 1;

                    return;
                }
                kontrol.sayı1++;
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "soyuncuisim", "Vatandaş İsmi: "+kontrol2.Oyuncuİsmi);
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "sonay", "Onaylamak İçin Tekrardan Tıklayınız");
                

            }
            if (buttonName == "cezakes") 
            {
                EffectKapat(pl);
                EffectManager.sendUIEffect(6004, 1, pl.CSteamID, true);

            }
            if (buttonName == "cezaekle") 
            {
                EffectKapat(pl);
                EffectManager.sendUIEffect(6004, 1, pl.CSteamID, true);

            }
            if (buttonName == "ccezakesbt")
            {
                if(kontrol.sayı1 == 0 && kontrol.sayı2 == 0)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "cuyarı", "Tüm Bilgileri Doldurunuz");
                    return;
                }
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                Configuration.Instance.Cezalılar.Add(new CezaYiyenler
                {
                    SteamİD = kontrol2.SteamİD,
                    Oyuncuİsmi = kontrol2.Oyuncuİsmi,
                    CezaSüresi = kontrol.sayı2,
                    CezaÜcreti = kontrol.sayı1,
                    CezaNedeni = kontrol.yazı2,
                    cezaöde = false,
                });
                Configuration.Save();
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "cuyarı2", kontrol2.Oyuncuİsmi+" Adlı Oyuncuya Ceza Kesildi");
                UnturnedPlayer pll = UnturnedPlayer.FromCSteamID(kontrol.yazı);
                UnturnedChat.Say(pll, pl.CharacterName + " Adlı Polis Tarafından Ceza Kesildi", Color.yellow);
                kontrol.sayı1 = 0;
                kontrol.sayı2 = 0;
                kontrol.yazı = pl.CSteamID;
                kontrol.yazı2 = "";

            }
            if (buttonName == "araçsorgula")
            {
                EffectKapat(pl);
                EffectManager.sendUIEffect(6006, 1, pl.CSteamID, true);
                

            }
            if(buttonName == "aaraçsorgula")
            {
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                var kontrol3 = kontrol2.Araçlar.FirstOrDefault(e => e.AraçPlakası == kontrol.yazı2);
                
                if(kontrol.sayı1 == 0)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arabasahibiisim","Araç Plakası Girdiğinden Emin Ol");
                    return;
                }
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arabasahibiisim", "Araç Sahibinin İsmi: "+kontrol2.Oyuncuİsmi);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "araçsahibino", "Araç Sahibinin Kimlik No: " + kontrol2.KimlikNo);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "araçisim", "Aracın İsmi: " + kontrol3.Araçİsmi+" || "+"Araç Plakası: "+kontrol3.AraçPlakası);
                kontrol.yazı = pl.CSteamID;
                kontrol.sayı1 = 0;
                kontrol.yazı2 = "";

            }
            if(buttonName == "aaraçkaydet")
            {


                if(kontrol.sayı1 == 0)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "aauyarı", "Eksik Bilgi Girildi");
                    return;

                }
                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                if (Physics.Raycast(player.look.aim.position, player.look.aim.forward, out RaycastHit hit, 4, RayMasks.VEHICLE) && hit.transform.TryGetComponent(out InteractableVehicle vehicle))
                {
                    
                    int değer = kontrol2.Araçlar.Count;
                    kontrol2.Araçlar.Add(new KayıtlıAraçlar
                    {
                        AraçSahibiSteamİD = kontrol.yazı,
                        Araçİsmi = vehicle.name,
                        EkleyenPolisİsmi = pl.CharacterName,
                        AraçPlakası = kontrol.yazı2,
                        AraçSayısı = değer+=1,
                    });
                    KayıtlıAraç.Add(new KayıtlıAraçlar
                    {
                        AraçSahibiSteamİD = kontrol.yazı,
                        Araçİsmi = vehicle.name,
                        EkleyenPolisİsmi = pl.CharacterName,
                        AraçPlakası = kontrol.yazı2,
                        AraçSayısı = değer += 1,
                    });
                    kontrol.yazı = pl.CSteamID;
                    kontrol.sayı1 = 0;
                    kontrol.yazı2 = "";
                    Configuration.Save();
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "aauyarı", "Araç Başarıyla Kaydedildi");
                    return;
                }
                else
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "aauyarı", "Baktığınız Yerde Araç Yok");
                    kontrol.yazı = pl.CSteamID;
                    kontrol.sayı1 = 0;
                    kontrol.yazı2 = "";
                    return;
                }

            }
            if (buttonName == "arananlar")
            {
                EffectKapat(pl);
                EffectManager.sendUIEffect(6008, 1, pl.CSteamID, true);
                string[] Oyuncular = new string[6];
                int i = 0;
               foreach(var değer2 in Arananlar)
                {
                    if(i == 5)
                    {
                        break;
                    }
                    Oyuncular[i] = değer2.Oyuncuİsmi;
                }
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu1", Oyuncular[0]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu2", Oyuncular[1]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu3", Oyuncular[2]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu4", Oyuncular[3]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu5", Oyuncular[4]);


            }
            if (buttonName == "ArananEkle")
            {
                var değer = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                    

                Arananlar.Add(new Aranan
                {
                    SteamİD = değer.SteamİD,
                    KimlikNo = değer.KimlikNo.ToString(),
                    Oyuncuİsmi = değer.Oyuncuİsmi,
                    AranmaSebebi = kontrol.yazı2,
                });
                kontrol.yazı = pl.CSteamID;
                kontrol.yazı2 = "";
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı", "Arananlara Başarıyla Eklendi");
                string[] Oyuncular = new string[6];
                int i = 0;
                foreach (var değer2 in Arananlar)
                {
                    if (i == 5)
                    {
                        break;
                    }
                    Oyuncular[i] = değer2.Oyuncuİsmi;
                }
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu1", Oyuncular[0]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu2", Oyuncular[1]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu3", Oyuncular[2]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu4", Oyuncular[3]);
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananoyuncu5", Oyuncular[4]);



            }
            if (buttonName == "aranankaldır")
            {

                var değer = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                var değer2 = Arananlar.FirstOrDefault(e => e.KimlikNo == değer.KimlikNo.ToString());
                Arananlar.Remove(değer2);
               
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "arananuyarı (1)", "Arananlardan Başarıyla Kaldırıldı");

                kontrol.yazı = pl.CSteamID;
                kontrol.yazı2 = "";
            }
            if(buttonName == "hapseat")
            {
                EffectKapat(pl);
                EffectManager.sendUIEffect(6012, 1, pl.CSteamID, true);

            }
            if(buttonName == "hapistençıkar")
            {
                EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı2", "Vatandaş Hapisten Çıkarıldı");
                UnturnedPlayer pll = UnturnedPlayer.FromCSteamID(kontrol.yazı);
                var kontrol2 = Hapis.FirstOrDefault(e => e.SteamİD == kontrol.SteamİD);

                if (pll == null)
                {
                    kontrol2.Dakika = 0;
                    return;
                }
                pll.Player.teleportToLocation(Configuration.Instance.HapishaneÇıkışKonum, 1);
                Hapis.Remove(kontrol2);

            }
            if (buttonName == "hapsesok") 
            {

                var kontrol2 = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == kontrol.yazı);
                UnturnedPlayer pll = UnturnedPlayer.FromCSteamID(kontrol2.SteamİD);
                if(pll == null)
                {
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı2", "Vatandaş Aktif Değil");
                    return;

                }
                else
                {
                    pll.Player.teleportToLocation(Configuration.Instance.HapishaneKonum, 1);
                    Hapis.Add(new Hapishane
                    {
                        SteamİD = pll.CSteamID,
                        Oyuncuİsmi = pll.CharacterName,
                        Dakika = kontrol.sayı1,
                    });
                    EffectManager.sendUIEffectText(1, pl.CSteamID, true, "hapisuyarı2", "Vatandaş Hapse Sokuldu");

                }


            }


        }

        protected override void Unload()
        {

        }
        [RocketCommand("tabletaç", "Gelişmiş Polis Tabletini Açar", "", (AllowedCaller)1)]
        [RocketCommandPermission("O.PolisTablet")]
        public void polisTabletAç(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer pl = caller as UnturnedPlayer;
            var kontrol = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
            if(kontrol == null)
            {
                polis.Add(new Polisler
                {
                    Polisİsmi = pl.CharacterName,
                    SteamİD = pl.CSteamID,
                    yazı = pl.CSteamID,
                    sayı1 = 1,
                    sayı2 = 2,
                });
                EffectManager.sendUIEffect(6000, 1, pl.CSteamID, true);
                pl.Player.serversideSetPluginModal(true);
                return;
            }
            kontrol.sayı1 = 1;
            kontrol.sayı2 = 1;
            kontrol.yazı = pl.CSteamID;
            
            EffectManager.sendUIEffect(6000, 1, pl.CSteamID, true);
            pl.Player.serversideSetPluginModal(true);
            

        }
        [RocketCommand("kimlikno", "Kimlik No Gösteriri", "", (AllowedCaller)1)]
        [RocketCommandPermission("O.kimlikno")]
        public void kimlikno(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer pl = caller as UnturnedPlayer;
            var değer = Configuration.Instance.Sabıkalılar.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
              UnturnedChat.Say(pl, "Kimlik Numaran: " + değer.KimlikNo.ToString());

        }
        [RocketCommand("hapisgiriş", "Kimlik No Gösteriri", "", (AllowedCaller)1)]
        [RocketCommandPermission("O.hapisgiriş")]
        public void HapishanGiriş(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer pl = caller as UnturnedPlayer;
            Configuration.Instance.HapishaneKonum = pl.Position;
            Configuration.Save();
            UnturnedChat.Say(pl, "Hapishane Giriş Konumu Belirlendi");
        }
        [RocketCommand("hapisçıkış", "Hapishane Çıkış Konumu Belirlendi", "", (AllowedCaller)1)]
        [RocketCommandPermission("O.hapisçıkış")]
        public void HapishanÇıkış(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer pl = caller as UnturnedPlayer;
            Configuration.Instance.HapishaneÇıkışKonum = pl.Position;
            Configuration.Save();
            UnturnedChat.Say(pl, "Hapishane Çıkış Konumu Belirlendi");
        }
        private void EffectKapat(UnturnedPlayer pl)
        {
            var değer = polis.FirstOrDefault(e => e.SteamİD == pl.CSteamID);
            değer.sayı1 = 0;
            değer.sayı2 = 0;
            değer.yazı = pl.CSteamID;
            değer.yazı2 = "";
            EffectManager.askEffectClearByID(6000, pl.CSteamID);
            EffectManager.askEffectClearByID(6002, pl.CSteamID);
            EffectManager.askEffectClearByID(6004, pl.CSteamID);
            EffectManager.askEffectClearByID(6006, pl.CSteamID);
            EffectManager.askEffectClearByID(6008, pl.CSteamID);
            EffectManager.askEffectClearByID(6012, pl.CSteamID);
        }
    }
}

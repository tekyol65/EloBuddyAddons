using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace PentakillYasou
{
    internal class Program
    {
        public static Kombo, Drawlar, KillCal, LaneTemizle, SonVurus, HarasKullanmaSacma, HayatKurtaranItemler, menu;
        public static Spell.Skillshot Q;
        public static Spell.SpellBase W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static Spell.Targeted Ignite;
        public static HitChance QHitChance = HitChance.Unknown;
        public static int PentaKills;
        public static int EQRange = 375;
        public static int TurretRange
        {
            get
            {
                if (menu.Get<CheckBox>("BTR").CurrentValue)
                    return 875 + 100;
                else
                    return 875;
            }
        }
        public static AIHeroClient _Player { get { return ObjectManager.Player; } }
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Yasuo")
                return;

            Q = new Spell.Skillshot(SpellSlot.Q, 550, SkillShotType.Linear);
            W = new Spell.Skillshot(SpellSlot.W, 400, SkillShotType.Linear);
            E = new Spell.Targeted(SpellSlot.E, 475);
            R = new Spell.Active(SpellSlot.R)
            {
                Range = 1400
            };

            menu = MainMenu.AddMenu("Pentakill Yasou", "PentakillYasou");
            menu.Add("ABOUT", new Label("Bu Addon Tekyol65 Tarafindan derlenmistir."));
            menu.Add("BTR", new CheckBox("Extend Turret Range (safety precaution)", false));
            menu.Add("QHitChance", new Slider("Q Hit Chance: Medium", 2, 0, 3));

            Kombo = menu.AddSubMenu("Kombo", "Kombo");

            Kombo.AddGroupLabel("Kombo Settings");
            Kombo.Add("CQ", new CheckBox("Kullan Q"));
            Kombo.Add("CE", new CheckBox("Kullan E"));
            Kombo.Add("CEQ", new CheckBox("Kullan EQ"));
            Kombo.Add("CR", new CheckBox("Kullan R"));
            Kombo.Add("CI", new CheckBox("Kullan HayatKurtaranItemler"));
            Kombo.Add("CEUT", new CheckBox("E Kule Altinda", false));
            Kombo.AddGroupLabel("Ulti Ayarlari - iki düsman görüsteyse");
            Kombo.Add("UltLS", new CheckBox("Ultiyi kac Saniyede Atacan"));
            Kombo.Add("UltAEIV", new CheckBox("2 kisi hopladiysa patlat ultiyi"));
            Kombo.Add("UltHEIV", new CheckBox("1 kisi hopladiysa patlat ultiyi"));
            Kombo.Add("UltLH", new CheckBox("hp %10 dan düsükse patlat ultiyi", false));
            Kombo.Add("UltREnemies", new Slider("Kac düsmana ulti atsin(delikanli adam 5 yapar)", 3, 0, 5));


            LaneTemizle = menu.AddSubMenu("Lane Temizle", "LaneTemizle");
            LaneTemizle.AddGroupLabel("Lane Temizle Settings");
            LaneTemizle.Add("LCQ", new CheckBox("Kullan Q"));
            LaneTemizle.Add("LC3Q", new CheckBox("Kullan 3Q"));
            LaneTemizle.Add("LCE", new CheckBox("Kullan E"));
            LaneTemizle.Add("LCEQ", new CheckBox("Kullan EQ"));
            LaneTemizle.Add("LCELH", new CheckBox("E yi Sadece Son Vurus icin Kullan"));
            LaneTemizle.Add("LCEUT", new CheckBox("Kule Alti E", false));
            LaneTemizle.Add("LCI", new CheckBox("Kullan HayatKurtaranItemler (Hydra/Timat)"));

            HarasKullanmaSacma = menu.AddSubMenu("HarasKullanmaSacma", "HarasKullanmaSacma");
            HarasKullanmaSacma.AddGroupLabel("HarasKullanmaSacma Settings");
            HarasKullanmaSacma.Add("HQ", new CheckBox("Kullan Q"));
            HarasKullanmaSacma.Add("HE", new CheckBox("Kullan E"));
            HarasKullanmaSacma.Add("HEQ", new CheckBox("Kullan EQ"));
            HarasKullanmaSacma.Add("HEUT", new CheckBox("E Kule Alti", false));
            HarasKullanmaSacma.Add("HI", new CheckBox("Kullan HayatKurtaranItemler (Hydra/Timat)"));
            HarasKullanmaSacma.AddGroupLabel("Auto-HarasKullanmaSacma Ayarlari");
            HarasKullanmaSacma.Add("AHQ", new CheckBox("Auto-HarasKullanmaSacma Q Kullan"));
            HarasKullanmaSacma.Add("AH3Q", new CheckBox("Auto-HarasKullanmaSacma 3. Q kullan", false));

            SonVurus = menu.AddSubMenu("Son Vurus", "SonVurus menu");
            SonVurus.AddGroupLabel("Son vurus ayarlari");
            SonVurus.Add("LHQ", new CheckBox("Kullan Q"));
            SonVurus.Add("LHQ3", new CheckBox("Kullan 3Q"));
            SonVurus.Add("LHE", new CheckBox("Kullan E"));
            SonVurus.Add("LHEQ", new CheckBox("Kullan EQ"));
            SonVurus.Add("LHEUT", new CheckBox("E KuleAlti", false));

            KillCal = menu.AddSubMenu("Kill Cal", "KillCal");
            KillCal.AddGroupLabel("Kill Calma Ayarlari");
            KillCal.Add("EnableKS", new CheckBox("KS"));
            KillCal.Add("KSQ", new CheckBox("KS ile Q"));
            KillCal.Add("KS3Q", new CheckBox("KS ile 3rd Q"));
            KillCal.Add("KSE", new CheckBox("KS ile E"));
            KillCal.Add("KSEQ", new CheckBox("KS ile EQ"));
            KillCal.Add("KSI", new CheckBox("KS ile Ignite"));
            KillCal.Add("KSEUT", new CheckBox("E Kule Alti", false));

            Drawlar = menu.AddSubMenu("Drawlars", "Drawlar");
            Drawlar.AddGroupLabel("Draw Ayarlari");
            Drawlar.Add("DQ", new CheckBox("Draw Q"));
            Drawlar.Add("DW", new CheckBox("Draw W"));
            Drawlar.Add("DE", new CheckBox("Draw E"));
            Drawlar.Add("DR", new CheckBox("Draw R"));
            Drawlar.Add("DT", new CheckBox("Draw Turret Range", false));

            HayatKurtaranItemler = menu.AddSubMenu("HayatKurtaranItemler", "HayatKurtaranItemlermenu");
            HayatKurtaranItemler.AddGroupLabel("Item Ayarlari");
            HayatKurtaranItemler.Add("HayatKurtaranItemlerT", new CheckBox("Kullan Tiamat"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerRH", new CheckBox("Kullan Ravenous Hydra"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerTH", new CheckBox("Kullan Titanic Hydra"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerBC", new CheckBox("Kullan Bilgewater Cutlass"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerBORK", new CheckBox("Kullan Blade of the Ruined King"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerY", new CheckBox("Kullan Youmuus"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerQSS", new CheckBox("Kullan Quick Silversash"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerMS", new CheckBox("Kullan Mercurial Scimitar"));
            HayatKurtaranItemler.Add("HayatKurtaranItemlerPotions", new CheckBox("Kullan Pot"));
            HayatKurtaranItemler.AddGroupLabel("QSS/Merc Scimitar Aylari");
            HayatKurtaranItemler.Add("QSSBlind", new CheckBox("Blind"));
            HayatKurtaranItemler.Add("QSSCharm", new CheckBox("Charm"));
            HayatKurtaranItemler.Add("QSSFear", new CheckBox("Fear"));
            HayatKurtaranItemler.Add("QSSKB", new CheckBox("Knockback"));
            HayatKurtaranItemler.Add("QSSSilence", new CheckBox("Silence"));
            HayatKurtaranItemler.Add("QSSSlow", new CheckBox("Slow"));
            HayatKurtaranItemler.Add("QSSSnare", new CheckBox("Snare"));
            HayatKurtaranItemler.Add("QSSStun", new CheckBox("Stun"));
            HayatKurtaranItemler.Add("QSSTaunt", new CheckBox("Taunt"));
            HayatKurtaranItemler.AddGroupLabel("Pot Ayarlari");
            HayatKurtaranItemler.Add("PotSlider", new Slider("Kullan Potion at Health Percent", 65, 1, 100));

            Spellbook spell = _Player.Spellbook;
            SpellDataInst Sum1 = spell.GetSpell(SpellSlot.Summoner1);
            SpellDataInst Sum2 = spell.GetSpell(SpellSlot.Summoner2);
            if (Sum1.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            else if (Sum2.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);

            Game.OnTick += Game_OnTick;
            Game.OnTick += WindWall.GameOnTick;
            Game.OnUpdate += WindWall.GameOnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Obj_AI_Base.OnCreate += WindWall.OnCreate;
            Obj_AI_Base.OnDelete += WindWall.OnDelete;
            Obj_AI_Base.OnUpdatePosition += WindWall.OnUpdate;
            menu.Get<Slider>("QHitChance").OnValueChange += OnHitChanceSliderChange;
            UnsignedEvade.SpellDatabase.Initialize();
            WindWall.OnGameLoad();
            PentaKills = _Player.PentaKills;

            OnHitChanceSliderChange(menu.Get<Slider>("QHitChance"), null);
        }
        
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead)
                return;

            if (Drawlar["DQ"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                Drawing.DrawCircle(_Player.Position, Q.Range, System.Drawing.Color.BlueViolet);
            if (Drawlar["DE"].Cast<CheckBox>().CurrentValue && E.IsLearned)
                Drawing.DrawCircle(_Player.Position, E.Range, System.Drawing.Color.BlueViolet);
            if (Drawlar["DW"].Cast<CheckBox>().CurrentValue && W.IsLearned)
                Drawing.DrawCircle(_Player.Position, W.Range, System.Drawing.Color.BlueViolet);
            if (Drawlar["DR"].Cast<CheckBox>().CurrentValue && R.IsLearned)
                Drawing.DrawCircle(_Player.Position, Program.R.Range, System.Drawing.Color.BlueViolet);

            if (Drawlar["DT"].Cast<CheckBox>().CurrentValue)
                foreach (Obj_AI_Turret t in EntityManager.Turrets.Enemies)
                    Drawing.DrawCircle(t.Position, TurretRange, System.Drawing.Color.BlueViolet);
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (_Player.IsDead)
                return;

            Q = YasuoFunctions.GetQType();
            YasuoFunctions.AutoHarrass();
            YasuoFunctions.KullanHayatKurtaranItemlerAndIgnite(YasuoFunctions.Mode.PotionManager);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                YasuoFunctions.Combo();
            if (KillCal["EnableKS"].Cast<CheckBox>().CurrentValue)
                YasuoFunctions.KS();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.SonVurus))
                YasuoFunctions.SonVurus();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.HarasKullanmaSacma))
                YasuoFunctions.Harrass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneTemizle) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                YasuoFunctions.LaneTemizle();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                YasuoFunctions.Flee();
            if (_Player.PentaKills > PentaKills)
            {
                Chat.Print("ADAMIN DIBI KOYDU KAFAYA PENTAYI GG WP");
                PentaKills = _Player.PentaKills;
            }
        }

        static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs buff)
        {
            if (sender.IsMe && buff.Buff.Name == "yasuoq3w")
                Q =  new Spell.Skillshot(SpellSlot.Q, 1000, SkillShotType.Linear);
        }
        static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs buff)
        {
            if (sender.IsMe && buff.Buff.Name == "yasuoq3w")
                Q =  new Spell.Skillshot(SpellSlot.Q, 550, SkillShotType.Linear);
        }

        static void OnHitChanceSliderChange(ValueBase sender, EventArgs args)
        {
            Slider slider = sender.Cast<Slider>();
            int value = slider.CurrentValue;

            if (value == 0)
            {
                slider.DisplayName = "Q Hit Chance: Any";
                QHitChance = HitChance.Unknown;
            }
            else if (value == 1)
            {
                slider.DisplayName = "Q Hit Chance: " + HitChance.Low.ToString();
                QHitChance = HitChance.Low;
            }
            else if (value == 2)
            {
                slider.DisplayName = "Q Hit Chance: " + HitChance.Medium.ToString();
                QHitChance = HitChance.Medium;
            }
            else if (value == 3)
            {
                slider.DisplayName = "Q Hit Chance: " + HitChance.High.ToString();
                QHitChance = HitChance.High;
            }
        }
    }
}

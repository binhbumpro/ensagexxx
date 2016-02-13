using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace Born_To_Be_Phantom_Assasin
{
     class Program
    {
        private static Item abyssalblade, BKB, satanic;
        private static Font _text;
        private static Hero me;
        private static Hero target;
        private static bool _loaded;
        private static string map;
        private static float _myHull;
        private static AbilityToggler menuValue;
        //private static bool statechanged;
        private static bool menuadded;

        private static readonly Dictionary<int, ParticleEffect> Effect = new Dictionary<int, ParticleEffect>();
        private static ParticleEffect effect;
        //private static readonly int[] qDmgcs = new int[5] { 0, 60, 100, 140, 180 };
        //private static readonly int[] qDmg = new int[5] { 0, 30, 50, 70, 90 };

        private static Menu Menu;

        private static void Main()
        {

            Events.OnLoad += On_Load;
            Events.OnClose += On_Close;
            Game.OnUpdate += Game_OnUpdate;
            //Game.OnUpdate += SpellsUsage;
        }
        private static void On_Load(object sender, EventArgs e)
        {
            me = ObjectMgr.LocalHero;
            if (me.ClassID == ClassID.CDOTA_Unit_Hero_PhantomAssassin)
            {
                if (!menuadded)
                {
                    InitMenu();
                    menuadded = true;
                }
                //statechanged = true;
                //map = Game.ShortLevelName;
            }
        }

        private static void On_Close(object sender, EventArgs e)
        {
            if (menuadded) Menu.RemoveFromMainMenu();
            if (Effect.TryGetValue(1, out effect))
            {
                effect.Dispose();
                Effect.Remove(1);
            }
            if (Effect.TryGetValue(2, out effect))
            {
                effect.Dispose();
                Effect.Remove(2);
            }
            if (Effect.TryGetValue(3, out effect))
            {
                effect.Dispose();
                Effect.Remove(3);
            }
            menuadded = false;
        }
        private static void InitMenu()
        {
            var itemdict = new Dictionary<string, bool>
                           {
                               { "item_abyssal_blade", true }, { "item_satanic", true},
                               { "item_black_king_bar", true }
                           };
            Menu = new Menu("BTB Phantom Assasin", "PA");
            Menu.AddItem(new MenuItem("keyBind", "Combo Key").SetValue(new KeyBind('G', KeyBindType.Press)));
            var comboMenu = new Menu("Combo", "combomenu", false, @"..\other\statpop_exclaim", true);
            comboMenu.AddItem(new MenuItem("enabledAbilities", "Items:").SetValue(new AbilityToggler(itemdict)));
            comboMenu.AddItem(
                new MenuItem("SpellQ", "Use Stifling Dagger").SetValue(true));
            comboMenu.AddItem(
                new MenuItem("SpellW", "Use Phantom Strike").SetValue(true));
            comboMenu.AddItem(
                new MenuItem("Wrange", "Min range to use W").SetValue(new Slider(0, 10, 1000)));
            comboMenu.AddItem(
                new MenuItem("UseSatanic", "% Heal to use satanic").SetValue(new Slider(0, 10, 100)));
            comboMenu.AddItem(
                new MenuItem("targetsearchrange", "Target Search Range").SetValue(new Slider(1000, 128, 2500))
                    .SetTooltip("Radius of target search range around cursor."));

            Menu.AddToMainMenu();


        }
        private static void Game_OnUpdate(EventArgs args)
        {
            target = me.ClosestToMouseTarget(Menu.Item("targetsearchrange").GetValue<Slider>().Value);
            var distance = me.Distance2D(target);
            //var bopby = Menu.Item("targetsearchrange").GetValue<Slider>().Value; // khoảng cách nắm bắt mục tiêu
            var inv = me.Inventory.Items; // check hòm đồ
            var enumerable = inv as Item[] ?? inv.ToArray();
            var dagger = enumerable.Any(x => x.Name == "item_blink" && x.Cooldown == 0); // add blink
            if (!_loaded)
            {
                if (!Game.IsInGame || me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_PhantomAssassin)
                {
                    return;
                }
              
                _loaded = true;
                Game.PrintMessage(
                "<font face='Comic Sans MS, cursive'><font color='#00aaff'>" + Menu.DisplayName + " By Bopby" +
                " loaded!</font> <font color='#aa0000'>v" + Assembly.GetExecutingAssembly().GetName().Version,
                MessageType.LogMessage);
                if (target != null && Menu.Item("keyBind").GetValue<KeyBind>().Active == true) {
                    SpellsUsage(me, target, distance, dagger);
                }
            }

            if (!Game.IsInGame || me == null)
            {
                _loaded = false;
                return;
            }
            if (Game.IsPaused) return;

            if (!menuadded) return;
        }
        private static void SpellsUsage(Hero me, Hero target, double distance, bool daggerIsReady)
        {
            //var qrange = 1200;
            //var wrange = 1000;
            var spellbook = me.Spellbook;
            var q = spellbook.SpellQ;
            var w = spellbook.SpellW;
            var e = spellbook.SpellE;
            
            //item
            abyssalblade = me.FindItem("item_abyssal_blade");

            BKB = me.FindItem("item_abyssal_blade");
            satanic = me.FindItem("item_satanic");
            if (target != null && Menu.Item("keyBind").GetValue<KeyBind>().Active == true)
            {
                if (q != null && q.CanBeCasted() && q.CastRange >= distance && Utils.SleepCheck(me.Handle + q.Name))
                {
                    q.UseAbility(target);
                    Utils.Sleep(500, me.Handle + q.Name);
                }
                if (w != null && w.CanBeCasted() && me.Distance2D(target) >= Menu.Item("Wrange").GetValue<Slider>().Value && Utils.SleepCheck(me.Handle + w.Name))
                {
                    w.UseAbility(target);
                    Utils.Sleep(500, me.Handle + w.Name);
                }
                if (abyssalblade != null && abyssalblade.CanBeCasted() && me.Distance2D(target) <= 140)
                {
                    abyssalblade.UseAbility(target);
                    Utils.Sleep(Game.Ping, "abyssalblade");

                }
                if (BKB != null && BKB.CanBeCasted())
                {
                    BKB.UseAbility();
                    Utils.Sleep(Game.Ping, "BKB");
                }
                if (satanic != null && satanic.CanBeCasted() && target.Health >= Menu.Item("UseSatanic").GetValue<Slider>().Value)
                {
                    satanic.UseAbility();
                    Utils.Sleep(Game.Ping, "satanic");
                }

            }
        }
    }
}
    


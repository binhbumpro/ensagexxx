using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using System.Windows.Input;
using SharpDX;

namespace Courrier_Killer
{
    internal static class Program
    {
        private static readonly Menu Menu = new Menu("Auto kill enemy Couriers", "ab", true);
        private static bool _loaded;
        private static void Main()

        {
            Menu.AddItem(new MenuItem("Kill", "Auto kill enemy Couriers").SetValue(new KeyBind('I', KeyBindType.Toggle, false)).SetTooltip("auto AA enemy Couriers"));
            //Menu.AddItem(new MenuItem("aa", "Number aa to kill").SetValue(new Slider(1, 1, 5)));
            Menu.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;

        }

        private static void On_Load(object sender, EventArgs e)
        {
            if (!_loaded)
            {
                if (!Game.IsInGame)
                {
                    return;
                }
                _loaded = true;
            }

            if (!Game.IsInGame)
            {
                _loaded = false;
                return;
            }

        }



        private static void Game_OnUpdate(EventArgs args)
        {

            if (!Utils.SleepCheck("rate"))
                return;




            var me = ObjectMgr.LocalHero;
            var range = me.AttackRange;
            var couriers = ObjectMgr.GetEntities<Courier>().Where(x => x.IsAlive && x.Team != me.Team);

            //var enemies = ObjectMgr.GetEntities<Hero>().Where(x => x.IsAlive && !x.IsIllusion && x.Team != me.Team).ToList();



            //var heal = couriers.heal;
            var aa = me.MinimumDamage  ;
            //var heal = couriers.Mi ;


            if (!_loaded)
            {
                if (!Game.IsInGame || me == null || !me.IsAlive)
                {
                    return;
                }
                _loaded = true;
            }

            if (!Game.IsInGame || me == null)
            {
                _loaded = false;
                return;
            }
            if (Game.IsPaused) return;
            //foreach (var enemy in enemies)
                foreach (var courier in couriers)

                    if (me.Distance2D(courier) <= range && Menu.Item("Kill").GetValue<KeyBind>().Active /*&& aa <= (Menu.Item("aa").GetValue<Slider>().Value)*/)
                    {
                        me.Attack(courier);
                    }
        }
    }
}
    




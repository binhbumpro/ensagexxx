using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using SharpDX;
using SharpDX.Direct3D9;

namespace Born_To_Use_Item
{
    internal class Program
    {
        private static bool inGame;
        private static Hero hero;

        private static bool cameraCentered;
        private static Font text;

        private static Ability spell;
        private static float spellCastRange;
        private static float distance;
        private static double castPoint;
        private static double angle;
        private static readonly Menu Menu = new Menu("Born to use item", "BTUI", true);
        private static bool _loaded;
        static void Main()
        {
            Menu.AddItem(new MenuItem("useBKB", "Use black king bar").SetValue(true));
            Menu.AddItem(new MenuItem("usepBLM", "Use blade mail").SetValue(true));
            Menu.AddToMainMenu();
            // menu
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
            if (me == null || !me.IsAlive)
                return;
            var phandame = hero.Inventory.Items.FirstOrDefault(blm => blm.Name.Contains("item_blade_mail"));
            var lotus = hero.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_lotus_orb"));
            var BKB = hero.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_black_king_bar"));
            var pipe = hero.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_pipe"));
            var Eul = hero.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_cyclone"));
            if (phandame != null && Menu.Item("useBLM").GetValue<bool>() == true)
            {
                if (phandame.Cooldown == 0)
                {
                    phandame.UseAbility(hero.Position);
                }
            }


                // 
                var enemies =
                ObjectMgr.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team == hero.GetEnemyTeam()).ToList();

                foreach (var enemy in enemies)
                {
                    switch (enemy.ClassID)
                    {
                        case ClassID.CDOTA_Unit_Hero_Zuus:
                            {
                                spell = enemy.Spellbook.SpellR;

                                if (spell.IsInAbilityPhase)
                                {

                                    int[] damage = { 225, 350, 475 };

                                    if (enemy.AghanimState())
                                    {
                                        damage[0] = 440;
                                        damage[1] = 540;
                                        damage[2] = 640;
                                    }

                                    if (hero.Health <= hero.DamageTaken(damage[spell.Level - 1], DamageType.Magical, enemy))
                                    {
                                        if (BKB != null && Menu.Item("useBKB").GetValue<bool>() == true)
                                        {
                                            if (BKB.Cooldown == 0)
                                            {
                                                BKB.UseAbility(hero);
                                            }
                                        }
                                    }
                                    //if (enemy.Health <= enemy.DamageTaken(damage[spell.Level - 1], DamageType.Magical, enemy))
                                    //{
                                    if (phandame != null && Menu.Item("useBLM").GetValue<bool>() == true)
                                    {
                                        if (phandame.Cooldown == 0)
                                        {
                                            phandame.UseAbility(hero);
                                        }
                                        //}
                                    }
                                }


                                break;

                                //get zeus ulti

                            }


                    }



                }
            }
        }
    }

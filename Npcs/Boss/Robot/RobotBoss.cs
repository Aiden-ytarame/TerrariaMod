
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using test.Npcs.Boss.Robot.Assets;

namespace test.Npcs.Boss.Robot
{
    [AutoloadBossHead]
    public class RobotBoss : ModNPC
    {
        readonly List<int> P1Attacks = new List<int> { 0, 1, 3 };
        readonly List<int> P2Attacks = new List<int> { 6, 7, 8, 9, 10 };
        readonly List<int> P3Attacks = new List<int> { 12, 9 };

        List<int> attackOrder = new List<int>();

        Vector2 PlatformPosition;
        Projectile[] telegraphs = new Projectile[16];
        int lastAttack = -1;
        float LaserRot = MathHelper.ToRadians(90 / 30);
        int cooldown;

        SoundStyle stun = new SoundStyle("test/Npcs/Boss/Robot/Assets/robotStun");

        int currentAttack
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        int currentPhase
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        Vector2 TargetPosition
        {
            get => new Vector2(NPC.ai[2], NPC.ai[3]);
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }

        int timer
        {
            get => (int)NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        int attackCondtion1
        {
            get => (int)NPC.localAI[1];
            set => NPC.localAI[1] = value;
        }

        int attackCondtion2
        {
            get => (int)NPC.localAI[2];
            set => NPC.localAI[2] = value;
        }

        int miscProperty
        {
            get => (int)NPC.localAI[3];
            set => NPC.localAI[3] = value;
        }

        #region default
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to. Most NPCs are immune to Confused.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            // This boss also becomes immune to OnFire and all buffs that inherit OnFire immunity during the second half of the fight. See the ApplySecondStageBuffImmunities method.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "ExampleMod/Assets/Textures/Bestiary/MinionBoss_Preview",
                PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 110;
            NPC.damage = 50;
            NPC.defense = 22;
            NPC.lifeMax = 190000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
            NPC.netUpdate = true;
            currentAttack = -1;
            TargetPosition = Vector2.Zero;
            attackCondtion1 = 0;
            attackCondtion2 = 0;
            currentAttack = -1;
            // Default buff immunities should be set in SetStaticDefaults through the NPCID.Sets.ImmuneTo{X} arrays.
            // To dynamically adjust immunities of an active NPC, NPC.buffImmune[] can be changed in AI: NPC.buffImmune[BuffID.OnFire] = true;
            // This approach, however, will not preserve buff immunities. To preserve buff immunities, use the NPC.BecomeImmuneTo and NPC.ClearImmuneToBuffs methods instead, as shown in the ApplySecondStageBuffImmunities method below.

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar
            //NPC.BossBar = ModContent.GetInstance<MinionBossBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Music/Tier2");

            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Example Minion Boss that spawns minions on spawn, summoned with a spawn item. Showcases boss minion handling, multiplayer conciderations, and custom boss bar.")
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            currentAttack = -1;
            timer = 60;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
            return true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(PlatformPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            PlatformPosition = reader.ReadVector2();
        }
        #endregion
        void ChooseRandomAttack()
        {
         
            attackCondtion1 = attackCondtion2 = miscProperty = 0;
            TargetPosition = Vector2.Zero;

            if (checkTransitions())
                return;
            
            if (timer > 0)
            {
                Idle();
                return;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (attackOrder.Count == 0)
                {

                    if (currentPhase == 0)
                    {
                        attackOrder = new List<int>(P1Attacks);
                        if (cooldown % 2 == 0)
                            attackOrder.Add(2);
                        Main.NewText("looped p1");
                    }


                    if (currentPhase == 2)
                    {
                        attackOrder = new List<int>(P2Attacks);
                        Main.NewText("looped p2");
                    }

                    if (currentPhase == 3)
                    {
                        attackOrder = new List<int>(P3Attacks);
                        if (cooldown % 5 == 0)
                            attackOrder.Add(13);
                        Main.NewText("looped p3");
                    }

                }

                int nextAttack = lastAttack;

                while (nextAttack == lastAttack)
                {
                    nextAttack = attackOrder[Main.rand.Next(attackOrder.Count)];
                }
                Main.NewText(nextAttack);
                attackOrder.Remove(nextAttack);
                currentAttack = nextAttack;
                cooldown++;
                NPC.netUpdate = true;
            }
        }

        bool checkTransitions()
        {
            if (currentPhase == 0)
                if (hasHealth(60))
                {
                    currentAttack = 4;
                    return true;
                }

            if (currentPhase == 1)
                if (hasHealth(55))
                {
                    currentAttack = 5;
                    return true;
                }

            if (currentPhase == 2)
                if (hasHealth(30))
                {
                    currentAttack = 11;
                    return true;
                }
            return false;
        }
        public override void AI()
        {
            if(currentPhase == 3)
            {
                Main.LocalPlayer.wingTime = 0;
                Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);
                Main.LocalPlayer.AddBuff(BuffID.ChaosState, 3, true);
            }

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }


            if (timer > 0)
                timer--;

            NPC.velocity = LerpVector2(NPC.velocity, Vector2.Zero, 0.05f);

            if (NPC.velocity.Length() < 10)
                NPC.rotation = NPC.rotation.AngleTowards(GetDirToPlayer().ToRotation() - MathHelper.PiOver2, 0.1f);
            else
                NPC.rotation = NPC.rotation.AngleTowards(NPC.velocity.ToRotation() - MathHelper.PiOver2, 0.1f);


            switch (currentAttack)
            {
                case 0:
                    ShootAndDash();
                    break;

                case 1:
                    LasersAllAround();
                    break;

                case 2:
                    SpawnKamikaze();
                    break;

                case 3:
                    missAndHit();
                    break;

                case 4:
                    Tantrum();
                    break;

                case 5:
                    TransitiomPhase2();
                    break;

                case 6:
                    CrossLaser();
                    break;

                case 7:
                    SingleLaser();
                    break;

                case 8:
                    TpAndShoot();
                    break;

                case 9:
                    SwordAndSlash();
                    break;

                case 10:
                    LasersFromAbove();
                    break;

                case 11:
                    TransitionToSky();
                    break;

                case 12:
                    Sweep();
                    break;

                case 13:
                    BlockArena();
                    break;

                default:
                    ChooseRandomAttack();
                    break;

            }

        }


        void Idle(float speed = 21f, float acceleration = 0.04f)
        {
            Vector2 idlePos = (Main.player[NPC.target].Center + new Vector2(0, -500)) - NPC.Center;
            float distace = Vector2.Distance(NPC.position, Main.player[NPC.target].position);
            NPC.velocity = LerpVector2(NPC.velocity, Vector2.Normalize(idlePos) * (distace > 300 ? speed * 1.5f : speed), acceleration);
        }

        #region phase1
        void ShootAndDash()
        {
            //spawn all projectiles before tp
            //spawn them every X ticks X amount of times
            //condition1 projectiles to spawn

            if (attackCondtion1 < 10)
            {
                NPC.velocity = LerpVector2(NPC.velocity, TargetPosition * 12, 0.2f);

                if (timer <= 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.InheritSource(NPC), NPC.Center, Main.rand.NextVector2Unit() * 5, ProjectileID.HallowBossRainbowStreak, 70, 1);
                    timer = 8;
                    attackCondtion1++;

                    if (attackCondtion1 >= 10)
                        timer = 100;
                }

                return;
            }

            if (attackCondtion2 == 0 && timer > 0)
            {
                Idle();
            }

            if (attackCondtion2 == 0 && timer <= 0)
            {
                if (Main.player[NPC.target].velocity != Vector2.Zero)
                    TargetPosition = Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 30;
                else
                    TargetPosition = Main.player[NPC.target].Center;

                NPC.netUpdate = true;
                attackCondtion2 = 1;
                timer = 50;
            }

            if (attackCondtion2 == 1 && timer > 0)
            {
                Idle(17);
                Dust dust = Dust.NewDustDirect(TargetPosition, 50, 50, DustID.YellowTorch, Scale: 4f);
                dust.noGravity = true;
                dust.velocity *= 5;

            }


            if (attackCondtion2 == 1 && timer <= 0)
            {
                NPC.Teleport(TargetPosition, TeleportationStyleID.TeleportationPotion);

                Mod.Logger.Debug(Vector2.Distance(TargetPosition, Main.player[NPC.target].position));

                TargetPosition = GetDirToPlayer();
                attackCondtion2 = 2;
                timer = 28;
                NPC.velocity = -TargetPosition * 4;
            }

            if (attackCondtion2 == 2 && timer > 0)
            {
                NPC.velocity = LerpVector2(NPC.velocity, TargetPosition * 22, 0.25f);
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
            }

            if (attackCondtion2 == 2 && timer <= 0)
            {
                attackCondtion2 = 3;
                timer = 30;
            }

            if (attackCondtion2 == 3 && timer <= 0)
            {
                timer = 75;

                lastAttack = currentAttack;
                currentAttack = -1;
                NPC.netUpdate = true;
            }
        }

        Vector2 ProjRotOriginal;
        void LasersAllAround()
        {
            if (NPC.velocity != Vector2.Zero)
                NPC.velocity = Vector2.Zero;

            if (attackCondtion1 == 0)
            {
                ProjRotOriginal = Main.rand.NextVector2Unit();
                TargetPosition = ProjRotOriginal;
            }

            if (attackCondtion1 < 16)
            {

                if (timer <= 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        double rotStep = MathHelper.ToRadians(22.5f);

                        telegraphs[attackCondtion1] = Projectile.NewProjectileDirect(Projectile.InheritSource(NPC), NPC.Center, TargetPosition, ModContent.ProjectileType<LaserTelegraph>(), 0, 0);
                        TargetPosition = TargetPosition.RotatedBy(rotStep);
                    }
                    timer = 2;
                    attackCondtion1++;

                    if (attackCondtion1 >= 16)
                        timer = 34;
                }

                return;
            }

            if (attackCondtion2 == 0 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        telegraphs[i].timeLeft = 0;
                    }
                    TargetPosition = ProjRotOriginal;
                    double rotStep = MathHelper.ToRadians(22.5f);
                    for (int i = 0; i < 16; i++)
                    {

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, TargetPosition, ModContent.ProjectileType<Laser>(), 70, 1);
                        TargetPosition = TargetPosition.RotatedBy(rotStep);
                    }

                }
                attackCondtion2 = 1;
            }


            if (attackCondtion2 == 1)
            {
                timer = 30;
                lastAttack = currentAttack;
                currentAttack = -1;
                NPC.netUpdate = true;
            }
        }

        void SpawnKamikaze()
        {
            Idle(14);

            if (timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC kam = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Kamikaze>());

                    if (attackCondtion1 % 2 == 0)
                        kam.velocity = Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.PiOver2) * 10;

                    else
                        kam.velocity = Vector2.Normalize(NPC.velocity).RotatedBy(-MathHelper.PiOver2) * 10;

                    NetMessage.SendData(MessageID.SyncNPC, number: kam.whoAmI);
                }
                timer = 12;
                attackCondtion1++;

                if (attackCondtion1 > 4)
                {
                    timer = 48;
                    lastAttack = currentAttack;
                    currentAttack = -1;
                    NPC.netUpdate = true;
                }
            }


        }
        Projectile Holder1;
        Projectile Holder2;
        void missAndHit()
        {
            if (attackCondtion1 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, TargetPosition, ModContent.ProjectileType<LaserTelegraph>(), 0, 0);
                    Holder1.timeLeft = 91;
                    Holder2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, TargetPosition, ModContent.ProjectileType<LaserTelegraph>(), 0, 0);
                    Holder2.timeLeft = 91;
                }
                timer = 90;
                attackCondtion1++;
            }
            if (attackCondtion1 == 1 && timer > 0)
            {

                if (timer > 30)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Idle(10);
                        TargetPosition = Main.player[NPC.target].Center;
                        Vector2 dir = Vector2.Normalize(TargetPosition - NPC.Center);

                        Holder1.Center = NPC.Center;
                        Holder1.velocity = (TargetPosition + dir.RotatedBy(MathHelper.PiOver2) * 55) - NPC.Center;
                        Holder2.Center = NPC.Center;
                        Holder2.velocity = (TargetPosition - dir.RotatedBy(MathHelper.PiOver2) * 55) - NPC.Center;

                    }
                }
                else
                {
                    NPC.velocity = Vector2.Zero;
                    if (Main.netMode != NetmodeID.MultiplayerClient && attackCondtion2 == 0)
                    {
                        Projectile tel = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, TargetPosition - NPC.Center, ModContent.ProjectileType<LaserTelegraph>(), 0, 0);
                        tel.timeLeft = 130;
                        miscProperty = tel.whoAmI;
                        attackCondtion2++;
                    }
                }
            }

            if (attackCondtion1 == 1 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 dir = Vector2.Normalize(TargetPosition - NPC.Center);
                    Holder1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (TargetPosition + dir.RotatedBy(MathHelper.PiOver2) * 55) - NPC.Center, ModContent.ProjectileType<Laser>(), 70, 10, ai1: 1);
                    Holder1.timeLeft = 31;
                    Holder2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (TargetPosition - dir.RotatedBy(MathHelper.PiOver2) * 55) - NPC.Center, ModContent.ProjectileType<Laser>(), 70, 10, ai1: 1);
                    Holder2.timeLeft = 31;
                }
                timer = 30;
                attackCondtion1++;
            }

            if (attackCondtion1 == 2 && timer > 0)
            {

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1.velocity = Holder1.velocity.RotatedBy(LaserRot);
                    Holder2.velocity = Holder2.velocity.RotatedBy(-LaserRot);
                }
            }

            if (attackCondtion1 == 2 && timer <= 0)
            {
                timer = 2;
                attackCondtion1 = 3;
            }

            if (attackCondtion1 == 3 && timer <= 0)
            {

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.projectile[miscProperty].Kill();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, TargetPosition - NPC.Center, ModContent.ProjectileType<Laser>(), 70, 10);
                }
                lastAttack = 3;
                currentAttack = -1;
                timer = 25;
                NPC.netUpdate = true;
            }


        }
        float RangeDegrees = MathHelper.ToRadians(70);
        void Tantrum()
        {
            if (attackCondtion1 == 0)
            {
                if (timer > 0)
                {
                    Idle(21, 0.08f);
                    return;
                }

                attackCondtion1 = 1;

            }
            if (attackCondtion1 == 1)
            {
                ProjRotOriginal = GetDirToPlayer().RotatedBy((-RangeDegrees / 2) + MathHelper.ToRadians(Main.rand.Next(16)));
                TargetPosition = ProjRotOriginal;
            }
            float rotateby = RangeDegrees / 6;

            NPC.velocity = Vector2.Zero;

            if (attackCondtion1 < 8)
            {

                if (timer <= 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        telegraphs[attackCondtion1] = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, TargetPosition, ModContent.ProjectileType<LaserTelegraph>(), 0, 0);
                        TargetPosition = TargetPosition.RotatedBy(rotateby);
                    }
                    timer = 3;
                    attackCondtion1++;

                    if (attackCondtion1 >= 8)
                        timer = 20 - (60 - NPC.life * 100 / NPC.lifeMax);
                }

                return;
            }

            if (attackCondtion2 == 0 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (telegraphs[i].active)
                            telegraphs[i].timeLeft = 0;
                    }
                    TargetPosition = ProjRotOriginal;

                    for (int i = 0; i < 7; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, TargetPosition, ModContent.ProjectileType<Laser>(), 70, 10);
                        TargetPosition = TargetPosition.RotatedBy(rotateby);
                    }

                }
                timer = 45;
                currentAttack = -1;
            }


            if (hasHealth(50))
                currentPhase = 1;

        }

        #endregion
        void TransitiomPhase2()
        {

            if (attackCondtion1 == 0)
            {
                for (int i = 0; i < telegraphs.Length; i++)
                {
                    if(telegraphs[i] != null)
                        if (telegraphs[i].whoAmI != -1 && telegraphs[i].whoAmI != 255 && telegraphs[i].active)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                telegraphs[i].Kill();
                        }
                }
                SoundEngine.PlaySound(stun);
                attackCondtion1 = 1;
                timer = 180;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
            }

            Dust dust = Dust.NewDustDirect(NPC.position, 90, 90, DustID.WhiteTorch, Scale: 3);
            dust.noGravity = true;
            dust.velocity *= 12;
            dust = Dust.NewDustDirect(NPC.position, 90, 90, DustID.WhiteTorch, Scale: 3);
            dust.noGravity = true;
            dust.velocity *= 12;

            NPC.rotation = 0;
            if (timer <= 0)
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
                attackOrder.Clear();
                currentPhase = 2;
                currentAttack = -1;
                lastAttack = -1;
            }

        }

        #region phase2
        void CrossLaser()
        {
            if (attackCondtion1 < 4)
            {
                Idle(18);
                if (timer <= 0)
                {
                    Vector2 pos = Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 44;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.InheritSource(NPC), pos, Vector2.Zero, ModContent.ProjectileType<CrossLaserProj>(), 0, 0);
                    timer = 30;
                    attackCondtion1++;
                }
            }
            else
            {
                lastAttack = 6;
                currentAttack = -1;
                timer = 35;
            }
        }

        void SingleLaser()
        {
            Vector2 pos = Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 19;
            Vector2 dir = pos - NPC.Center;

            if (attackCondtion1 < 2)
                Idle(13);

            if (attackCondtion1 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, dir, ModContent.ProjectileType<Assets.LaserTelegraph>(), 0, 0);
                    Holder1.timeLeft = 300;
                }
                attackCondtion1 = 1;
                timer = 51;
            }

            if (attackCondtion1 == 1 && timer > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1.Center = NPC.Center;
                    Holder1.velocity = dir;
                }
            }

            if (attackCondtion1 == 1 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Holder1.velocity = dir;
                TargetPosition = dir;
                timer = 14;
                NPC.velocity = Vector2.Zero;
                attackCondtion1 = 2;
            }


            if (attackCondtion1 == 2 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1.Kill();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, TargetPosition, ModContent.ProjectileType<Laser>(), 100, 10);
                }


                timer = 25;
                lastAttack = 7;
                currentAttack = -1;
            }
        }


        void TpAndShoot()
        {
            Idle(3);
            if (attackCondtion2 == 0)
            {
                TargetPosition = Main.player[NPC.target].Center + new Vector2(Main.player[NPC.target].velocity.X * 15, -500);

                NPC.netUpdate = true;
                attackCondtion2 = 1;
                timer = 50;
            }

            if (attackCondtion2 == 1 && timer > 0)
            {
                Idle(17);
                Dust dust = Dust.NewDustDirect(TargetPosition, 50, 50, DustID.YellowTorch, Scale: 4f);
                dust.noGravity = true;
                dust.velocity *= 5;

            }


            if (attackCondtion2 == 1 && timer <= 0)
            {
                NPC.Teleport(TargetPosition, TeleportationStyleID.TeleportationPotion);

                attackCondtion2 = 2;
                timer = 28;

                TargetPosition = Vector2.UnitX;
                double rotStep = MathHelper.ToRadians(22.5f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    for (int i = 0; i < 16; i++)
                    {

                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + TargetPosition * 3, TargetPosition, ModContent.ProjectileType<BossEnergyBall>(), NPC.damage, 1);
                        TargetPosition = TargetPosition.RotatedBy(rotStep);
                    }
                currentAttack = -1;
                lastAttack = 8;
            }
        }

        void SwordAndSlash()
        {
            if (attackCondtion2 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Holder1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<LaserSword>(), 80, 10);
                miscProperty = 40;
                for (int i = 0; i < 16; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.Center, Holder1.Hitbox.Width, Holder1.Hitbox.Height, DustID.YellowTorch);
                    dust.noGravity = true;
                    dust.scale = 3;
                    dust.velocity *= 8;
                }
                timer = 60;
            }

            if (attackCondtion1 < 6)
            {

                if (timer <= 0)
                {

                    if (attackCondtion2 < 13)
                    {
                        NPC.velocity = LerpVector2(NPC.velocity, TargetPosition * 32, 0.25f);
                        NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Holder1.Center = NPC.Center + Holder1.velocity * 150;
                            if (attackCondtion1 % 2 == 0)
                                Holder1.velocity = Holder1.velocity.RotatedBy(0.08f);

                            if (attackCondtion1 % 2 == 1)
                                Holder1.velocity = Holder1.velocity.RotatedBy(-0.08f);
                        }
                        attackCondtion2++;
                    }
                    else
                    {
                        timer = miscProperty;
                        miscProperty -= 3;
                        attackCondtion1++;
                    }

                }
                else
                {
                    TargetPosition = GetDirToPlayer();
                    Idle(5);

                    attackCondtion2 = 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (attackCondtion1 % 2 == 0)
                            Holder1.velocity = Holder1.velocity.ToRotation().AngleLerp(TargetPosition.ToRotation() - MathHelper.PiOver4 / 1.25f, 0.1f).ToRotationVector2();

                        if (attackCondtion1 % 2 == 1)
                            Holder1.velocity = Holder1.velocity.ToRotation().AngleLerp(TargetPosition.ToRotation() + MathHelper.PiOver4 / 1.25f, 0.1f).ToRotationVector2();

                        Holder1.Center = NPC.Center + Holder1.velocity * 150;
                    }
                }

                return;
            }

            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.Center, Holder1.Hitbox.Width, Holder1.Hitbox.Height, DustID.YellowTorch);
                dust.noGravity = true;
                dust.scale = 3;
                dust.velocity *= 8;
            }
            Holder1.Kill();
            timer = 15;
            currentAttack = -1;
            lastAttack = 9;

        }


        void LasersFromAbove()
        {
            if (attackCondtion1 == 0)
            {
                TargetPosition = Main.player[NPC.target].Center + new Vector2(700 - Main.rand.Next(20), -800);
                timer = 30;
                attackCondtion1 = 1;
            }

            if (attackCondtion1 == 1 && timer > 0)
            {
                Idle(17);
                Dust dust = Dust.NewDustDirect(TargetPosition, 50, 50, DustID.YellowTorch, Scale: 4f);
                dust.noGravity = true;
                dust.velocity *= 5;
            }

            if (attackCondtion1 == 1 && miscProperty == 0)
            {
                miscProperty = 1;
                Vector2 offset = TargetPosition;
                for (int i = 0; i < 16; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        telegraphs[i] = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), offset, Vector2.UnitY, ModContent.ProjectileType<LaserTelegraph>(), 0, 0);
                    offset += new Vector2(-125, 0);

                }
            }

            if (attackCondtion1 == 1 && timer <= 0)
            {
                attackCondtion1 = 2;
                NPC.Teleport(TargetPosition, TeleportationStyleID.TeleportationPotion);
            }
           
            if (attackCondtion1 == 2)
            {
                NPC.velocity = new Vector2(-25, 0);

                if (attackCondtion2 < 16)
                {

                    if (timer <= 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            telegraphs[attackCondtion2].Kill();
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), TargetPosition, Vector2.UnitY, ModContent.ProjectileType<Laser>(), 100, 10);
                            TargetPosition += new Vector2(-125, 0);
                        }
                        timer = 5;
                        attackCondtion2++;

                        if (attackCondtion2 >= 16)
                        {
                            timer = 20;
                            currentAttack = -1;
                            lastAttack = 10;

                        }
                    }
                }
            }
        }
        #endregion

        void TransitionToSky()
        {
            
            if(attackCondtion1 == 0)
            {
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                SoundEngine.PlaySound(stun);
                attackCondtion1 = 1;
                timer = 120;
            }
            if (attackCondtion1 == 1 && timer > 0)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, 90, 90, DustID.WhiteTorch, Scale: 3);
                dust.noGravity = true;
                dust.velocity *= 12;
                dust = Dust.NewDustDirect(NPC.position, 90, 90, DustID.WhiteTorch, Scale: 3);
                dust.noGravity = true;
                dust.velocity *= 12;
            }

            if (attackCondtion1 == 1 && timer <= 0)
            {
                attackCondtion1 = 2;
                timer = 80;
            }

            if(attackCondtion1 == 2  && timer > 0)
            {
                NPC.velocity = LerpVector2(NPC.velocity, Vector2.Normalize(Main.player[NPC.target].position + new Vector2(0, 400) - NPC.position) * 21, 0.08f);
            }

            if(attackCondtion1 == 2 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(20)), ModContent.ProjectileType<Laser>(), 100, 10, ai0: 0);
                    Holder1.timeLeft = 9999;
                    Holder2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (-Vector2.UnitX).RotatedBy(MathHelper.ToRadians(-20)), ModContent.ProjectileType<Laser>(), 100, 10, ai0: 0);
                    Holder2.timeLeft = 9999;
                }
                attackCondtion1 = 3;
            }

            if (attackCondtion1 == 3 && NPC.Center.Y > 4200)
            {
                if (Holder1.ai[0] < 30)
                    Holder1.ai[0] += 0.5f;
                if (Holder2.ai[0] < 30)
                    Holder2.ai[0] += 0.5f;
                float distance = Vector2.Distance(NPC.position, Main.player[NPC.target].position);
                NPC.velocity = LerpVector2(NPC.velocity,new Vector2(GetDirToPlayer().X * (distance / 300), -1) * (distance < 300 ? 32 : 44), 0.02f);

                Holder1.position = NPC.Center;
                Holder2.position = NPC.Center;
          
                Main.LocalPlayer.wingTime = 2;
                Main.LocalPlayer.AddBuff(BuffID.ChaosState, 3, true);
                 
            }
            else if (attackCondtion1 == 3)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Holder1.ai[2] = 0.0333f;
                    Holder2.ai[2] = 0.0333f;
                }
                timer = 25;
                attackCondtion1 = 4;
            }

            if (attackCondtion1 == 4 && timer <= 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LaserHolder>());
                    PlatformPosition = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y - 400, ModContent.NPCType<Phase3Platform>()).position;
                }
               
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
                PlatformPosition = NPC.Center - new Vector2(0, 400);
                currentPhase = 3;
                attackOrder.Clear();
                currentAttack = -1;
                lastAttack = -1;
                NPC.netUpdate = true;
                Main.NewText(Main.LocalPlayer.rocketBoots);
              
            }
        }


        void Sweep()
        {
            if (attackCondtion1 == 0)
            {
                timer = 80;
                attackCondtion1 = 1;
                TargetPosition = PlatformPosition + new Vector2(500, -85);
            }

            if(attackCondtion1 == 1 && timer > 0)
            {
                Idle();
                Dust dust = Dust.NewDustDirect(TargetPosition, 50, 50, DustID.YellowTorch, Scale: 4f);
                dust.noGravity = true;
                dust.velocity *= 5;
            }

            if (attackCondtion1 == 1 && timer <= 0)
            {
                NPC.Teleport(TargetPosition, TeleportationStyleID.TeleportationPotion);
                attackCondtion1 = 2;
                timer = 33;
            }

            if (attackCondtion1 == 2 && timer > 0)
            {
                NPC.velocity = new Vector2(-22, 0);
            }

            if (attackCondtion1 == 2 && timer <= 0)
            {
                currentAttack = -1;
                lastAttack = 12;
                timer = 75;
            }
        }

        void BlockArena()
        {
            int rand = Main.rand.Next(2);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), PlatformPosition + new Vector2(rand == 0 ? 250 : -250, -400), Vector2.Zero,ModContent.ProjectileType<BlockArena>(), 0, 10);
            lastAttack = 13;
            currentAttack = -1;
            timer = 120;
        }
        #region utils
        Vector2 LerpVector2(Vector2 CurrentVector, Vector2 TargetVector, float speed)
        {
            return new Vector2(MathHelper.Lerp(CurrentVector.X, TargetVector.X, speed), MathHelper.Lerp(CurrentVector.Y, TargetVector.Y, speed));
        }

        Vector2 GetDirToPlayer()
        {
            Vector2 dir = Main.player[NPC.target].Center - NPC.Center;
            return Vector2.Normalize(dir);
        }

        bool hasHealth(float health)
        {
            return (float)NPC.life * 100f / (float)NPC.lifeMax <= health;
        }
        #endregion
    }

}
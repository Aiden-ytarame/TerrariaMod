
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using System;
using System.Collections.Generic;
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
    public class LaserHolder : ModNPC
    {
        Projectile Holder1;
        Projectile Holder2;
        public override void SetDefaults()
        {
            NPC.width = 0;
            NPC.height = 0;
            NPC.lifeMax = 4200;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0;
            NPC.npcSlots = 10f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Holder1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitX * 10, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(20)), ModContent.ProjectileType<Laser>(), 999, 10, ai0: 1, ai2: -1);
            Holder2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center - Vector2.UnitX * 10, (-Vector2.UnitX).RotatedBy(MathHelper.ToRadians(-20)), ModContent.ProjectileType<Laser>(), 999, 10, ai0: 1, ai2: -1);
        }
        public override void AI()
        {
            NPC.velocity = Vector2.Zero;
            Holder1.timeLeft = 2;
            Holder2.timeLeft = 2;

            if(Holder1.velocity != Vector2.UnitX)
                Holder1.velocity = Holder1.velocity.ToRotation().AngleLerp(0, 0.1f).ToRotationVector2();

            if (Holder2.velocity != -Vector2.UnitX)
                Holder2.velocity = Holder2.velocity.ToRotation().AngleLerp(MathHelper.Pi, 0.1f).ToRotationVector2();

            if (Holder1.ai[0] < 30)
                Holder1.ai[0] += 0.5f;
            if (Holder2.ai[0] < 30)
                Holder2.ai[0] += 0.5f;

            if (Main.LocalPlayer.position.Y > NPC.position.Y + 100)
            {
                Main.LocalPlayer.Teleport(NPC.Center - new Vector2(0, 500));
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<RobotBoss>())
                    return;
            }

            NPC.life = 0;
        }

    }

    
}


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
    public class Kamikaze : ModNPC
    {

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 20;
            NPC.damage = 0;
            NPC.defense = 22;
            NPC.lifeMax = 4200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = 0;
        }


        public override void OnKill()
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<KamExplosion>(), 100, 10);
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            if (NPC.localAI[0] > 20)
            {
                Vector2 dir = Main.player[NPC.target].Center - NPC.Center;
                dir = Vector2.Normalize(dir);
                NPC.velocity = LerpVector2(NPC.velocity, dir * 16, 0.08f);
            }
            else
            {
                NPC.localAI[0] += 1;
                NPC.velocity = LerpVector2(NPC.velocity, Vector2.Zero, 0.02f);
            }

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            foreach (Player player in Main.player)
            {
                bool hit = Collision.CheckAABBvAABBCollision(NPC.Hitbox.TopLeft(), NPC.Hitbox.Size(), player.Hitbox.TopLeft(), player.Hitbox.Size());

                if(hit)
                {
                    NPC.life = 0;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<KamExplosion>(), 100, 10);
                    return;
                }

            }
        }

        Vector2 LerpVector2(Vector2 CurrentVector, Vector2 TargetVector, float speed)
        {
            return new Vector2(MathHelper.Lerp(CurrentVector.X, TargetVector.X, speed), MathHelper.Lerp(CurrentVector.Y, TargetVector.Y, speed));
        }
    }
}

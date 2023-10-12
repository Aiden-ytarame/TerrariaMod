using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using test.Buffs;

namespace test.Projectiles
{
	public class ShockProjectile : ModProjectile
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 6;
			Projectile.height = 8;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 600;
			Projectile.light = 0.3f;
			Projectile.ignoreWater = true;

		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
				target.AddBuff(ModContent.BuffType<Paralized>(), 60);

        }
        NPC target = null;
        public override void AI()
        {
			if (target == null || !target.active) 
			{
				float distance = 1000f;
				for (int i = 0; i < 200; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.active && !npc.dontTakeDamage && !npc.friendly && npc.lifeMax > 5)
					{
						Vector2 newMove = npc.Center - Projectile.Center;
						float newDistance = newMove.Length();
						if (distance > newDistance)
						{
							distance = newDistance;
							target = npc;
						}
					}

				}

			}
			else
			{
				Vector2 newVelocity = target.Center - Projectile.Center;
				Projectile.velocity += Vector2.Normalize(newVelocity);
				float mag = Projectile.velocity.Length();

				if (mag > 12)
					Projectile.velocity *= 12f / mag;
            }

			int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Electric);
			Main.dust[dust].noGravity = true;
        }
    }
}
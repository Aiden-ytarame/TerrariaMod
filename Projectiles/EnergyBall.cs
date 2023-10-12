using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace test.Projectiles
{
	public class EnergyBall : ModProjectile
	{
		// The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.damage = 26;
			Projectile.width = 6;
			Projectile.height = 8;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 600;
			Projectile.light = 0.3f;
			Projectile.ignoreWater = true;
		
		}

        public override void AI()
        {

			int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.RedTorch, Scale: 2.5f);
			Main.dust[dust].noGravity = true;
        }

    }
}
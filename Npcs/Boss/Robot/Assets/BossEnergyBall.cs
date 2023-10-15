using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace test.Npcs.Boss.Robot.Assets
{
	public class BossEnergyBall : ModProjectile
	{
		// The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.damage = 26;
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.scale = 1.2f;
			Projectile.aiStyle = -1; ;
			Projectile.hostile = true;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 300;
			Projectile.light = 0.3f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;

        }

        public override void AI()
        {
			int dust = Dust.NewDust(Projectile.position, 20, 20, DustID.YellowTorch, Scale: 2.5f);
			Main.dust[dust].noGravity = true;

			Projectile.velocity *= 1.02f;
        }

    }
}
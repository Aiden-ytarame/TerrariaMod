using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace test.Npcs.Boss.Robot.Assets
{
	public class BlockArena : ModProjectile
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.
        public override string Texture => "Terraria/Images/Projectile_650";

		public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1100;
        }
        public override void SetDefaults()
		{
			Projectile.hide = true;
			Projectile.damage = 0;
			Projectile.width = 500;
			Projectile.height = 800;
			Projectile.aiStyle = -1; ;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;

        }

        public override void AI()
        {
			if (Projectile.timeLeft < 480 && Projectile.damage == 0)
				Projectile.damage = 40;

			Dust dust = Dust.NewDustDirect(Projectile.Hitbox.TopLeft(), Projectile.width, Projectile.height, DustID.CorruptTorch);
			dust.noGravity = true;
			dust.scale = 3;
            dust = Dust.NewDustDirect(Projectile.Hitbox.TopLeft(), Projectile.width, Projectile.height, DustID.CorruptTorch);
            dust.noGravity = true;
            dust.scale = 3;
        }
    }
}
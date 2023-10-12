using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using test.Buffs;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace test.Npcs.Boss.Robot
{
	public class KamExplosion : ModProjectile
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Projectile.damage = 100;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 30;
			Projectile.ignoreWater = true;
			Projectile.hide = true;
		}

     
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
			return Collision.CheckAABBvAABBCollision(projHitbox.TopLeft(), projHitbox.Size(), targetHitbox.TopLeft(), targetHitbox.Size());
        }

        public override string Texture => "Terraria/Images/Projectile_650";
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 15; i++)
			{
				int dust = Dust.NewDust(Projectile.Hitbox.TopLeft(), 5, 5, DustID.Smoke, Scale: 4);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].velocity *= Main.rand.NextFloat(3);
            }
        }

        public override bool ShouldUpdatePosition() => false;
      
    }
}
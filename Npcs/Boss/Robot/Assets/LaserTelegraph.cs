using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using test.Buffs;
using Microsoft.Xna.Framework.Graphics;

namespace test.Npcs.Boss.Robot.Assets
{
	public class LaserTelegraph : ModProjectile
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.
        Texture2D tex;


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
		{
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 140;
            Projectile.tileCollide = false;
            tex = ModContent.Request<Texture2D>(Texture).Value;
            Projectile.ai[0] = 10;
        }

        public override string Texture => "test/Npcs/Boss/Robot/Assets/Laser";
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = 10;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(122, 122, 0, 1), Projectile.velocity.ToRotation(), new Vector2(0,1), new Vector2(3000, Projectile.ai[0]), 0, 0);
     
            return false;   
        }
        public override bool ShouldUpdatePosition() => false;
      
    }
}
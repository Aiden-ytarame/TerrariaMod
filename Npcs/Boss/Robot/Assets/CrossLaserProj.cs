using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using test.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;


namespace test.Npcs.Boss.Robot.Assets
{
	public class CrossLaserProj : ModProjectile
	{
        public override void SetStaticDefaults()
        {
           // ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
		{
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 45;
            Projectile.hide = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override string Texture => "Terraria/Images/Projectile_650";

        public override void OnSpawn(IEntitySource source)
        {
            DustLine(Projectile.Center + new Vector2(0, 2800), Projectile.Center - new Vector2(0, 2800), 300, Color.YellowGreen);
            DustLine(Projectile.Center + new Vector2(2800, 0), Projectile.Center - new Vector2(2800, 0), 300, Color.YellowGreen);
           
        }

        //the scale on the default QuickLineDust is too small
        void DustLine(Vector2 start, Vector2 end, float splits, Color color)
        {
            Dust.QuickDust(start, color).scale = 2f;
            Dust.QuickDust(end, color).scale = 2f;
            float num = 1f / splits;
            for (float num2 = 0f; num2 < 1f; num2 += num)
            {
                Dust.QuickDust(Vector2.Lerp(start, end, num2), color).scale = 2;
            }
        }
        public override void AI()    
        {
            //Dust.QuickDustLine(Projectile.Center + new Vector2(0,1000), Projectile.Center - new Vector2(0,1000), 1500, Color.YellowGreen);
           // Dust.QuickDustLine(Projectile.Center + new Vector2(1000, 0), Projectile.Center - new Vector2(1000, 0), 1500, Color.YellowGreen);

            if(Projectile.timeLeft == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<CrossExplosion>(), 100, 20);
            } 
        }
     
        public override bool ShouldUpdatePosition() => false;
      
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using test.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace test.Npcs.Boss.Robot.Assets
{
	public class Laser : ModProjectile
	{
        Texture2D tex;
        float decreaseOT = 0;
        public float size = 10;
        List<Vector2> oldVelocity = new List<Vector2>();
        List<Vector2> oldV;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
		{
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.damage = 100;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            decreaseOT = 1f / Projectile.timeLeft;
            tex = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        float widthMult = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = 10;

            if (Projectile.localAI[0] == 0 && Projectile.ai[1] > 0)
            {
                decreaseOT = 1f / Projectile.timeLeft;
                Projectile.localAI[0] = Projectile.timeLeft;
                Projectile.timeLeft = (int)((float)Projectile.timeLeft * 1.5f);
                
            }
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(tex, pos, null, new Color(255, 255, 0), Projectile.velocity.ToRotation(), new Vector2(0, 1), new Vector2(3000, Projectile.ai[0] * widthMult), 0, 0);
            
            float afterImgScale = widthMult;
            if(!Main.gamePaused || !Main.autoPause)
                widthMult -= decreaseOT;

            if (Projectile.ai[1] > 0)
            {
                oldV = new List<Vector2>(oldVelocity);
                oldV.Reverse();
                Color color;
                for (int i = 0; i < oldV.Count; i++)
                {

                    color = new Color(130, 130, 0, 1) * (((float)(oldV.Count - i) / (float)oldV.Count) - 0.4f);
                    Main.EntitySpriteDraw(tex, pos, null, color, oldV[i].ToRotation(), new Vector2(0, 1), new Vector2(3000, Projectile.ai[0] * afterImgScale), 0, 0);
                    afterImgScale += decreaseOT;
                }
                if (!Main.gamePaused || !Main.autoPause)
                    oldVelocity.Add(Projectile.velocity);
            }
            //cast light
            DelegateMethods.v3_1 = new Vector3(1f, 1f, 0f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 5000, 26, DelegateMethods.CastLight);

            return false;   
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if(Projectile.localAI[0] != 0)
            {
                if (Projectile.localAI[0] >= Projectile.timeLeft)
                    return false;
            }
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + unit * 3000, 20, ref point);
        }
        public override bool ShouldUpdatePosition() => false;
      
    }
}
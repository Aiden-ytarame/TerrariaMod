using CollisionLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

namespace test.Npcs.Boss.Robot
{

    public class Phase3Platform : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Platform");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public CollisionSurface[] colliders = null;
        public override void SetDefaults()
        {
            NPC.scale = 3;
            NPC.width = 264;
            NPC.height = 2;
            NPC.lifeMax = 1;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.alpha = 255;
        }
        public override bool CheckActive() => true;
        public override bool PreAI()
        {
            if (colliders == null || colliders.Length != 1)
            {
                NPC.velocity = Vector2.Zero;
                colliders = new CollisionSurface[] {
                    new CollisionSurface(NPC.TopLeft, NPC.TopRight, new int[] { 2, 0, 0, 0 }, true) };
            }
            return true;
        }
        public override void AI()
        {
            if (colliders != null && colliders.Length == 1)
            {
                colliders[0].Update();
                colliders[0].endPoints[0] = NPC.Center + (NPC.TopLeft - NPC.Center).RotatedBy(NPC.rotation);
                colliders[0].endPoints[1] = NPC.Center + (NPC.TopRight - NPC.Center).RotatedBy(NPC.rotation);
            }
 
            if (NPC.alpha > 0)
                NPC.alpha--;

            foreach(NPC npc in Main.npc)
            {
                if (npc.active && npc.type == ModContent.NPCType<RobotBoss>())
                    return;
            }

            NPC.life = 0;
        }
        public override void PostAI()
        {
            if (colliders != null)
            {
                foreach (CollisionSurface collider in colliders)
                {
                    collider.PostUpdate();
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.EntitySpriteDraw(texture, NPC.Center - new Vector2(0, -2 + (NPC.velocity.Y * 2)) - screenPos, null, NPC.GetAlpha(Color.White), NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}

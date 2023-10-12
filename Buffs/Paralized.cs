
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace test.Buffs
{

    public class Paralized : ModBuff
	{
        public override void SetStaticDefaults()
        {
           
            Main.debuff[ModContent.BuffType<Paralized>()] = true;
        }
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.
        Vector2 lastVelocity = Vector2.Zero;
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.noGravity)
                npc.velocity = Vector2.Zero;

            else
                npc.velocity *= new Vector2(0,1);     
        }

    }
}
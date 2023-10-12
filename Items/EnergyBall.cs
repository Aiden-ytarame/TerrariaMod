using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;

namespace test.Items
{

	public class testing : GlobalNPC
	{

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
			if(entity.type == NPCID.DungeonGuardian)
				return true;

			return false;
        }

        public override bool CheckActive(NPC npc)
        {
			return false; 
        }
   
        public override bool PreAI(NPC npc)
        {
			Vector2 dir = Main.LocalPlayer.Center - npc.Center;
		
			npc.velocity = Vector2.Normalize(dir) * 4;
			npc.rotation += 0.1f;
			return false;
			
        }
    }

	public class EnergyBall : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 26;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 30;
			Item.height = 30;
			Item.value = 100;
			Item.rare = 2;
			Item.consumable = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.EnergyBall>();
			Item.ammo = Item.type;
			Item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);
			recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
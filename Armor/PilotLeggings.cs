using Microsoft.Xna.Framework;
using rail;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;
using test.Items;

namespace test.Armor
{
  
    [AutoloadEquip(EquipType.Legs)]
	public class PilotLeggings : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.
		
		public override void SetDefaults()
		{	
			Item.defense = 12;
			Item.value = 800000;
			Item.rare = ItemRarityID.Purple;
		}
      
        public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Ranged) += 0.04f;
			player.moveSpeed += 0.17f;
			player.GetCritChance(DamageClass.Generic) += 0.09f;
        }

 
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup("TitaniumBar", 9);
			recipe.AddIngredient(ItemID.SoulofSight, 2);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}

}
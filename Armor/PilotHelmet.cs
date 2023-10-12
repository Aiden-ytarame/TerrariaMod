using rail;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;
using test.Items;

namespace test.Armor
{

	[AutoloadEquip(EquipType.Head)]
	public class PilotHelmet : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.
		
		public override void SetDefaults()
		{	
			Item.defense = 15;
			Item.value = 800000;
			Item.rare = ItemRarityID.Purple;
		}
      
        public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Ranged) += 0.12f;


        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup("TitaniumBar", 12);
			recipe.AddIngredient(ItemID.LightShard, 2);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
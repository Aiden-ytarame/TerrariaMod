using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace test.Items
{
	public class test : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 10000;
			//Item.DamageType = DamageClass.Melee;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 1;
			Item.width = 100;
			Item.height = 100;
			Item.useTime = 1;
			Item.useAnimation = 1;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
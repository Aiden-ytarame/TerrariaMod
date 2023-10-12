using log4net.Repository.Hierarchy;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace test.Items
{
	public class VineBow : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.


        public override void SetDefaults()
		{
			Item.damage = 14;
			
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = 1;
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 7f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Vine, 5);
			recipe.AddIngredient(ItemID.Wood, 10);
            recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
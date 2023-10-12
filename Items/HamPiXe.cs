using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace test.Items
{



    public class HamPiXe : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Melee;
			Item.axe = 15;
			Item.pick = 70;
			Item.hammer = 60;
			Item.width = 50;
			Item.height = 50;
			Item.useTime = 10;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.useTurn = true;
			Item.scale = 1.3f;
		}

		public override void AddRecipes()
		{
	
			Recipe recipe = CreateRecipe();

            recipe.AddRecipeGroup("Wood", 5);
            recipe.AddRecipeGroup("IronBar", 5);
			recipe.AddRecipeGroup("CrimtaneBar", 5);

            recipe.AddTile(TileID.Anvils);
     
            recipe.Register();
        }
	}
}
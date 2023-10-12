using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using test.Projectiles;
using test.Buffs;
using test.Armor;

namespace test.Items
{
	public class TeslaStaff : ModItem
	{
		int timer = 0;
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 14;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			//Item.width = 40;
			//Item.height = 40;
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.shoot = ModContent.ProjectileType<ShockProjectile>();
			Item.shootSpeed = 8;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.noMelee = true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			position += velocity * 4f;
        }
		
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("CrimtaneBar", 5);
            recipe.AddIngredient(ItemID.Wire, 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
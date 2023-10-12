using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using rail;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using test.Projectiles;
namespace test.Items
{
	public class LStar : ModItem
	{ 
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 49;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 10;
			Item.useAnimation = 10;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = 1;
			Item.useAmmo = ModContent.ItemType<EnergyBall>();
			Item.shootSpeed = 8;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.scale = 0.65f;
		}
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-12,0);

        }
   
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position += velocity * 5f;
            float rotation = MathHelper.ToRadians(Main.rand.NextFloat(-9,9));  		
            Vector2 perturbedSpeed = velocity.RotatedBy(rotation); // Watch out for dividing by 0 if there is only 1 projectile.

            Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<Projectiles.EnergyBall>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("TitaniumBar", 25);
            recipe.AddIngredient(ItemID.SpaceGun, 10);
			recipe.AddIngredient(ModContent.ItemType<EnergyBall>(), 50);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddIngredient(ItemID.IllegalGunParts, 1);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
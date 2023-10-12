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
	public class PlasmaCutter : ModItem
	{ 
        // The Display Name and Tooltip of this Item can be edited in the Localization/en-US_Mods.test.hjson file.

		public override void SetDefaults()
		{
            Item.damage = 140;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
           // Item.channel = true; //Channel so that you can held the weapon [Important]
            Item.mana = 5;
            Item.scale = 0.8f;
            Item.rare = ItemRarityID.Pink;
            Item.width = 28;
            Item.height = 30;
            Item.UseSound = SoundID.Item13;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<PlasmaLaser>();
            Item.value = Item.sellPrice(silver: 3);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(6,0);

        }
     
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rot = MathHelper.ToRadians(90);
  
            Projectile.NewProjectile(source, position + velocity.RotatedBy(rot) * 9, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position + velocity.RotatedBy(-rot) * 9, velocity, type, damage, knockback, player.whoAmI);
            return true;
        }


        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("TitaniumBar", 25);
            recipe.AddIngredient(ItemID.SpaceGun, 10);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddIngredient(ItemID.IllegalGunParts, 1);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
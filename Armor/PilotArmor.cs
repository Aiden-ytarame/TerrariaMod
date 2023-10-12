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
  
    [AutoloadEquip(EquipType.Body)]
	public class PilotArmor : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.test.hjson file.
		
		public override void SetDefaults()
		{	
			Item.defense = 21;
			Item.value = 800000;
			Item.rare = ItemRarityID.Purple;
		}
      
        public override void UpdateEquip(Player player)
		{
            player.ammoCost75 = true;
			player.GetDamage(DamageClass.Ranged) += 0.04f;
     
        }

	
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
			if (head.type == ModContent.ItemType<PilotHelmet>() && legs.type == ModContent.ItemType<PilotLeggings>())
				return true;

			return false;
        }

        bool hasJumped = false;
        public override void UpdateArmorSet(Player player)
        {
			player.setBonus = "Increases ranged damage by 7% \nRemember yo use your jump kit! \nWhen your wings get tired, JUMP to do an air jump and increase your air speed until you recharge wings back. \nEffect also happens after dismounting mid-air";

            player.GetDamage(DamageClass.Ranged) += 0.07f;

            //check if player is grouded to recharge the jump;
            if (player.velocity.Y == 0 || player.sliding)
                hasJumped = false;

            //check if can use the jump;
            if (player.wingTime != 0 || player.wingTimeMax == 0 || player.mount.Active)
                return;

            player.GetJumpState<DoubleJump>().Enable();

            if (!hasJumped)
            {
                player.GetJumpState<DoubleJump>().Available = true;
                hasJumped = true;
            }
            else
            {
                if (player.GetJumpState<DoubleJump>().Available == true)
                    return;

                player.moveSpeed += 1.88f;
                SpawnParticles(player);
            }

     
        }

        void SpawnParticles(Player player)
        {
            int offsetY = player.height - 15;
            if (player.gravDir == -1f)
                offsetY = 15;

            Vector2 spawnPos = new Vector2(player.position.X, player.position.Y + offsetY);

            Dust dust = Dust.NewDustDirect(spawnPos, player.width, 12, DustID.BlueTorch, player.velocity.X * 0.3f, player.velocity.Y * 0.3f, newColor: Color.Cyan);
            dust.fadeIn = 1.5f;
    
            dust.noGravity = true;
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddRecipeGroup("TitaniumBar", 25);
			recipe.AddIngredient(ItemID.SoulofFlight, 2);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}


    public class DoubleJump : ExtraJump
    {
        public override Position GetDefaultPosition() => AfterBottleJumps;


        public override float GetDurationMultiplier(Player player)
        {
            return 1f;
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.maxRunSpeed *= 1.3f;
            player.runAcceleration *= 1.5f;
        }
        public override void OnStarted(Player player, ref bool playSound)
        {

            int offsetY = player.height;
            if (player.gravDir == -1f)
                offsetY = 0;

            offsetY -= 16;

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position + new Vector2(-34f, offsetY), 102, 32, DustID.BlueTorch, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 100, Color.Cyan, 2.5f);
                dust.velocity = dust.velocity * 0.5f - player.velocity * new Vector2(0.1f, 0.3f);
            }

  
        }



    }

}
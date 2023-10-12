using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using test.Items;

namespace test
{
	public class test : Mod
    {
        public bool testerrrr = false;
    }

    public class mainSystem : ModSystem
    {
        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => "Any Evil Biome Bar", new int[]
            {
                ItemID.CrimtaneBar, ItemID.DemoniteBar
            });

            RecipeGroup.RegisterGroup("CrimtaneBar", group);

            RecipeGroup HMOres3 = new RecipeGroup(() => "Any Titanium Bar", new int[]
           {
                ItemID.TitaniumBar, ItemID.AdamantiteBar
           });

            RecipeGroup.RegisterGroup("TitaniumBar", group);
        }
    }


    public class LuckPlayer : ModPlayer
    { 
    
    
    
    }


}
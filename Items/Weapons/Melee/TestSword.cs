using Terraria.ID;
using Terraria.ModLoader;

namespace iridium.Items.Weapons.Melee {
    class TestSword : ModItem {
        public override void SetStaticDefaults() {
            Tooltip.SetDefault("void sword");
        }

        public override void SetDefaults() {
            item.melee = true;
            item.damage = 20;
            item.width = 40;
            item.height = 40;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 100;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
        }
    }
}
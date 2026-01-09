using System.Drawing;

namespace NHSE.Core
{
    /// <summary>
    /// 提供获取物品颜色的功能
    /// </summary>
    public static class ItemColor
    {
        /// <summary>
        /// 根据物品对象获取对应的颜色
        /// </summary>
        /// <param name="item">物品对象</param>
        /// <returns>物品对应的颜色</returns>
        public static Color GetItemColor(Item item)
        {
            if (item.ItemId == Item.NONE)
                return Color.Transparent;
            var kind = ItemInfo.GetItemKind(item);
            if (kind == ItemKind.Unknown)
                return Color.LimeGreen;
            return ColorUtil.GetColor((int)kind);
        }

        /// <summary>
        /// 根据物品ID获取对应的颜色
        /// </summary>
        /// <param name="item">物品ID</param>
        /// <returns>物品对应的颜色</returns>
        public static Color GetItemColor(ushort item)
        {
            if (item == Item.NONE)
                return Color.Transparent;
            var kind = ItemInfo.GetItemKind(item);
            if (kind == ItemKind.Unknown)
                return Color.LimeGreen;
            return ColorUtil.GetColor((int)kind);
        }
    }
}

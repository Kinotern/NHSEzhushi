using System.Drawing;

namespace NHSE.Core
{
    /// <summary>
    /// 提供获取岛屿区域(Acre)瓷砖颜色的功能
    /// </summary>
    public static class AcreTileColor
    {
        /// <summary>
        /// 从资源文件加载的岛屿区域瓷砖数据
        /// </summary>
        public static readonly byte[] AcreTiles = ResourceUtil.GetBinaryResource("outside.bin");

        /// <summary>
        /// 获取指定岛屿区域(Acre)中指定坐标的瓷砖颜色
        /// </summary>
        /// <param name="acre">岛屿区域(Acre)的ID</param>
        /// <param name="x">X坐标 (0-63)</param>
        /// <param name="y">Y坐标 (0-63)</param>
        /// <returns>瓷砖颜色的ARGB值</returns>
        public static int GetAcreTileColor(ushort acre, int x, int y)
        {
            // 检查acre值是否有效
            if (acre > (ushort)OutsideAcre.FldOutNGardenRFront00)
                return Color.Transparent.ToArgb();
            
            // 计算基础偏移量：每个acre占用32x32的瓷砖，每个瓷砖4字节
            var baseOfs = acre * 32 * 32 * 4;

            // 计算具体瓷砖的偏移量：64x64的网格
            var shift = (4 * ((y * 64) + x));
            var ofs = baseOfs + shift;
            
            // 获取瓷砖类型并返回对应的颜色
            var tile = AcreTiles[ofs];
            return CollisionUtil.Dict[tile].ToArgb();
        }
    }
}

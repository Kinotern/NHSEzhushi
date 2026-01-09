using System;
using System.Drawing;
using static NHSE.Core.TerrainUnitModel;
using static NHSE.Core.LandAngles;

namespace NHSE.Core
{
    /// <summary>
    /// 提供获取地形 tiles 颜色的功能
    /// </summary>
    public static class TerrainTileColor
    {
        /// <summary>
        /// 河流颜色
        /// </summary>
        private static readonly Color River = Color.FromArgb(128, 215, 195);
        /// <summary>
        /// 草地颜色
        /// </summary>
        private static readonly Color Grass = Color.ForestGreen;

        /// <summary>
        /// 获取地形 tile 的颜色
        /// </summary>
        /// <param name="tile">地形 tile 对象</param>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>地形 tile 对应的颜色</returns>
        public static Color GetTileColor(TerrainTile tile, int relativeX, int relativeY)
        {
            if (tile.UnitModelRoad.IsRoad())
                return GetRoadColor(tile.UnitModelRoad);
            var baseColor = GetTileDefaultColor(tile.UnitModel, tile.LandMakingAngle, relativeX, relativeY);
            if (tile.Elevation == 0)
                return baseColor;

            return ColorUtil.Blend(baseColor, Color.White, 1.4d / (tile.Elevation + 1));
        }

        /// <summary>
        /// 获取道路颜色
        /// </summary>
        /// <param name="mdl">地形单元模型</param>
        /// <returns>道路对应的颜色</returns>
        private static Color GetRoadColor(TerrainUnitModel mdl)
        {
            if (mdl.IsRoadBrick())
                return Color.Firebrick;
            if (mdl.IsRoadDarkSoil())
                return Color.SaddleBrown;
            if (mdl.IsRoadSoil())
                return Color.Peru;
            if (mdl.IsRoadStone())
                return Color.DarkGray;
            if (mdl.IsRoadPattern())
                return Color.Ivory;
            if (mdl.IsRoadTile())
                return Color.SteelBlue;
            if (mdl.IsRoadSand())
                return Color.SandyBrown;
            return Color.BurlyWood;
        }

        /// <summary>
        /// 获取河流颜色
        /// 注意：河流名称中的数字表示有多少边/对角线是水
        /// </summary>
        /// <param name="mdl">地形单元模型</param>
        /// <param name="landAngle">土地角度</param>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>河流对应的颜色</returns>
        private static Color GetRiverColor(TerrainUnitModel mdl, LandAngles landAngle, int relativeX, int relativeY)
        {
            return mdl switch
            {
                // River0A 单个水"洞"，四周都是陆地。旋转无效果
                River0A when (relativeX < 4 || relativeX >= 12 || relativeY < 4 || relativeY >= 12) =>
                    Grass,
                // River1A 狭窄通道，底部开口，其他三面是陆地
                River1A => landAngle switch
                {
                    Default when relativeX < 4 || relativeX >= 12 || relativeY < 4 => Grass,
                    Rotate90ClockAnverse when relativeX < 4 || relativeY < 4 || relativeY >= 12 => Grass,
                    Rotate180ClockAnverse when relativeX < 4 || relativeX >= 12 || relativeY >= 12 => Grass,
                    Rotate270ClockAnverse when relativeY < 4 || relativeY >= 12 || relativeX >= 12 => Grass,
                    _ => River
                },
                // River2A 狭窄水道，上下开口，左右是陆地
                River2A => landAngle switch
                {
                    Default when relativeX is < 4 or >= 12 => Grass,
                    Rotate90ClockAnverse when relativeY is >= 12 or < 4 => Grass,
                    Rotate180ClockAnverse when relativeX is < 4 or >= 12 => Grass,
                    Rotate270ClockAnverse when relativeY is < 4 or >= 12 => Grass,
                    _ => River
                },
                // River2B 45度角狭窄通道，左上角是陆地，右下角有小块陆地
                River2B => landAngle switch
                {
                    Default when IsPointInMultiTriangle(relativeX, relativeY, new(4, 15), new(0, 0), new(15, 4), new(0, 15), new(15, 0)) || IsNubOnBottomRight(relativeX, relativeY) || relativeX < 4 || relativeY < 4 => Grass,
                    Rotate90ClockAnverse when IsPointInMultiTriangle(relativeX, relativeY, new(4, 0), new(0, 15), new(15, 12), new(0, 0), new(15, 15)) || IsNubOnTopRight(relativeX, relativeY) || relativeX < 4 || relativeY >= 12 => Grass,
                    Rotate180ClockAnverse when IsPointInMultiTriangle(relativeX, relativeY, new(0, 12), new(15, 15), new(12, 0), new(0, 15), new(15, 0)) || IsNubOnTopLeft(relativeX, relativeY) || relativeX >= 12 || relativeY >= 12 => Grass, 
                    Rotate270ClockAnverse when IsPointInMultiTriangle(relativeX, relativeY, new(0, 4), new(15, 0), new(12, 15), new(0, 0), new(15, 15)) || IsNubOnBottomLeft(relativeX, relativeY) || relativeX >= 12 || relativeY < 4 => Grass,
                    _ => River
                },
                // River2C 90度角狭窄通道，左上角是陆地，右下角有小块陆地
                River2C => landAngle switch
                {
                    Default when relativeX < 4 || relativeY < 4 || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when relativeX < 4 || relativeY >= 12 || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when relativeX >= 12 || relativeY >= 12 || IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when relativeX >= 12 || relativeY < 4 || IsNubOnBottomLeft(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River3A 三向狭窄通道，左侧是陆地，右上角和右下角有小块陆地
                River3A => landAngle switch
                {
                    Default when relativeX < 4 || IsNubOnTopRight(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when relativeY >= 12 || IsNubOnTopLeft(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when relativeX >= 12 || IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when relativeY < 4 || IsNubOnBottomRight(relativeX, relativeY) || IsNubOnBottomLeft(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River3B 45度角河流拐角，左上角是陆地，无小块陆地
                River3B => landAngle switch
                {
                    Default when IsPointInMultiTriangle(relativeX, relativeY, new(4, 15), new(0, 0), new(15, 4), new(0, 15), new(15, 0)) => Grass,
                    Rotate90ClockAnverse when IsPointInMultiTriangle(relativeX, relativeY, new(4, 0), new(0, 15), new(15, 12), new(0, 0), new(15, 15)) => Grass,
                    Rotate180ClockAnverse when IsPointInMultiTriangle(relativeX, relativeY, new(0, 12), new(15, 15), new(12, 0), new(0, 15), new(15, 0)) => Grass,
                    Rotate270ClockAnverse when IsPointInMultiTriangle(relativeX, relativeY, new(0, 4), new(15, 0), new(12, 15), new(0, 0), new(15, 15)) => Grass,
                    _ => River
                },
                // River3C 90度角河流拐角，左上角是陆地，无小块陆地
                River3C => landAngle switch
                {
                    Default when relativeX < 4 || relativeY < 4 => Grass,
                    Rotate90ClockAnverse when relativeX < 4 || relativeY >= 12 => Grass,
                    Rotate180ClockAnverse when relativeX >= 12 || relativeY >= 12 => Grass,
                    Rotate270ClockAnverse when relativeX >= 12 || relativeY < 4 => Grass,
                    _ => River
                },
                // River4A 河流侧边，顶部有小块陆地，左侧是陆地，只有右上角有小块陆地
                River4A => landAngle switch
                {
                    Default when relativeX < 4 || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when relativeY >= 12 || IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when relativeX >= 12 || IsNubOnBottomLeft(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when relativeY < 4 || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River4B 河流侧边，底部有小块陆地，左侧是陆地，只有右下角有小块陆地
                River4B => landAngle switch
                {
                    Default when relativeX < 4 || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when relativeY >= 12 || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when relativeX >= 12 || IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when relativeY < 4 || IsNubOnBottomLeft(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River4C 四向狭窄通道，四个角落都有小块陆地，四边都是水。旋转无效果
                River4C when (IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) || IsNubOnTopLeft(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY)) => Grass,
                // River5A 河流拐角连接两个狭窄通道，左上角、右上角和右下角有小块陆地。两个狭窄通道汇入河流
                River5A => landAngle switch
                {
                    Default when IsNubOnTopLeft(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnTopLeft(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) || IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River5B 河流侧边，左侧是陆地
                River5B => landAngle switch
                {
                    Default when relativeX < 4 => Grass,
                    Rotate90ClockAnverse when relativeY >= 12 => Grass,
                    Rotate180ClockAnverse when relativeX >= 12 => Grass,
                    Rotate270ClockAnverse when relativeY < 4 => Grass,
                    _ => River
                },
                // River6A 河流有两个相对的小块陆地，左上角和右下角
                River6A => landAngle switch
                {
                    Default when IsNubOnTopLeft(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when IsNubOnTopLeft(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River6B 河流同一侧有两个小块陆地，左下角和右下角，一个狭窄通道在底部汇入河流
                River6B => landAngle switch
                {
                    Default when IsNubOnBottomLeft(relativeX, relativeY) || IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when IsNubOnBottomRight(relativeX, relativeY) || IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when IsNubOnTopRight(relativeX, relativeY) || IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when IsNubOnTopLeft(relativeX, relativeY) || IsNubOnBottomLeft(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River7A 河流有一个小块陆地，左下角，填充对角线河岸的间隙
                River7A => landAngle switch
                {
                    Default when IsNubOnBottomLeft(relativeX, relativeY) => Grass,
                    Rotate90ClockAnverse when IsNubOnBottomRight(relativeX, relativeY) => Grass,
                    Rotate180ClockAnverse when IsNubOnTopRight(relativeX, relativeY) => Grass,
                    Rotate270ClockAnverse when IsNubOnTopLeft(relativeX, relativeY) => Grass,
                    _ => River
                },
                // River8A 河流无陆地，全是水。旋转无关紧要
                River8A => River,
                _ => River
            };
        }

        /// <summary>
        /// 检查点是否在左上角小块陆地内
        /// </summary>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>如果点在左上角小块陆地内返回 true，否则返回 false</returns>
        private static bool IsNubOnTopLeft(int relativeX, int relativeY) => IsPointInTriangle(relativeX, relativeY, new(0, 4), new(0, 0), new(4, 0));
        /// <summary>
        /// 检查点是否在右上角小块陆地内
        /// </summary>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>如果点在右上角小块陆地内返回 true，否则返回 false</returns>
        private static bool IsNubOnTopRight(int relativeX, int relativeY) => IsPointInTriangle(relativeX, relativeY, new(12, 0), new(15, 0), new(15, 4));
        /// <summary>
        /// 检查点是否在左下角小块陆地内
        /// </summary>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>如果点在左下角小块陆地内返回 true，否则返回 false</returns>
        private static bool IsNubOnBottomLeft(int relativeX, int relativeY) => IsPointInTriangle(relativeX, relativeY, new(0, 12), new(0, 15), new(4, 15));
        /// <summary>
        /// 检查点是否在右下角小块陆地内
        /// </summary>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>如果点在右下角小块陆地内返回 true，否则返回 false</returns>
        private static bool IsNubOnBottomRight(int relativeX, int relativeY) => IsPointInTriangle(relativeX, relativeY, new(12, 15), new(15, 15), new(15, 12));

        /// <summary>
        /// 检查点是否在多个三角形内
        /// </summary>
        /// <param name="px">点的 X 坐标</param>
        /// <param name="py">点的 Y 坐标</param>
        /// <param name="a">三角形顶点 A</param>
        /// <param name="b">三角形顶点 B</param>
        /// <param name="c">三角形顶点 C</param>
        /// <param name="vortexA">漩涡点 A</param>
        /// <param name="vortexB">漩涡点 B</param>
        /// <returns>如果点在任何一个三角形内返回 true，否则返回 false</returns>
        private static bool IsPointInMultiTriangle(int px, int py, Coordinate a, Coordinate b, Coordinate c, Coordinate vortexA, Coordinate vortexB)
        {
            return IsPointInTriangle(px, py, a, vortexA, b)
                || IsPointInTriangle(px, py, a, b, c)
                || IsPointInTriangle(px, py, c, b, vortexB);
        }

        /// <summary>
        /// 检查点是否在三角形内
        /// </summary>
        /// <param name="px">点的 X 坐标</param>
        /// <param name="py">点的 Y 坐标</param>
        /// <param name="a">三角形顶点 A</param>
        /// <param name="b">三角形顶点 B</param>
        /// <param name="c">三角形顶点 C</param>
        /// <returns>如果点在三角形内返回 true，否则返回 false</returns>
        private static bool IsPointInTriangle(int px, int py, Coordinate a, Coordinate b, Coordinate c)
        {
            Coordinate p = new(px, py);
            float areaTotal = GetTriangleArea(a, b, c);
            float area1 = GetTriangleArea(p, b, c);
            float area2 = GetTriangleArea(a, p, c);
            float area3 = GetTriangleArea(a, b, p);

            return Math.Abs(areaTotal - (area1 + area2 + area3)) < 0.0001f;
        }

        /// <summary>
        /// 计算三角形面积
        /// </summary>
        /// <param name="A">三角形顶点 A</param>
        /// <param name="B">三角形顶点 B</param>
        /// <param name="C">三角形顶点 C</param>
        /// <returns>三角形面积</returns>
        private static float GetTriangleArea(Coordinate A, Coordinate B, Coordinate C)
        {
            return Math.Abs((A.X * (B.Y - C.Y) +
                             B.X * (C.Y - A.Y) +
                             C.X * (A.Y - B.Y)) / 2.0f);
        }

        /// <summary>
        /// 坐标结构体
        /// </summary>
        private readonly record struct Coordinate(int X, int Y);

        /// <summary>
        /// 悬崖基础颜色
        /// </summary>
        private static readonly Color CliffBase = ColorUtil.Blend(Grass, Color.Black, 0.6d);

        /// <summary>
        /// 获取地形 tile 的默认颜色
        /// </summary>
        /// <param name="mdl">地形单元模型</param>
        /// <param name="landAngle">土地角度</param>
        /// <param name="relativeX">相对 X 坐标</param>
        /// <param name="relativeY">相对 Y 坐标</param>
        /// <returns>地形 tile 的默认颜色</returns>
        private static Color GetTileDefaultColor(TerrainUnitModel mdl, ushort landAngle, int relativeX, int relativeY)
        {
            var angle = (LandAngles)landAngle;
            if (mdl.IsRiver())
                return GetRiverColor(mdl, angle, relativeX, relativeY);
            if (mdl.IsFall())
                return Color.DeepSkyBlue;
            if (mdl.IsCliff())
                return CliffBase;
            return Grass;
        }

        /// <summary>
        /// 数字字符数组
        /// </summary>
        private static readonly char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// 获取地形 tile 的名称
        /// </summary>
        /// <param name="tile">地形 tile 对象</param>
        /// <returns>格式化后的地形 tile 名称</returns>
        public static string GetTileName(TerrainTile tile)
        {
            var name = tile.UnitModel.ToString();
            var num = name.IndexOfAny(Numbers);
            if (num < 0)
                return name;
            return name.Substring(0, num) + Environment.NewLine + name.Substring(num);
        }
    }
}

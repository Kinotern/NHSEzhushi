using System.Collections.Generic;

namespace NHSE.Core
{
    /// <summary>
    /// 存储游戏本地化字符串，供逻辑使用
    /// </summary>
    public sealed class GameStrings : IRemakeString
    {
        /// <summary>
        /// 当前使用的语言代码
        /// </summary>
        private readonly string lang;

        /// <summary>
        /// 村民名称数组
        /// </summary>
        public readonly string[] villagers;
        /// <summary>
        /// 物品名称数组
        /// </summary>
        public readonly string[] itemlist;
        /// <summary>
        /// 物品显示名称数组
        /// </summary>
        public readonly string[] itemlistdisplay;
        /// <summary>
        /// 村民默认短语数组
        /// </summary>
        public readonly string[] villagerDefaultPhrases;
        /// <summary>
        /// 村民名称映射字典
        /// </summary>
        public readonly Dictionary<string, string> VillagerMap;
        /// <summary>
        /// 村民默认短语映射字典
        /// </summary>
        public readonly Dictionary<string, string> VillagerDefaultPhraseMap;
        /// <summary>
        /// 物品数据源列表
        /// </summary>
        public readonly List<ComboItem> ItemDataSource;
        /// <summary>
        /// 内部名称翻译字典
        /// </summary>
        public readonly Dictionary<string, string> InternalNameTranslation = new();

        /// <summary>
        /// 身体部位字典
        /// </summary>
        public IReadOnlyDictionary<string, string> BodyParts { get; }
        /// <summary>
        /// 身体颜色字典
        /// </summary>
        public IReadOnlyDictionary<string, string> BodyColor { get; }
        /// <summary>
        /// 布料部位字典
        /// </summary>
        public IReadOnlyDictionary<string, string> FabricParts { get; }
        /// <summary>
        /// 布料颜色字典
        /// </summary>
        public IReadOnlyDictionary<string, string> FabricColor { get; }

        /// <summary>
        /// 获取指定标识的字符串数组
        /// </summary>
        /// <param name="ident">字符串资源标识</param>
        /// <returns>字符串数组</returns>
        private string[] Get(string ident) => GameLanguage.GetStrings(ident, lang);

        /// <summary>
        /// 初始化GameStrings实例
        /// </summary>
        /// <param name="l">语言代码</param>
        public GameStrings(string l)
        {
            lang = l;
            villagers = Get("villager");
            VillagerMap = GetMap(villagers);
            villagerDefaultPhrases = Get("phrase");
            VillagerDefaultPhraseMap = GetMap(villagerDefaultPhrases);
            itemlist = Get("item");
            itemlistdisplay = GetItemDisplayList(itemlist);
            ItemDataSource = CreateItemDataSource(itemlistdisplay);

            BodyParts = GetDictionary(Get("body_parts"));
            BodyColor = GetDictionary(Get("body_color"));
            FabricParts = GetDictionary(Get("fabric_parts"));
            FabricColor = GetDictionary(Get("fabric_color"));
        }

        /// <summary>
        /// 将字符串数组转换为字典
        /// </summary>
        /// <param name="lines">字符串数组</param>
        /// <param name="split">分隔符</param>
        /// <returns>转换后的字典</returns>
        private static IReadOnlyDictionary<string, string> GetDictionary(IEnumerable<string> lines, char split = '\t')
        {
            var result = new Dictionary<string, string>();
            foreach (var s in lines)
            {
                if (s.Length == 0)
                    continue;
                var index = s.IndexOf(split);
                var key = s.Substring(0, index);
                var value = s.Substring(index + 1);
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        /// 从字符串数组创建物品数据源
        /// </summary>
        /// <param name="strings">字符串数组</param>
        /// <returns>物品数据源列表</returns>
        private List<ComboItem> CreateItemDataSource(string[] strings)
        {
            var dataSource = ComboItemUtil.GetArray(strings);

            // load special
            dataSource.Add(new ComboItem(itemlist[0], Item.NONE));
            dataSource.SortByText();

            return dataSource;
        }

        /// <summary>
        /// 从物品ID集合创建物品数据源
        /// </summary>
        /// <param name="dict">物品ID集合</param>
        /// <param name="none">是否包含无物品选项</param>
        /// <returns>物品数据源列表</returns>
        public List<ComboItem> CreateItemDataSource(IReadOnlyCollection<ushort> dict, bool none = true)
        {
            var display = itemlistdisplay;
            var result = new List<ComboItem>(dict.Count);
            foreach (var x in dict)
                result.Add(new ComboItem(display[x], x));

            if (none)
                result.Add(new ComboItem(itemlist[0], Item.NONE));

            result.SortByText();
            return result;
        }

        /// <summary>
        /// 从键值对集合创建物品数据源
        /// </summary>
        /// <param name="dict">键值对集合</param>
        /// <param name="none">是否包含无物品选项</param>
        /// <returns>物品数据源列表</returns>
        public List<ComboItem> CreateItemDataSource(IReadOnlyCollection<KeyValuePair<ushort, ushort>> dict, bool none = true)
        {
            var display = itemlistdisplay;
            var result = new List<ComboItem>(dict.Count);
            foreach (var x in dict)
                result.Add(new ComboItem(display[x.Value], x.Key));

            if (none)
                result.Add(new ComboItem(itemlist[0], Item.NONE));

            result.SortByText();
            return result;
        }

        /// <summary>
        /// 从字符串数组创建映射字典
        /// </summary>
        /// <param name="arr">字符串数组</param>
        /// <returns>映射字典</returns>
        private static Dictionary<string, string> GetMap(IReadOnlyCollection<string> arr)
        {
            var map = new Dictionary<string, string>(arr.Count);
            foreach (var kvp in arr)
            {
                var index = kvp.IndexOf('\t');
                if (index < 0)
                    continue;
                var abbrev = kvp.Substring(0, index);
                var name = kvp.Substring(index + 1);
                map.Add(abbrev, name);
            }
            return map;
        }

        /// <summary>
        /// 获取村民名称
        /// </summary>
        /// <param name="name">村民标识</param>
        /// <returns>村民名称</returns>
        public string GetVillager(string name)
        {
            return VillagerMap.TryGetValue(name, out var result) ? result : name;
        }

        /// <summary>
        /// 获取村民默认短语
        /// </summary>
        /// <param name="name">村民标识</param>
        /// <returns>村民默认短语</returns>
        public string GetVillagerDefaultPhrase(string name)
        {
            return VillagerDefaultPhraseMap.TryGetValue(name, out var result) ? result : name; // I know it shouldn't be name but I have to return something
        }

        /// <summary>
        /// 获取物品显示名称列表
        /// </summary>
        /// <param name="items">物品名称数组</param>
        /// <returns>物品显示名称数组</returns>
        public static string[] GetItemDisplayList(string[] items)
        {
            items = (string[])items.Clone();
            items[0] = string.Empty;
            var set = new HashSet<string>();
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (string.IsNullOrEmpty(item))
                    items[i] = $"(Item #{i:000})";
                else if (set.Contains(item))
                    items[i] += $" (#{i:000})";
                else
                    set.Add(item);
            }
            return items;
        }

        /// <summary>
        /// 获取物品名称
        /// </summary>
        /// <param name="item">物品实例</param>
        /// <returns>物品名称</returns>
        public string GetItemName(Item item)
        {
            var index = item.ItemId;
            if (index == Item.NONE)
                return itemlist[0];
            if (index == Item.EXTENSION)
                return GetItemName(item.ExtensionItemId);

            var kind = ItemInfo.GetItemKind(index);

            if (kind.IsFlowerGene(index))
            {
                var display = GetItemName(index);
                if (item.Genes != 0)
                    return $"{display} - {item.Genes}";
            }

            if (kind == ItemKind.Kind_DIYRecipe || kind == ItemKind.Kind_MessageBottle)
            {
                var display = itemlistdisplay[index];
                var recipeID = (ushort)item.FreeParam;
                var isKnown = RecipeList.Recipes.TryGetValue(recipeID, out var result);
                var makes = isKnown ? GetItemName(result) : recipeID.ToString("000");
                return $"{display} - {makes}";
            }

            if (kind == ItemKind.Kind_FossilUnknown)
            {
                var display = itemlistdisplay[index];
                var fossilID = (ushort)item.FreeParam;
                var fossilName = GetItemName(fossilID);
                return $"{display} - {fossilName}";
            }

            if (kind == ItemKind.Kind_Tree)
            {
                var display = GetItemName(index);
                var willDrop = item.Count;
                if (willDrop != 0)
                {
                    var dropName = GetItemName(willDrop);
                    return $"{display} - {dropName}";
                }
            }

            return GetItemName(index);
        }

        /// <summary>
        /// 获取物品名称
        /// </summary>
        /// <param name="index">物品ID</param>
        /// <returns>物品名称</returns>
        public string GetItemName(ushort index)
        {
            if (index >= itemlistdisplay.Length)
                return GetItemName60000(index);
            return itemlistdisplay[index];
        }

        /// <summary>
        /// 获取ID大于60000的物品名称
        /// </summary>
        /// <param name="index">物品ID</param>
        /// <returns>物品名称</returns>
        private static string GetItemName60000(ushort index)
        {
            if (FieldItemList.Items.TryGetValue(index, out var val))
                return val.Name;

            // 63,000 ???
            if (index == Item.LLOYD)
                return "Lloyd";

            return "???";
        }

        /// <summary>
        /// 获取带有括号的服装或物品颜色变体
        /// </summary>
        /// <param name="id">颜色变体搜索的物品ID</param>
        /// <param name="baseItemName">不包含相关颜色变体的物品名称</param>
        /// <returns>物品ID和物品名称的映射</returns>
        public List<ComboItem> GetAssociatedItems(ushort id, out string baseItemName)
        {
            baseItemName = string.Empty;
            var stringMatch = GetItemName(id);
            var index = stringMatch.IndexOf('(');
            if (index < 0)
                return new List<ComboItem>();

            var search = baseItemName = stringMatch.Substring(0, index);
            if (!string.IsNullOrWhiteSpace(search))
                return ItemDataSource.FindAll(x => x.Text.StartsWith(search));
            else
                return new List<ComboItem>();
        }

        /// <summary>
        /// 检查是否有关联物品
        /// </summary>
        /// <param name="baseName">基础物品名称</param>
        /// <param name="items">关联物品列表</param>
        /// <returns>是否有关联物品</returns>
        public bool HasAssociatedItems(string baseName, out List<ComboItem>? items)
        {
            if (string.IsNullOrWhiteSpace(baseName))
            {
                items = null;
                return false;
            }

            baseName = baseName.Trim().ToLower();
            if (!baseName.EndsWith(" "))
                baseName += " ";
            baseName += "(";

            items = ItemDataSource.FindAll(x => x.Text.ToLower().StartsWith(baseName));
            return items.Count > 0;
        }
    }

    /// <summary>
    /// 重制版字符串接口
    /// </summary>
    public interface IRemakeString
    {
        /// <summary>
        /// 身体部位字典
        /// </summary>
        IReadOnlyDictionary<string, string> BodyParts { get; }
        /// <summary>
        /// 身体颜色字典
        /// </summary>
        IReadOnlyDictionary<string, string> BodyColor { get; }
        /// <summary>
        /// 布料部位字典
        /// </summary>
        IReadOnlyDictionary<string, string> FabricParts { get; }
        /// <summary>
        /// 布料颜色字典
        /// </summary>
        IReadOnlyDictionary<string, string> FabricColor { get; }
    }
}

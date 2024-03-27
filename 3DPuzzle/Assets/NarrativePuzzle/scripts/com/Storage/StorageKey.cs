namespace com
{
    public class StorageKey
    {
        public static string prefix = "vom";

        public static string GetFullKey(string accountId, string storageId)
        {
            return prefix + "_" + accountId + "_" + storageId;
        }

        public static string GetFullKey(string accountId, Key k)
        {
            return GetFullKey(accountId, k.ToString());
        }

        public enum Key
        {
            Account = 0,
            Misc = 1,//misc
            Inventory = 2,//equipments items
            //DailyLevel = 3,//pendingLevels perk data date
            Talent = 4,
            Town = 5,
            Hero = 6,//class
            Shop = 7,
            Map = 8,//blessings
            Quest = 9,//npc give crt & completed quests
            Settings = 10,
        }
    }
}
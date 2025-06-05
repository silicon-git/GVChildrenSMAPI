using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static StardewValley.Minigames.TargetGame;
using System.Xml.Linq;

namespace GVChildrenSMAPI
{
    public class ChildTokenImplement

    {
        public bool AllowsInput()
        {
            return true;
        }

        public bool RequiresInput()
        {
            return true;
        }

        public IEnumerable<string> GetValues(string input)
        {
            string[] entries = input.Split("+");

            int childindex = int.Parse(entries[0]);
            string childdata = entries[1];

            if (Context.IsWorldReady)
            {
                //taken from LittleNPCs by mus-candidus
                var children = GetChildrenFromFarmHouse(false, out FarmHouse farmHouse);
                Child child = children.FirstOrDefault(c => c.GetChildIndex() == childindex);
                if (child is not null)
                {

                    if (childdata.ToLower() == "isdarkskinned")
                    {
                        yield return child.darkSkinned.ToString();
                    }
                    else
                    {
                        yield return "";
                    }
                }
                else
                {
                    yield return "";
                }
            }
            else
            {
                //taken from LittleNPCs by mus-candidus
                // World not ready, load from save.
                var children = GetChildrenFromFarmHouse(true, out FarmHouse farmHouse);
                Child child = children.FirstOrDefault(c => c.GetChildIndex() == childindex);
                if (child is not null)
                {

                    if (childdata.ToLower() == "isdarkskinned")
                    {
                        yield return child.darkSkinned.ToString();
                    }
                    else
                    {
                        yield return "";
                    }
                }
                else
                {
                    yield return "";
                }
            }

            //taken from LittleNPCs by mus-candidus
            static IEnumerable<Child> GetChildrenFromFarmHouse(bool loadFromSave, out FarmHouse farmHouse)
            {
                farmHouse = loadFromSave ? SaveGame.loaded?.locations.OfType<FarmHouse>().FirstOrDefault(l => l.Name == "FarmHouse")
                                         : Utility.getHomeOfFarmer(Game1.player);

                return farmHouse is not null ? farmHouse.getChildren()
                                             : Enumerable.Empty<Child>();
            }
        }
    }
}

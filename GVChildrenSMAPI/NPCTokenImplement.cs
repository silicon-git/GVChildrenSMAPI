using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Characters;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GVChildrenSMAPI
{
    public class NPCTokenImplement
    {
        public bool AllowsInput()
        {
            return true;
        }

        public bool RequiresInput()
        {
            return true;
        }

        public bool UpdateContext()
        {
            return Context.IsWorldReady != false;
        }
        public bool IsReady()
        {
            return Context.IsWorldReady != false;
        }
        public IEnumerable<string> GetValues(string input)
        {
            string[] entries = input.Split("+");

            string npcname = entries[0];
            string npcdata = entries[1];

            if (npcname == "")
            {
                yield return "";
            }

            else
            {

                NPC target = Game1.RequireCharacter(npcname);

                if (npcdata.ToLower() == "displayname")
                {
                    yield return target.displayName.ToString();
                }

                if (npcdata.ToLower() == "gender")
                {
                    yield return target.Gender.ToString();
                }

                else if (npcdata.ToLower() == "birthday_season")
                {
                    yield return target.Birthday_Season.ToString();
                }

                else if (npcdata.ToLower() == "birthday_day")
                {
                    yield return target.Birthday_Day.ToString();
                }

                else if (npcdata.ToLower() == "age")
                {
                    yield return target.Age.ToString();
                }

                else if (npcdata.ToLower() == "manners")
                {
                    yield return target.Manners.ToString();
                }

                else if (npcdata.ToLower() == "socialanxiety")
                {
                    yield return target.SocialAnxiety.ToString();
                }

                else if (npcdata.ToLower() == "optimism")
                {
                    yield return target.Optimism.ToString();
                }

                else if (npcdata.ToLower() == "datable")
                {
                    yield return target.datable.ToString();
                }

                else if (npcdata.ToLower() == "loveinterest")
                {
                    yield return target.loveInterest.ToString();
                }

                else if (npcdata.ToLower() == "haspartnerfordance")
                {
                    yield return target.HasPartnerForDance.ToString();
                }

                else if (npcdata.ToLower() == "isinvisible")
                {
                    yield return target.IsInvisible.ToString();
                }

                else if (npcdata.ToLower() == "cansocialize")
                {
                    yield return target.CanSocialize.ToString();
                }

                else if (npcdata.ToLower() == "isdarkskinned")
                {
                    yield return target.hasDarkSkin().ToString();
                }

                else if (npcdata.ToLower() == "isadoptionspouse")
                {
                    yield return target.isAdoptionSpouse().ToString();
                }

                else if (npcdata.ToLower() == "cangetpregnant")
                {
                    yield return target.canGetPregnant().ToString();
                }

                else if (npcdata.ToLower() == "ismarriedorengaged")
                {
                    yield return target.isMarriedOrEngaged().ToString();
                }

                else if (npcdata.ToLower() == "canreceivegifts")
                {
                    yield return target.CanReceiveGifts().ToString();
                }

                else
                {
                    yield return null;
                }
            }

        }
    }
}

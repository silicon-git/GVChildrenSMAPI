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
    public class ResolvedConfigValues
    {
        private ModConfig Config;
        public static string ChildDialogue(string ChildDialogueSet)
        {
            if (ChildDialogueSet == "Default")
            {
                if (Game1.player.isMarriedOrRoommates())
                
                    {
                        return "Canonical";
                    }
                else
                    {
                        return "Single";
                    }
            }
            else
            {
                return ChildDialogueSet;
            }
        }

        public static string ParentFarmer(string ParentFarmerOverrides, string ParentFarmerMaleSet, string DefaultParentFarmerMale, string ParentFarmerFemaleSet, string DefaultParentFarmerFemale, string ParentFarmerOtherSet, string DefaultParentFarmerOther, bool OverrideModLoaded)
        {
            string ParentFarmerMale = ParentFarmerMaleSet == ""
                           ? DefaultParentFarmerMale
                           : ParentFarmerMaleSet;
            string ParentFarmerFemale = ParentFarmerFemaleSet == ""
                           ? DefaultParentFarmerFemale
                           : ParentFarmerFemaleSet;
            string ParentFarmerOther = ParentFarmerOtherSet == ""
                           ? DefaultParentFarmerOther
                           : ParentFarmerOtherSet;

            if (Context.IsWorldReady)
            {
                switch (ParentFarmerOverrides)
                {

                    case "UseFarmerName":
                        {
                            return Game1.player.Name;
                        }
                    case "UseFarmerOther":
                        {
                            return ParentFarmerOther;
                        }
                    default:
                        {
                            if (OverrideModLoaded)
                            {
                                return ParentFarmerOther;
                            }
                            else
                            {
                                switch (Game1.player.Gender)
                                {
                                    case Gender.Male:
                                        {
                                            return ParentFarmerMale;
                                        }
                                    case Gender.Female:
                                        {
                                            return ParentFarmerFemale;
                                        }
                                    default:
                                        {
                                            return ParentFarmerOther;
                                        }
                                }
                            }
                        }
                }
            }
            else
            {
                return "no_game_loaded";
            }
        }

        public static string ParentSpouse(string ParentSpouseOverrides, string ParentSpouseMaleSet, string DefaultParentSpouseMale, string ParentSpouseFemaleSet, string DefaultParentSpouseFemale, string ParentSpouseOtherSet, string DefaultParentSpouseOther)
        {
            string ParentSpouseMale = ParentSpouseMaleSet == ""
                           ? DefaultParentSpouseMale
                           : ParentSpouseMaleSet;
            string ParentSpouseFemale = ParentSpouseFemaleSet == ""
                           ? DefaultParentSpouseFemale
                           : ParentSpouseFemaleSet;
            string ParentSpouseOther = ParentSpouseOtherSet == ""
                           ? DefaultParentSpouseOther
                           : ParentSpouseOtherSet;

            if (Context.IsWorldReady)
            {
                switch (ParentSpouseOverrides)
                {
                    case "UseSpouseName":
                        {
                            if (Game1.player.getSpouse() != null)
                            {
                                return Game1.player.getSpouse().displayName;
                            }
                            else
                            {
                                return ParentSpouseOther;
                            }
                        }
                    case "UseSpouseOther":
                        {
                            return ParentSpouseOther;
                        }
                    default:
                        {
                            if (Game1.player.getSpouse() != null)
                            {
                                switch (Game1.player.getSpouse().Gender)
                                {
                                    case Gender.Male:
                                        {
                                            return ParentSpouseMale;
                                        }
                                    case Gender.Female:
                                        {
                                            return ParentSpouseFemale;
                                        }
                                    default:
                                        {
                                            return ParentSpouseOther;
                                        }
                                }
                            } 
                            else
                            {
                                return ParentSpouseOther;
                            }
                        }
                }
            }
            else
            {
                return "no_game_loaded";
            }
        }


        public static string ParentRoommate(string RoommateOverrides, string RoommateMaleSet, string DefaultRoommateMale, string RoommateFemaleSet, string DefaultRoommateFemale, string RoommateOtherSet, string DefaultRoommateOther, string DefaultRoommateKrobus)
        {
            string RoommateMale = RoommateMaleSet == ""
                           ? DefaultRoommateMale
                           : RoommateMaleSet;
            string RoommateFemale = RoommateFemaleSet == ""
                           ? DefaultRoommateFemale
                           : RoommateFemaleSet;
            string RoommateOther = RoommateOtherSet == ""
                           ? DefaultRoommateOther
                           : RoommateOtherSet;

            if (Context.IsWorldReady)
            {
                if (RoommateOverrides == "DefaultRoommateName" & Game1.player.getSpouse() == Game1.getCharacterFromName("Krobus"))
                {
                    return DefaultRoommateKrobus;
                }
                else
                {
                    switch (RoommateOverrides)
                    {
                        case "UseRoommateTitle":
                            {
                                if (Game1.player.getSpouse() != null)
                                {
                                    switch (Game1.player.getSpouse().Gender)
                                    {
                                        case Gender.Male:
                                            {
                                                return RoommateMale;
                                            }
                                        case Gender.Female:
                                            {
                                                return RoommateFemale;
                                            }
                                        default:
                                            {
                                                return RoommateOther;
                                            }
                                    }
                                }
                                else
                                {
                                    return RoommateOther;
                                }
                            }
                        case "UseRoommateOther":
                            {
                                return RoommateOther;
                            }
                        default:
                            {
                                if (Game1.player.getSpouse() != null)
                                {
                                    return Game1.player.getSpouse().displayName;
                                }
                                else
                                {
                                    return RoommateOther;
                                }
                            }
                    }
                }
            }
            else
            {
                return "no_game_loaded";
            }
        }


        public static string CompanionPronouns(bool SpousePronounsEnabled, string DefaultSpousePronounsMale, string DefaultSpousePronounsFemale, string DefaultSpousePronounsOther, string SpousePronounSet)
        {
            if (Context.IsWorldReady)
            {
                if (SpousePronounsEnabled)
                {
                    return SpousePronounSet;
                }
                else
                {
                    if (Game1.player.getSpouse() != null)
                    {
                        switch (Game1.player.getSpouse().Gender)
                        {
                            case Gender.Male:
                                {
                                    return DefaultSpousePronounsMale;
                                }
                            case Gender.Female:
                                {
                                    return DefaultSpousePronounsFemale;
                                }
                            default:
                                {
                                    return DefaultSpousePronounsOther;
                                }
                            }
                        }
                    else
                    {
                        return DefaultSpousePronounsOther;
                    }

                }
            }
            else
            {
                return "no_game_loaded";
            }
        }


        public static bool CompanionPronounPlurality(bool SpousePronounsEnabled, bool SpousePronounPluralitySet)
        {
            if (Context.IsWorldReady)
            {
                if (SpousePronounsEnabled)
                {
                    return SpousePronounPluralitySet;
                }
                else
                {
                    if (Game1.player.getSpouse() != null)
                    {
                        if (Game1.player.getSpouse().Gender == Gender.Male | Game1.player.getSpouse().Gender == Gender.Female)
                            {
                                return false;
                            }
                        else
                            {
                                return true;
                            }
                    }
                    else
                        {
                            return true;
                        }
                    }
            }
            else
            {
                return true;
            }
        }


    }
}

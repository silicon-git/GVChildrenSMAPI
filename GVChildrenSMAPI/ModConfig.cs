using StardewModdingAPI.Events;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.GameData.Characters;

namespace GVChildrenSMAPI
{
    public sealed class ModConfig
    {
        public string FirstbornHairTypeSet { get; set; } = "Default";
        public string FirstbornHairLengthSet { get; set; } = "Shorter";
        public string FirstbornHairColourSet { get; set; } = "Default";
        public string FirstbornEyeColourSet { get; set; } = "Default";
        public string FirstbornClothingTypeSet { get; set; } = "Shirt";
        public string FirstbornClothingColourSet { get; set; } = "Red"; 

        public string SecondbornHairTypeSet { get; set; } = "Default";
        public string SecondbornHairLengthSet { get; set; } = "Shorter";
        public string SecondbornHairColourSet { get; set; } = "Default";
        public string SecondbornEyeColourSet { get; set; } = "Default";
        public string SecondbornClothingTypeSet { get; set; } = "Shirt";
        public string SecondbornClothingColourSet { get; set; } = "Blue";

        public string ChildSkinToneSet { get; set; } = "Default";


        public string ParentFarmerMale { get; set; } = "";
        public string ParentFarmerFemale { get; set; } = "";
        public string ParentFarmerOther { get; set; } = "";
        public string ParentFarmerOverrides { get; set; } = "None";

        public string ParentSpouseMale { get; set; } = "";
        public string ParentSpouseFemale { get; set; } = "";
        public string ParentSpouseOther { get; set; } = "";
        public string ParentSpouseOverrides { get; set; } = "None";

        public string RoommateOverrides { get; set; } = "DefaultRoommateName";
        public string RoommateMale { get; set; } = "";
        public string RoommateFemale { get; set; } = "";
        public string RoommateOther { get; set; } = "";

        public bool SpousePronounsEnabled { get; set; } = false;
        public string SpousePronounsSubject { get; set; } = "They";
        public string SpousePronounsObject { get; set; } = "Them";
        public string SpousePronounsPosDet { get; set; } = "Their";
        public string SpousePronounsPossessive { get; set; } = "Theirs";
        public string SpousePronounsReflexive { get; set; } = "Themself";
        public bool SpousePronounsPlurality { get; set; } = true;

        public string SpouseGenderOverride { get; set; } = "None";
        public string RoommateGenderOverride { get; set; } = "None";


        public bool EliasPromoEnabled { get; set; } = false;
        public bool LizziePromoEnabled { get; set; } = false;

        public string FirstbornSelect { get; set; } = "DefaultParentCheck";
        public string FirstbornDialogueSet { get; set; } = "Default";
        public string SecondbornSelect { get; set; } = "DefaultParentCheck";
        public string SecondbornDialogueSet { get; set; } = "Default";

        public string FarmerRefFirstbornSet { get; set; } = "";
        public string CompanionRefFirstbornSet { get; set; } = "";
        public bool CompanionPrnsFirstbornEnabled { get; set; } = false;
        public string CompanionPrnsSubjectFirstborn { get; set; } = "They";
        public string CompanionPrnsObjectFirstborn { get; set; } = "Them";
        public string CompanionPrnsPosDetFirstborn { get; set; } = "Their";
        public string CompanionPrnsPossessiveFirstborn { get; set; } = "Theirs";
        public string CompanionPrnsReflexiveFirstborn { get; set; } = "Themself";
        public bool CompanionPrnsPluralityFirstborn { get; set; } = true;

        public string FarmerRefSecondbornSet { get; set; } = "";
        public string CompanionRefSecondbornSet { get; set; } = "";
        public bool CompanionPrnsSecondbornEnabled { get; set; } = false;
        public string CompanionPrnsSubjectSecondborn { get; set; } = "They";
        public string CompanionPrnsObjectSecondborn { get; set; } = "Them";
        public string CompanionPrnsPosDetSecondborn { get; set; } = "Their";
        public string CompanionPrnsPossessiveSecondborn { get; set; } = "Theirs";
        public string CompanionPrnsReflexiveSecondborn { get; set; } = "Themself";
        public bool CompanionPrnsPluralitySecondborn { get; set; } = true;
    }

}

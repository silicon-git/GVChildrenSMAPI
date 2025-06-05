using ContentPatcher;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Triggers;
using StardewValley.BellsAndWhistles;
using Netcode;

namespace GVChildrenSMAPI
{
    public class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.Content.AssetReady += this.OnAssetReady;
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            this.Config = this.Helper.ReadConfig<ModConfig>();
        }


        /*
         * taken from ichortower who got it from Shockah, I am not smart enough to do this
         */
        public static Lazy<Action<string>> QueueConsoleCommand = new(() => {
            var sCoreType = Type.GetType(
                    "StardewModdingAPI.Framework.SCore,StardewModdingAPI")!;
            var commandQueueType = Type.GetType(
                    "StardewModdingAPI.Framework.CommandQueue,StardewModdingAPI")!;
            var sCoreGetter = sCoreType.GetProperty("Instance",
                    BindingFlags.NonPublic | BindingFlags.Static).GetGetMethod(true);
            var rawCommandQueueField = sCoreType.GetField("RawCommandQueue",
                    BindingFlags.NonPublic | BindingFlags.Instance);
            var queueAddMethod = commandQueueType.GetMethod("Add",
                    BindingFlags.Public | BindingFlags.Instance);

            var method = new DynamicMethod("QueueConsoleCommand",
                    null, new Type[] { typeof(string) });
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Call, sCoreGetter);
            il.Emit(OpCodes.Ldfld, rawCommandQueueField);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, queueAddMethod);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate<Action<string>>();
        });

        /*
         * lots of help from focustense's StardewRadialMenu mod config here
         */

        /* original image implementation
        //initialize variables for each preview image asset
        private readonly Color[] EliasPreviewDefault = new Color[334 * 93];
        private readonly Color[] LizziePreviewDefault = new Color[334 * 93];
        private readonly Color[] EliasPreviewAlt = new Color[334 * 93];
        private readonly Color[] LizziePreviewAlt = new Color[334 * 93];

        //initialize null variable for the preview image which is used
        private Texture2D EliasPreview = null!;
        private Texture2D LizziePreview = null!;

        //actually set up preview images
        public void PreviewUpdateSetup()
        {
            LoadAssets();
            EliasPreview = new(Game1.graphics.GraphicsDevice, 334, 93);
            LizziePreview = new(Game1.graphics.GraphicsDevice, 334, 93);
            UpdateFieldDataFromConfig();
        }

        //function for getting data from preview image files and load into preview image asset variables
        private void LoadAssets()
        {
            this.Helper.ModContent.Load<Texture2D>("assets/previews/Elias_promo_preview_default.png").GetData(EliasPreviewDefault);
            this.Helper.ModContent.Load<Texture2D>("assets/previews/Lizzie_promo_preview_default.png").GetData(LizziePreviewDefault);
            this.Helper.ModContent.Load<Texture2D>("assets/previews/Elias_promo_preview_alternate.png").GetData(EliasPreviewAlt);
            this.Helper.ModContent.Load<Texture2D>("assets/previews/Lizzie_promo_preview_alternate.png").GetData(LizziePreviewAlt);
        }

        //set data of preview image which is used depending on which conditions are fulfilled (Elias)
        private void UpdateEliasPreview()
        {
            Color[] previewData = new Color[334 * 93];
            for (var i = 0; i < 334 * 93; i++)
            {
                if (GetBoolFromAmbiguousType(liveFieldData["siliconmodding.GVChildrenSMAPI_EliasPromoEnabled"]) == true)
                {
                    previewData[i] = EliasPreviewAlt[i];
                }
                else
                {
                    previewData[i] = EliasPreviewDefault[i];
                }
            }
            EliasPreview.SetData(previewData);
        }

        //set data of preview image which is used depending on which conditions are fulfilled (Lizzie)
        private void UpdateLizziePreview()
        {
            Color[] previewData = new Color[334 * 93];
            for (var i = 0; i < 334 * 93; i++)
            {
                if (GetBoolFromAmbiguousType(liveFieldData["siliconmodding.GVChildrenSMAPI_LizziePromoEnabled"]) == true)
                {
                    previewData[i] = LizziePreviewAlt[i];
                }
                else
                {
                    previewData[i] = LizziePreviewDefault[i];
                }
            }
            LizziePreview.SetData(previewData);
        }
        */

        // GMCM won't update the actual configuration until it's saved, so we have to store transient
        // values (while editing) from OnFieldChanged in a local lookup.
        //(in silicon's words): GMCM stupid so we have to track live values ourselves
        //initialize dictionary for live value tracking
        private readonly Dictionary<string, object> liveFieldData = [];

        //function for updating live field data dictionary with saved config
        private void UpdateFieldDataFromConfig()
        {
            liveFieldData["siliconmodding.GVChildrenSMAPI_EliasPromoEnabled"] = this.Config.EliasPromoEnabled;
            liveFieldData["siliconmodding.GVChildrenSMAPI_LizziePromoEnabled"] = this.Config.LizziePromoEnabled;
        }

        //go from object to bool (as GMCM will give us an ambiguous object, not a bool)
        private static bool? GetBoolFromAmbiguousType(object value)
        {
            if (value is bool isbool)
            {
                return isbool;
            }
            return null;
        }


        //taking this from Pathos's Lookup Anything
        public static class PortraitFrame
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("LooseSprites/Cursors");
            public static readonly Rectangle Sprite = new Rectangle(603, 414, 74, 74);
        }

        public static class EliasDefaultPale
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/EliasDefaultPale");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }
        public static class EliasPromoPale
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/EliasPromoPale");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }
        public static class EliasDefaultDark
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/EliasDefaultDark");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }
        public static class EliasPromoDark
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/EliasPromoDark");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }

        public static class LizzieDefaultDark
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/LizzieDefaultDark");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }
        public static class LizziePromoDark
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/LizziePromoDark");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }
        public static class LizzieDefaultPale
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/LizzieDefaultPale");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }
        public static class LizziePromoPale
        {
            public static Texture2D Sheet => Game1.content.Load<Texture2D>("siliconmodding.GVChildrenSMAPI/LizziePromoPale");
            public static readonly Rectangle Sprite = new Rectangle(0, 0, 64, 64);
        }

        //b-b-blueberry's spritebatches and spritebatch loads in Love of Cooking were a huge help
        public void DrawPreviewElias(SpriteBatch b, Vector2 v)
        {
            Color maskColourPromo = Color.White;
            Color maskColourDefault = Color.White;

            if (GetBoolFromAmbiguousType(liveFieldData["siliconmodding.GVChildrenSMAPI_EliasPromoEnabled"]) == true)
            {
                maskColourPromo = Color.White;
                maskColourDefault = Color.Gray;
            }
            else
            {
                maskColourPromo = Color.Gray;
                maskColourDefault = Color.White;
            }

            //spacing this first one out so future me doesn't die looking at it
            b.Draw(
                PortraitFrame.Sheet, 
                new Vector2(2, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), //first Vector2: Position in the 334*74 spritebatch, times 3 because it's scaled by 3. second Vector2: v.X (the X value of v, a Vector2 from GMCM) - width of full image * 3 (scale) / 2 (centers the image onscreen)
                PortraitFrame.Sprite, 
                maskColourDefault, 
                0, 
                Vector2.Zero, 
                new Vector2(3, 3), //scales the image up by 3
                SpriteEffects.None, 
                0);
            b.Draw(PortraitFrame.Sheet, new Vector2(78, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(PortraitFrame.Sheet, new Vector2(182, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(PortraitFrame.Sheet, new Vector2(258, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(EliasDefaultPale.Sheet, new Vector2(7, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), EliasDefaultPale.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(EliasPromoPale.Sheet, new Vector2(83, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), EliasPromoPale.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(EliasDefaultDark.Sheet, new Vector2(187, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), EliasDefaultDark.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(EliasPromoDark.Sheet, new Vector2(263, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), EliasPromoDark.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
        }

        public void DrawPreviewLizzie(SpriteBatch b, Vector2 v)
        {
            Color maskColourPromo = Color.White;
            Color maskColourDefault = Color.White;

            if (GetBoolFromAmbiguousType(liveFieldData["siliconmodding.GVChildrenSMAPI_LizziePromoEnabled"]) == true)
            {
                maskColourPromo = Color.White;
                maskColourDefault = Color.Gray;
            }
            else
            {
                maskColourPromo = Color.Gray;
                maskColourDefault = Color.White;
            }

            b.Draw(PortraitFrame.Sheet, new Vector2(2, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(PortraitFrame.Sheet, new Vector2(78, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(PortraitFrame.Sheet, new Vector2(182, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(PortraitFrame.Sheet, new Vector2(258, 0) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), PortraitFrame.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(LizzieDefaultDark.Sheet, new Vector2(7, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), LizzieDefaultDark.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(LizziePromoDark.Sheet, new Vector2(83, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), LizziePromoDark.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(LizzieDefaultPale.Sheet, new Vector2(187, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), LizzieDefaultPale.Sprite, maskColourDefault, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);
            b.Draw(LizziePromoPale.Sheet, new Vector2(263, 5) * 3 + new Vector2(v.X - 334 * 3 / 2, v.Y), LizziePromoPale.Sprite, maskColourPromo, 0, Vector2.Zero, new Vector2(3, 3), SpriteEffects.None, 0);

        }


        private Texture2D Data;
        private List<string> assets = new List<string>() { "LizziePromoDark", "LizzieDefaultDark", "LizziePromoPale", "LizzieDefaultPale", "EliasPromoPale", "EliasDefaultPale", "EliasPromoDark", "EliasDefaultDark", };

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            //
            // 1. define the custom asset based on the internal file
            //
            foreach (var asset in assets)
                if (e.Name.IsEquivalentTo($"siliconmodding.GVChildrenSMAPI/{asset}"))
                {
                    e.LoadFromModFile<Texture2D>($"assets/{asset}.png", AssetLoadPriority.Low);
                }
        }
        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            //
            // 2. update the data when it's reloaded
            //
            foreach (var asset in assets)
                if (e.Name.IsEquivalentTo($"siliconmodding.GVChildrenSMAPI/{asset}"))
                {
                    this.Data = Game1.content.Load<Texture2D>($"siliconmodding.GVChildrenSMAPI/{asset}");
                }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            foreach (var asset in assets)
                {
                    this.Data = Game1.content.Load<Texture2D>($"siliconmodding.GVChildrenSMAPI/{asset}");
                }

            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (api != null)
            {
                //duplicate of NPCTokens embedded in GV
                api.RegisterToken(this.ModManifest, "NPC", new NPCTokenImplement());

                //Child tokens, haphazardly assembled from bits of modified LittleNPCs and NPCTokens code. Definitely not MP compatible
                api.RegisterToken(this.ModManifest, "Child", new ChildTokenImplement());

                //token for locale code, used for a stopgap solution for LittleNPCs bug
                api.RegisterToken(this.ModManifest, "LocaleCode", () =>
                {
                    return new[] { LocalizedContentManager.CurrentLanguageString };
                });

                //tokens for config
                var properties = typeof(ModConfig).GetProperties();
                foreach (var property in properties)
                { 
                    api.RegisterToken(this.ModManifest, property.Name, () =>
                    {
                        return new[] { property.GetValue(Config).ToString() };
                    });
                }

                //resolved tokens for config
                api.RegisterToken(this.ModManifest, "FirstbornDialogue", () =>
                {
                    return new[] { ResolvedConfigValues.ChildDialogue(this.Config.FirstbornDialogueSet) };
                });
                api.RegisterToken(this.ModManifest, "SecondbornDialogue", () =>
                {
                    return new[] { ResolvedConfigValues.ChildDialogue(this.Config.SecondbornDialogueSet) };
                });

                //block of variables (note to self: variables won't update when ya pass them through a function >< doesn't matter here because mods loaded shouldn't update right...?)
                bool GNMCPisLoaded = this.Helper.ModRegistry.IsLoaded("Hana.GenderNeutralityMod");
                bool FDDisLoaded = this.Helper.ModRegistry.IsLoaded("Hana.FixDialogueDifferences");
                bool GNMTisLoaded = this.Helper.ModRegistry.IsLoaded("Hana.GNMTokens");
                bool OverrideModLoaded = GNMTisLoaded | GNMCPisLoaded | FDDisLoaded;

                //default farmer reference token (limited use). authors will need to check for GNM Tokens themselves, but it will fallback to ParentFarmerOther values by default
                api.RegisterToken(this.ModManifest, "ParentFarmerDefault", () =>
                {
                      return new[] { ResolvedConfigValues.ParentFarmer(this.Config.ParentFarmerOverrides, this.Config.ParentFarmerMale, this.Helper.Translation.Get("Default.ParentFarmerMale"), this.Config.ParentFarmerFemale, this.Helper.Translation.Get("Default.ParentFarmerFemale"), this.Config.ParentFarmerOther, this.Helper.Translation.Get("Default.ParentFarmerOther"), OverrideModLoaded) };
                });

                //spouse token (not sure what the use for this one would be)
                api.RegisterToken(this.ModManifest, "ParentSpouseDefault", () =>
                {
                    return new[] { ResolvedConfigValues.ParentSpouse(this.Config.ParentSpouseOverrides, this.Config.ParentSpouseMale, this.Helper.Translation.Get("Default.ParentSpouseMale"), this.Config.ParentSpouseFemale, this.Helper.Translation.Get("Default.ParentSpouseFemale"), this.Config.ParentSpouseOther, this.Helper.Translation.Get("Default.ParentSpouseOther")) };
                });

                //roommate token (not sure what the use for this one would be)
                api.RegisterToken(this.ModManifest, "ParentRoommateDefault", () =>
                {
                    return new[] { ResolvedConfigValues.ParentRoommate(this.Config.RoommateOverrides, this.Config.RoommateMale, this.Helper.Translation.Get("Default.RoommateMale"), this.Config.RoommateFemale, this.Helper.Translation.Get("Default.RoommateFemale"), this.Config.RoommateOther, this.Helper.Translation.Get("Default.RoommateOther"), this.Helper.Translation.Get("Default.RoommateKrobus")) };
                });

                //default companion reference token (limited use)
                api.RegisterToken(this.ModManifest, "CompanionRefDefault", () =>
                {
                    if (Game1.player.hasRoommate())
                    {
                        return new[] { ResolvedConfigValues.ParentRoommate(this.Config.RoommateOverrides, this.Config.RoommateMale, this.Helper.Translation.Get("Default.RoommateMale"), this.Config.RoommateFemale, this.Helper.Translation.Get("Default.RoommateFemale"), this.Config.RoommateOther, this.Helper.Translation.Get("Default.RoommateOther"), this.Helper.Translation.Get("Default.RoommateKrobus")) };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.ParentSpouse(this.Config.ParentSpouseOverrides, this.Config.ParentSpouseMale, this.Helper.Translation.Get("Default.ParentSpouseMale"), this.Config.ParentSpouseFemale, this.Helper.Translation.Get("Default.ParentSpouseFemale"), this.Config.ParentSpouseOther, this.Helper.Translation.Get("Default.ParentSpouseOther")) };
                    }
                });

                //firstborn farmer reference token. authors will need to check for GNM Tokens themselves, but it will fallback to ParentFarmerOther values by default
                api.RegisterToken(this.ModManifest, "FarmerRefFirstborn", () =>
                {
                    if (this.Config.FarmerRefFirstbornSet == "")
                    {
                        return new[] { ResolvedConfigValues.ParentFarmer(this.Config.ParentFarmerOverrides, this.Config.ParentFarmerMale, this.Helper.Translation.Get("Default.ParentFarmerMale"), this.Config.ParentFarmerFemale, this.Helper.Translation.Get("Default.ParentFarmerFemale"), this.Config.ParentFarmerOther, this.Helper.Translation.Get("Default.ParentFarmerOther"), OverrideModLoaded) };
                    }
                    else
                    {
                        return new[] { this.Config.FarmerRefFirstbornSet };
                    }
                });

                //secondborn farmer reference token. authors will need to check for GNM Tokens themselves, but it will fallback to ParentFarmerOther values by default
                api.RegisterToken(this.ModManifest, "FarmerRefSecondborn", () =>
                {
                    if (this.Config.FarmerRefSecondbornSet == "")
                    {
                        return new[] { ResolvedConfigValues.ParentFarmer(this.Config.ParentFarmerOverrides, this.Config.ParentFarmerMale, this.Helper.Translation.Get("Default.ParentFarmerMale"), this.Config.ParentFarmerFemale, this.Helper.Translation.Get("Default.ParentFarmerFemale"), this.Config.ParentFarmerOther, this.Helper.Translation.Get("Default.ParentFarmerOther"), OverrideModLoaded) };
                    }
                    else
                    {
                        return new[] { this.Config.FarmerRefSecondbornSet };
                    }
                });

                //firstborn companion reference token
                api.RegisterToken(this.ModManifest, "CompanionRefFirstborn", () =>
                {
                    if (this.Config.CompanionRefFirstbornSet == "")
                    {
                        if (Game1.player.hasRoommate())
                        {
                            return new[] { ResolvedConfigValues.ParentRoommate(this.Config.RoommateOverrides, this.Config.RoommateMale, this.Helper.Translation.Get("Default.RoommateMale"), this.Config.RoommateFemale, this.Helper.Translation.Get("Default.RoommateFemale"), this.Config.RoommateOther, this.Helper.Translation.Get("Default.RoommateOther"), this.Helper.Translation.Get("Default.RoommateKrobus")) };
                        }
                        else
                        {
                            return new[] { ResolvedConfigValues.ParentSpouse(this.Config.ParentSpouseOverrides, this.Config.ParentSpouseMale, this.Helper.Translation.Get("Default.ParentSpouseMale"), this.Config.ParentSpouseFemale, this.Helper.Translation.Get("Default.ParentSpouseFemale"), this.Config.ParentSpouseOther, this.Helper.Translation.Get("Default.ParentSpouseOther")) };
                        }
                    }
                    else
                    {
                        return new[] { this.Config.CompanionRefFirstbornSet };
                    }
                });

                //secondborn companion reference token
                api.RegisterToken(this.ModManifest, "CompanionRefSecondborn", () =>
                {
                    if (this.Config.CompanionRefSecondbornSet == "")
                    {
                        if (Game1.player.hasRoommate())
                        {
                            return new[] { ResolvedConfigValues.ParentRoommate(this.Config.RoommateOverrides, this.Config.RoommateMale, this.Helper.Translation.Get("Default.RoommateMale"), this.Config.RoommateFemale, this.Helper.Translation.Get("Default.RoommateFemale"), this.Config.RoommateOther, this.Helper.Translation.Get("Default.RoommateOther"), this.Helper.Translation.Get("Default.RoommateKrobus")) };
                        }
                        else
                        {
                            return new[] { ResolvedConfigValues.ParentSpouse(this.Config.ParentSpouseOverrides, this.Config.ParentSpouseMale, this.Helper.Translation.Get("Default.ParentSpouseMale"), this.Config.ParentSpouseFemale, this.Helper.Translation.Get("Default.ParentSpouseFemale"), this.Config.ParentSpouseOther, this.Helper.Translation.Get("Default.ParentSpouseOther")) };
                        }
                    }
                    else
                    {
                        return new[] { this.Config.CompanionRefSecondbornSet };
                    }
                });


                //firstborn spouse/roommate pronoun tokens
                api.RegisterToken(this.ModManifest, "CoSubFirst", () =>
                {
                    if (this.Config.CompanionPrnsFirstbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsSubjectFirstborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpSubMale"), this.Helper.Translation.Get("Default.SpSubFemale"), this.Helper.Translation.Get("Default.SpSubOther"), this.Config.SpousePronounsSubject) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoObjFirst", () =>
                {
                    if (this.Config.CompanionPrnsFirstbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsObjectFirstborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpObjMale"), this.Helper.Translation.Get("Default.SpObjFemale"), this.Helper.Translation.Get("Default.SpObjOther"), this.Config.SpousePronounsObject) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoPDFirst", () =>
                {
                    if (this.Config.CompanionPrnsFirstbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsPosDetFirstborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpPDMale"), this.Helper.Translation.Get("Default.SpPDFemale"), this.Helper.Translation.Get("Default.SpPDOther"), this.Config.SpousePronounsPosDet) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoPosFirst", () =>
                {
                    if (this.Config.CompanionPrnsFirstbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsPossessiveFirstborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpPosMale"), this.Helper.Translation.Get("Default.SpPosFemale"), this.Helper.Translation.Get("Default.SpPosOther"), this.Config.SpousePronounsPossessive) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoRefFirst", () =>
                {
                    if (this.Config.CompanionPrnsFirstbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsReflexiveFirstborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpRefMale"), this.Helper.Translation.Get("Default.SpRefFemale"), this.Helper.Translation.Get("Default.SpRefOther"), this.Config.SpousePronounsReflexive) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoPluFirst", () =>
                {
                    if (this.Config.CompanionPrnsFirstbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsPluralityFirstborn.ToString() };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronounPlurality(this.Config.SpousePronounsEnabled, this.Config.SpousePronounsPlurality).ToString() };
                    }
                });


                //secondborn spouse/roommate pronoun tokens
                api.RegisterToken(this.ModManifest, "CoSubSecond", () =>
                {
                    if (this.Config.CompanionPrnsSecondbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsSubjectSecondborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpSubMale"), this.Helper.Translation.Get("Default.SpSubFemale"), this.Helper.Translation.Get("Default.SpSubOther"), this.Config.SpousePronounsSubject) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoObjSecond", () =>
                {
                    if (this.Config.CompanionPrnsSecondbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsObjectSecondborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpObjMale"), this.Helper.Translation.Get("Default.SpObjFemale"), this.Helper.Translation.Get("Default.SpObjOther"), this.Config.SpousePronounsObject) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoPDSecond", () =>
                {
                    if (this.Config.CompanionPrnsSecondbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsPosDetSecondborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpPDMale"), this.Helper.Translation.Get("Default.SpPDFemale"), this.Helper.Translation.Get("Default.SpPDOther"), this.Config.SpousePronounsPosDet) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoPosSecond", () =>
                {
                    if (this.Config.CompanionPrnsSecondbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsPossessiveSecondborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpPosMale"), this.Helper.Translation.Get("Default.SpPosFemale"), this.Helper.Translation.Get("Default.SpPosOther"), this.Config.SpousePronounsPossessive) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoRefSecond", () =>
                {
                    if (this.Config.CompanionPrnsSecondbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsReflexiveSecondborn };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronouns(this.Config.SpousePronounsEnabled, this.Helper.Translation.Get("Default.SpRefMale"), this.Helper.Translation.Get("Default.SpRefFemale"), this.Helper.Translation.Get("Default.SpRefOther"), this.Config.SpousePronounsReflexive) };
                    }
                });
                api.RegisterToken(this.ModManifest, "CoPluSecond", () =>
                {
                    if (this.Config.CompanionPrnsSecondbornEnabled)
                    {
                        return new[] { this.Config.CompanionPrnsPluralitySecondborn.ToString() };
                    }
                    else
                    {
                        return new[] { ResolvedConfigValues.CompanionPronounPlurality(this.Config.SpousePronounsEnabled, this.Config.SpousePronounsPlurality).ToString() };
                    }
                });


                api.RegisterToken(this.ModManifest, "FirstbornNameErrorMessage", () =>
                {
                    string FirstbornNameErrorMessage = this.Helper.Translation.Get("Default.FirstbornName.ErrorMessage");
                    return new[] { FirstbornNameErrorMessage };
                });
                api.RegisterToken(this.ModManifest, "SecondbornNameErrorMessage", () =>
                {
                    string SecondbornNameErrorMessage = this.Helper.Translation.Get("Default.SecondbornName.ErrorMessage");
                    return new[] { SecondbornNameErrorMessage };
                });




            }
            //PreviewUpdateSetup() (from old image implementation)
            UpdateFieldDataFromConfig();
            // register custom trigger type
            TriggerActionManager.RegisterTrigger("siliconmodding.GVChildrenSMAPI_OnConfigChangedEarly");
            TriggerActionManager.RegisterTrigger("siliconmodding.GVChildrenSMAPI_OnConfigChangedLate");

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => {
                    this.Helper.WriteConfig(this.Config);
                    if (Game1.gameMode != Game1.titleScreenGameMode)
                    {
                        QueueConsoleCommand.Value("patch update");
                        TriggerActionManager.Raise("siliconmodding.GVChildrenSMAPI_OnConfigChangedEarly"); // trigger can pass optional trigger arguments
                        TriggerActionManager.Raise("siliconmodding.GVChildrenSMAPI_OnConfigChangedLate"); // trigger can pass optional trigger arguments
                    }
                }
            );

            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "GVConfigPageGenericChild",
                text: () => this.Helper.Translation.Get("config.GVConfigPageGenericChild.name")
            );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.GVConfigPageGenericChild.description")
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "GVConfigPageParentalReference",
                text: () => this.Helper.Translation.Get("config.GVConfigPageParentalReference.name")
            );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.GVConfigPageParentalReference.description")
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "GVConfigPageAdvanced",
                text: () => this.Helper.Translation.Get("config.GVConfigPageAdvanced.name")
            );
            configMenu.AddParagraph(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.GVConfigPageAdvanced.description")
            );
            configMenu.AddComplexOption(
                mod: this.ModManifest,
                name: () => "",
                draw: (_, _) => { },
                height: () => 0,
                beforeMenuOpened: () =>
                {
                    UpdateFieldDataFromConfig();
                    //UpdateEliasPreview(); (from old image implementation)
                    //UpdateLizziePreview(); (from old image implementation)
                }
            );


            /*********
            ** Child customization page
            *********/
        configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "GVConfigPageGenericChild",
                pageTitle: () => this.Helper.Translation.Get("config.GVConfigPageGenericChild.name")
            );

            /*********
            ** Firstborn customization config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.FirstbornCustomization.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.FirstbornCustomization.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornHairTypeSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornHairTypeSet.description"),
                getValue: () => this.Config.FirstbornHairTypeSet,
                setValue: value => this.Config.FirstbornHairTypeSet = value,
                allowedValues: new string[] { "Default", "Fluffy", "Coily", "Spiky", "Curly", "Straight" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornHairTypeSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornHairLengthSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornHairLengthSet.description"),
                getValue: () => this.Config.FirstbornHairLengthSet,
                setValue: value => this.Config.FirstbornHairLengthSet = value,
                allowedValues: new string[] { "Shorter", "Longer" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornHairLengthSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornHairColourSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornHairColourSet.description"),
                getValue: () => this.Config.FirstbornHairColourSet,
                setValue: value => this.Config.FirstbornHairColourSet = value,
                allowedValues: new string[] { "Default", "1_Custom", "2_Custom", "Auburn", "Brown", "Black", "Dark_brown", "Copper", "Pale_blonde", "Dark_fuschia", "Light_auburn", "Ash_brown", "Pale_ash", "White", "Silver", "Deep_ginger", "Light_ginger", "Pale_ginger", "Golden_blonde", "Strawberry_blonde", "Platinum_blonde", "Yellow", "Violet", "Pink", "Light_red", "Cherry_red", "Wine_red", "Forest_green", "Emerald_green", "Pale_green", "Teal", "Sky_blue", "Ocean_blue", "Navy_blue", "Abigail", "Alex", "Caroline", "Clint", "Demetrius", "Elliott", "Emily", "Gus", "Haley", "Harvey", "Jodi", "Kent", "Leah", "Lewis", "Linus", "Marlon", "Marnie", "Maru", "Morris", "Pam", "Penny", "Pierre", "Robin", "Sam", "Sandy", "Sebastian", "Shane", "Willy", "Wizard" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornHairColourSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornEyeColourSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornEyeColourSet.description"),
                getValue: () => this.Config.FirstbornEyeColourSet,
                setValue: value => this.Config.FirstbornEyeColourSet = value,
                allowedValues: new string[] { "Default", "1_Custom", "2_Custom", "Chestnut", "Brown", "Black", "Dark_brown", "Green", "Teal", "Blue", "Purple", "Pink", "Red", "Orange", "Yellow", "Gold", "Hazel", "Grey", "Light_grey", "White" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornEyeColourSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornClothingTypeSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornClothingTypeSet.description"),
                getValue: () => this.Config.FirstbornClothingTypeSet,
                setValue: value => this.Config.FirstbornClothingTypeSet = value,
                allowedValues: new string[] { "Shirt", "Tank_top", "Round_collar", "Overalls" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornClothingTypeSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornClothingColourSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornClothingColourSet.description"),
                getValue: () => this.Config.FirstbornClothingColourSet,
                setValue: value => this.Config.FirstbornClothingColourSet = value,
                allowedValues: new string[] { "Red", "Blue", "Yellow", "Black", "White" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornClothingColourSet.values." + rawValue)
            );

            /*********
            ** Secondborn customization config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.SecondbornCustomization.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.SecondbornCustomization.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornHairTypeSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornHairTypeSet.description"),
                getValue: () => this.Config.SecondbornHairTypeSet,
                setValue: value => this.Config.SecondbornHairTypeSet = value,
                allowedValues: new string[] { "Default", "Fluffy", "Coily", "Spiky", "Curly", "Straight" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornHairTypeSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornHairLengthSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornHairLengthSet.description"),
                getValue: () => this.Config.SecondbornHairLengthSet,
                setValue: value => this.Config.SecondbornHairLengthSet = value,
                allowedValues: new string[] { "Shorter", "Longer" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornHairLengthSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornHairColourSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornHairColourSet.description"),
                getValue: () => this.Config.SecondbornHairColourSet,
                setValue: value => this.Config.SecondbornHairColourSet = value,
                allowedValues: new string[] { "Default", "1_Custom", "2_Custom", "Auburn", "Brown", "Black", "Dark_brown", "Copper", "Pale_blonde", "Dark_fuschia", "Light_auburn", "Ash_brown", "Pale_ash", "White", "Silver", "Deep_ginger", "Light_ginger", "Pale_ginger", "Golden_blonde", "Strawberry_blonde", "Platinum_blonde", "Yellow", "Violet", "Pink", "Light_red", "Cherry_red", "Wine_red", "Forest_green", "Emerald_green", "Pale_green", "Teal", "Sky_blue", "Ocean_blue", "Navy_blue", "Abigail", "Alex", "Caroline", "Clint", "Demetrius", "Elliott", "Emily", "Gus", "Haley", "Harvey", "Jodi", "Kent", "Leah", "Lewis", "Linus", "Marlon", "Marnie", "Maru", "Morris", "Pam", "Penny", "Pierre", "Robin", "Sam", "Sandy", "Sebastian", "Shane", "Willy", "Wizard" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornHairColourSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornEyeColourSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornEyeColourSet.description"),
                getValue: () => this.Config.SecondbornEyeColourSet,
                setValue: value => this.Config.SecondbornEyeColourSet = value,
                allowedValues: new string[] { "Default", "1_Custom", "2_Custom", "Chestnut", "Brown", "Black", "Dark_brown", "Green", "Teal", "Blue", "Purple", "Pink", "Red", "Orange", "Yellow", "Gold", "Hazel", "Grey", "Light_grey", "White" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornEyeColourSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornClothingTypeSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornClothingTypeSet.description"),
                getValue: () => this.Config.SecondbornClothingTypeSet,
                setValue: value => this.Config.SecondbornClothingTypeSet = value,
                allowedValues: new string[] { "Shirt", "Tank_top", "Round_collar", "Overalls" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornClothingTypeSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornClothingColourSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornClothingColourSet.description"),
                getValue: () => this.Config.SecondbornClothingColourSet,
                setValue: value => this.Config.SecondbornClothingColourSet = value,
                allowedValues: new string[] { "Red", "Blue", "Yellow", "Black", "White" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornClothingColourSet.values." + rawValue)
            );

            /*********
            ** Misc. child customization config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.MiscChildCustomization.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.MiscChildCustomization.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ChildSkinToneSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.ChildSkinToneSet.description"),
                getValue: () => this.Config.ChildSkinToneSet,
                setValue: value => this.Config.ChildSkinToneSet = value,
                allowedValues: new string[] { "Default", "Pale_Dark", "Dark_Darker" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.ChildSkinToneSet.values." + rawValue)
            );



            /*********
            ** Parental reference customization page
            *********/
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "GVConfigPageParentalReference",
                pageTitle: () => this.Helper.Translation.Get("config.GVConfigPageParentalReference.name")
            );

            /*********
            ** Farmer reference config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.ParentFarmerTitle.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.ParentFarmerTitle.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentFarmerMale.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentFarmerMale.description"),
                getValue: () => this.Config.ParentFarmerMale,
                setValue: value => this.Config.ParentFarmerMale = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentFarmerFemale.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentFarmerFemale.description"),
                getValue: () => this.Config.ParentFarmerFemale,
                setValue: value => this.Config.ParentFarmerFemale = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentFarmerOther.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentFarmerOther.description"),
                getValue: () => this.Config.ParentFarmerOther,
                setValue: value => this.Config.ParentFarmerOther = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentFarmerOverrides.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentFarmerOverrides.description"),
                getValue: () => this.Config.ParentFarmerOverrides,
                setValue: value => this.Config.ParentFarmerOverrides = value,
                allowedValues: new string[] { "None", "UseFarmerName", "UseFarmerOther" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.ParentFarmerOverrides.values." + rawValue)
            );

            /*********
            ** Spouse reference config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.ParentSpouseTitle.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.ParentSpouseTitle.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentSpouseMale.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentSpouseMale.description"),
                getValue: () => this.Config.ParentSpouseMale,
                setValue: value => this.Config.ParentSpouseMale = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentSpouseFemale.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentSpouseFemale.description"),
                getValue: () => this.Config.ParentSpouseFemale,
                setValue: value => this.Config.ParentSpouseFemale = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentSpouseOther.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentSpouseOther.description"),
                getValue: () => this.Config.ParentSpouseOther,
                setValue: value => this.Config.ParentSpouseOther = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.ParentSpouseOverrides.name"),
                tooltip: () => this.Helper.Translation.Get("config.ParentSpouseOverrides.description"),
                getValue: () => this.Config.ParentSpouseOverrides,
                setValue: value => this.Config.ParentSpouseOverrides = value,
                allowedValues: new string[] { "None", "UseSpouseName", "UseSpouseOther" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.ParentSpouseOverrides.values." + rawValue)
            );

            /*********
            ** Roommate reference config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.RoommateTitle.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.RoommateTitle.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.RoommateOverrides.name"),
                tooltip: () => this.Helper.Translation.Get("config.RoommateOverrides.description"),
                getValue: () => this.Config.RoommateOverrides,
                setValue: value => this.Config.RoommateOverrides = value,
                allowedValues: new string[] { "DefaultRoommateName", "UseRoommateTitle", "UseRoommateName", "UseRoommateOther" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.RoommateOverrides.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.RoommateMale.name"),
                tooltip: () => this.Helper.Translation.Get("config.RoommateMale.description"),
                getValue: () => this.Config.RoommateMale,
                setValue: value => this.Config.RoommateMale = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.RoommateFemale.name"),
                tooltip: () => this.Helper.Translation.Get("config.RoommateFemale.description"),
                getValue: () => this.Config.RoommateFemale,
                setValue: value => this.Config.RoommateFemale = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.RoommateOther.name"),
                tooltip: () => this.Helper.Translation.Get("config.RoommateOther.description"),
                getValue: () => this.Config.RoommateOther,
                setValue: value => this.Config.RoommateOther = value
            );

            /*********
            ** Tweaks config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.Tweaks.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.Tweaks.description")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsEnabled.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsEnabled.description"),
                getValue: () => this.Config.SpousePronounsEnabled,
                setValue: value => this.Config.SpousePronounsEnabled = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsSubject.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsSubject.description"),
                getValue: () => this.Config.SpousePronounsSubject,
                setValue: value => this.Config.SpousePronounsSubject = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsObject.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsObject.description"),
                getValue: () => this.Config.SpousePronounsObject,
                setValue: value => this.Config.SpousePronounsObject = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPosDet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPosDet.description"),
                getValue: () => this.Config.SpousePronounsPosDet,
                setValue: value => this.Config.SpousePronounsPosDet = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPossessive.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPossessive.description"),
                getValue: () => this.Config.SpousePronounsPossessive,
                setValue: value => this.Config.SpousePronounsPossessive = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsReflexive.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsReflexive.description"),
                getValue: () => this.Config.SpousePronounsReflexive,
                setValue: value => this.Config.SpousePronounsReflexive = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPlurality.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPlurality.description"),
                getValue: () => this.Config.SpousePronounsPlurality,
                setValue: value => this.Config.SpousePronounsPlurality = value
            );

            /*********
            ** Gender override config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.GenderOverrides.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.GenderOverrides.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpouseGenderOverride.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpouseGenderOverride.description"),
                getValue: () => this.Config.SpouseGenderOverride,
                setValue: value => this.Config.SpouseGenderOverride = value,
                allowedValues: new string[] { "None", "Male", "Female", "Undefined" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SpouseGenderOverride.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.RoommateGenderOverride.name"),
                tooltip: () => this.Helper.Translation.Get("config.RoommateGenderOverride.description"),
                getValue: () => this.Config.RoommateGenderOverride,
                setValue: value => this.Config.RoommateGenderOverride = value,
                allowedValues: new string[] { "None", "Male", "Female", "Undefined" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.RoommateGenderOverride.values." + rawValue)
            );


            /*********
            ** Advanced config page
            *********/
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "GVConfigPageAdvanced",
                pageTitle: () => this.Helper.Translation.Get("config.GVConfigPageAdvanced.name")
            );

            /*********
            ** Alt art config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.AlternativePortraits.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.AlternativePortraits.description")
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.EliasPromoEnabled.name"),
                tooltip: () => this.Helper.Translation.Get("config.EliasPromoEnabled.description"),
                getValue: () => this.Config.EliasPromoEnabled,
                setValue: value => this.Config.EliasPromoEnabled = value,
                fieldId: "siliconmodding.GVChildrenSMAPI_EliasPromoEnabled"
            );
            /* original image draw
            configMenu.AddImage(
                mod: this.ModManifest,
                texture: () => EliasPreview,
                texturePixelArea: new Rectangle(0, 0, 334, 93),
                scale: 3
            );
            */
            configMenu.AddComplexOption(
                mod: this.ModManifest,
                name: () => "",
                draw: (SpriteBatch b, Vector2 v) =>
                {
                    DrawPreviewElias(b, v);
                },
                height: () => 75 * 3
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.LizziePromoEnabled.name"),
                tooltip: () => this.Helper.Translation.Get("config.LizziePromoEnabled.description"),
                getValue: () => this.Config.LizziePromoEnabled,
                setValue: value => this.Config.LizziePromoEnabled = value,
                fieldId: "siliconmodding.GVChildrenSMAPI_LizziePromoEnabled"
            );
            /* original image draw
            configMenu.AddImage(
                mod: this.ModManifest,
                texture: () => LizziePreview,
                texturePixelArea: new Rectangle(0, 0, 334, 93),
                scale: 3
            );
            */
            configMenu.AddComplexOption(
                mod: this.ModManifest,
                name: () => "",
                draw: (SpriteBatch b, Vector2 v) =>
                {
                    DrawPreviewLizzie(b, v);
                },
                height: () => 75 * 3
            );

            /*********
            ** Child selection config
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.ChildSelection.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.ChildSelection.description")
            );

            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornSelect.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornSelect.description"),
                getValue: () => this.Config.FirstbornSelect,
                setValue: value => this.Config.FirstbornSelect = value,
                allowedValues: new string[] { "DefaultParentCheck", "Alex", "Elliott", "Emily", "Leah", "Penny", "Sebastian", "FarmerDefault" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornSelect.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FirstbornDialogueSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FirstbornDialogueSet.description"),
                getValue: () => this.Config.FirstbornDialogueSet,
                setValue: value => this.Config.FirstbornDialogueSet = value,
                allowedValues: new string[] { "Default", "Canonical", "Generic", "Single" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.FirstbornDialogueSet.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornSelect.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornSelect.description"),
                getValue: () => this.Config.SecondbornSelect,
                setValue: value => this.Config.SecondbornSelect = value,
                allowedValues: new string[] { "DefaultParentCheck", "Alex", "Elliott", "Emily", "Leah", "Penny", "Sebastian", "FarmerDefault" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornSelect.values." + rawValue)
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SecondbornDialogueSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SecondbornDialogueSet.description"),
                getValue: () => this.Config.SecondbornDialogueSet,
                setValue: value => this.Config.SecondbornDialogueSet = value,
                allowedValues: new string[] { "Default", "Canonical", "Generic", "Single" },
                formatAllowedValue: (string rawValue) => Helper.Translation.Get("config.SecondbornDialogueSet.values." + rawValue)
            );

            /*********
            ** Split companion reference config firstborn
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.CompanionRefSplitFirstborn.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.CompanionRefSplitFirstborn.description")
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FarmerRefFirstbornSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FarmerRefFirstbornSet.description"),
                getValue: () => this.Config.FarmerRefFirstbornSet,
                setValue: value => this.Config.FarmerRefFirstbornSet = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.CompanionRefFirstbornSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.CompanionRefFirstbornSet.description"),
                getValue: () => this.Config.CompanionRefFirstbornSet,
                setValue: value => this.Config.CompanionRefFirstbornSet = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.CompanionPrnsFirstbornEnabled.name"),
                tooltip: () => this.Helper.Translation.Get("config.CompanionPrnsFirstbornEnabled.description"),
                getValue: () => this.Config.CompanionPrnsFirstbornEnabled,
                setValue: value => this.Config.CompanionPrnsFirstbornEnabled = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsSubject.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsSubject.description"),
                getValue: () => this.Config.CompanionPrnsSubjectFirstborn,
                setValue: value => this.Config.CompanionPrnsSubjectFirstborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsObject.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsObject.description"),
                getValue: () => this.Config.CompanionPrnsObjectFirstborn,
                setValue: value => this.Config.CompanionPrnsObjectFirstborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPosDet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPosDet.description"),
                getValue: () => this.Config.CompanionPrnsPosDetFirstborn,
                setValue: value => this.Config.CompanionPrnsPosDetFirstborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPossessive.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPossessive.description"),
                getValue: () => this.Config.CompanionPrnsPossessiveFirstborn,
                setValue: value => this.Config.CompanionPrnsPossessiveFirstborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsReflexive.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsReflexive.description"),
                getValue: () => this.Config.CompanionPrnsReflexiveFirstborn,
                setValue: value => this.Config.CompanionPrnsReflexiveFirstborn = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPlurality.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPlurality.description"),
                getValue: () => this.Config.CompanionPrnsPluralityFirstborn,
                setValue: value => this.Config.CompanionPrnsPluralityFirstborn = value
            );

            /*********
            ** Split companion reference config secondborn
            *********/
            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => this.Helper.Translation.Get("config.section.CompanionRefSplitSecondborn.name"),
                tooltip: () => this.Helper.Translation.Get("config.section.CompanionRefSplitSecondborn.description")
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.FarmerRefSecondbornSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.FarmerRefSecondbornSet.description"),
                getValue: () => this.Config.FarmerRefSecondbornSet,
                setValue: value => this.Config.FarmerRefSecondbornSet = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.CompanionRefSecondbornSet.name"),
                tooltip: () => this.Helper.Translation.Get("config.CompanionRefSecondbornSet.description"),
                getValue: () => this.Config.CompanionRefSecondbornSet,
                setValue: value => this.Config.CompanionRefSecondbornSet = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.CompanionPrnsSecondbornEnabled.name"),
                tooltip: () => this.Helper.Translation.Get("config.CompanionPrnsSecondbornEnabled.description"),
                getValue: () => this.Config.CompanionPrnsSecondbornEnabled,
                setValue: value => this.Config.CompanionPrnsSecondbornEnabled = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsSubject.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsSubject.description"),
                getValue: () => this.Config.CompanionPrnsSubjectSecondborn,
                setValue: value => this.Config.CompanionPrnsSubjectSecondborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsObject.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsObject.description"),
                getValue: () => this.Config.CompanionPrnsObjectSecondborn,
                setValue: value => this.Config.CompanionPrnsObjectSecondborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPosDet.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPosDet.description"),
                getValue: () => this.Config.CompanionPrnsPosDetSecondborn,
                setValue: value => this.Config.CompanionPrnsPosDetSecondborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPossessive.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPossessive.description"),
                getValue: () => this.Config.CompanionPrnsPossessiveSecondborn,
                setValue: value => this.Config.CompanionPrnsPossessiveSecondborn = value
            );
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsReflexive.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsReflexive.description"),
                getValue: () => this.Config.CompanionPrnsReflexiveSecondborn,
                setValue: value => this.Config.CompanionPrnsReflexiveSecondborn = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("config.SpousePronounsPlurality.name"),
                tooltip: () => this.Helper.Translation.Get("config.SpousePronounsPlurality.description"),
                getValue: () => this.Config.CompanionPrnsPluralitySecondborn,
                setValue: value => this.Config.CompanionPrnsPluralitySecondborn = value
            );

            configMenu.OnFieldChanged(
                mod: this.ModManifest, 
                (fieldId, value) =>
                {
                    switch (fieldId)
                    {
                        case "siliconmodding.GVChildrenSMAPI_EliasPromoEnabled":
                            {
                                liveFieldData[fieldId] = value;
                                //UpdateEliasPreview(); (from old image implementation)
                            }
                            break;
                        case "siliconmodding.GVChildrenSMAPI_LizziePromoEnabled":
                            {
                                liveFieldData[fieldId] = value;
                                //UpdateLizziePreview(); (from old image implementation)
                            }
                            break;
                    }
                }
            );
        }
    }
}

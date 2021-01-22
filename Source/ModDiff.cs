﻿using HarmonyLib;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ModDiff
{
    public class Settings : ModSettings
    {
       
        public bool ignoreSelf = false;
        public bool selfPreservation = true;
        public bool alternativePallete = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref ignoreSelf, "ignoreSelf", false);
            Scribe_Values.Look(ref selfPreservation, "selfPreservation", true);
            Scribe_Values.Look(ref alternativePallete, "alternativePallete", false);

            base.ExposeData();

            ModDiffCell.NeedInitStyles = true;
        }

        //public string[] LockedMods = {
        //    "dubwise.dubsperformanceanalyzer",
        //    "automatic.startupimpact",
        //    "unlimitedhugs.hugslib" 
        //};
    }


    public class ModDiff : CMod
    {
        public static string PackageIdOfMine = null;
        public static Settings Settings { get; private set; }

        private static bool debug = false;

        static string commitInfo = null;
        public static string CommitInfo => debug ? (commitInfo + "-dev") : commitInfo;
        public static bool CassowaryPackaged = true;

        public ModDiff(ModContentPack content) : base(content)
        {
            ReadModInfo(content);
            Settings = GetSettings<Settings>();

            Harmony harmony = new Harmony(PackageIdOfMine);

            harmony.Patch(AccessTools.Method(typeof(ScribeMetaHeaderUtility), "TryCreateDialogsForVersionMismatchWarnings"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), "TryCreateDialogsForVersionMismatchWarnings_Prefix"));

            RWLayoutHarmonyPatches.PatchOnce();
        }

        private static void ReadModInfo(ModContentPack content)
        {
            PackageIdOfMine = content.PackageId;

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(name + ".git.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    commitInfo = reader.ReadToEnd()?.TrimEndNewlines();
                }
            }
            catch
            {
                commitInfo = null;
            }

            debug = PackageIdOfMine.EndsWith(".dev");
        }

        public override string SettingsCategory()
        {
            return "Mod Diff";
        }


        public override void ConstructGui()
        {
            Gui.StackTop(StackOptions.Create(intrinsicIfNotSet: true, constrainEnd: false),
             Gui.AddElement(new CCheckboxLabeled
             {
                 Title = "IgnoreSelfTitle".Translate(),
                 Checked = Settings.ignoreSelf,
                 Changed = (_, value) => Settings.ignoreSelf = value,
             }), 2,
             Gui.AddElement(new CCheckboxLabeled
             {
                 Title = "KeepSelfLoaded".Translate(),
                 Checked = Settings.selfPreservation,
                 Changed = (_, value) => Settings.selfPreservation = value,
             }), 2,
             Gui.AddElement(new CCheckboxLabeled
             {
                 Title = "AlternativePalette".Translate(),
                 Checked = Settings.alternativePallete,
                 Changed = (_, value) => Settings.alternativePallete = value,
             })
            );

            var footer = Gui.AddElement(new CLabel
            {
                Title = $"Version: {CommitInfo}",
                TextAlignment = TextAnchor.LowerRight,
                Color = new Color(1, 1, 1, 0.5f),
                Font = GameFont.Tiny
            });

            Gui.AddConstraints(
                footer.top ^ Gui.bottom + 3,
                footer.width ^ footer.intrinsicWidth,
                footer.right ^ Gui.right,
                footer.height ^ footer.intrinsicHeight);

        }
        /*
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);

            var options = new Listing_Standard();
            options.maxOneColumn = true;

            options.Begin(inRect);

            options.CheckboxLabeled("IgnoreSelfTitle".Translate(), ref Settings.ignoreSelf, "IgnoreSelfHint".Translate());
            options.CheckboxLabeled("KeepSelfLoaded".Translate(), ref Settings.selfPreservation);
            options.CheckboxLabeled("AlternativePalette".Translate(), ref Settings.alternativePallete);

            options.End();
        }
        */

    }



}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diff;
using RimWorld;
using Verse;

namespace ModDiff
{
    public class ModDiffModel
    {
        public ModInfo[] saveMods;
        public ModInfo[] runningMods;
        public List<Change<ModInfo>> info;

        public void CalculateDiff()
        {
            CalculateDiff(saveMods, runningMods);
        }

        public bool HaveMissingMods = false;

        private void CalculateDiff(ModInfo[] saveMods, ModInfo[] runningMods)
        {
            var diff = new Myers<ModInfo>(saveMods, runningMods);
            diff.Compute();

            info = diff.changeSet;

            var moved = info.Where(x => x.change == ChangeType.Removed).Select(x => x.value).ToHashSet();
            moved.IntersectWith(info.Where(x => x.change == ChangeType.Added).Select(x => x.value));

            foreach (var change in diff.changeSet)
            {
                if (moved.Contains(change.value))
                {
                    change.value.isMoved = true;
                }

                if (ModLister.GetModWithIdentifier(change.value.packageId, false) == null)
                {
                    change.value.isMissing = true;
                    HaveMissingMods = true;
                }
            }
        }


        static string harmonyId = "brrainz.harmony";

        public void TrySetActiveMods()
        {
            var loadedModIdsList = new List<string>(ScribeMetaHeaderUtility.loadedModIdsList);

            if (ModDiff.settings.selfPreservation && !loadedModIdsList.Contains(ModDiff.packageIdOfMine))
            {
                
                var index = loadedModIdsList.IndexOf(harmonyId);

                if (index != -1)
                {
                    loadedModIdsList.Insert(index + 1, ModDiff.packageIdOfMine);
                }
                else
                {
                    loadedModIdsList.Insert(0, harmonyId);
                    loadedModIdsList.Insert(1, ModDiff.packageIdOfMine);
                }
            }

            if (Current.ProgramState == ProgramState.Entry)
            {
                ModsConfig.SetActiveToList(loadedModIdsList);
            }
            ModsConfig.SaveFromList(loadedModIdsList);

            // "MissingMods".Translate(),
            /*IEnumerable<string> enumerable = Enumerable
                .Range(0, ScribeMetaHeaderUtility.loadedModIdsList.Count)
                .Where((int id) => ModLister.GetModWithIdentifier(ScribeMetaHeaderUtility.loadedModIdsList[id], false) == null)
                .Select((int id) => ScribeMetaHeaderUtility.loadedModNamesList[id]);
            */

            ModsConfig.RestartFromChangedMods();
        }

    }
}

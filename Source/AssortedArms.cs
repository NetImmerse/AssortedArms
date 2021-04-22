using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AssortedArms
{
	public class AssortedArms_Settings : ModSettings
	{
		public Dictionary<string, bool> weaponStates = new Dictionary<string, bool>();
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref weaponStates, "psycastStates", LookMode.Value, LookMode.Value, ref psycastKeys, ref boolValues);
		}

		private List<string> psycastKeys;
		private List<bool> boolValues;

		public void DoSettingsWindowContents(Rect inRect)
		{
			var keys = weaponStates.Keys.ToList().OrderByDescending(x => x).ToList();
			Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
			Rect rect2 = new Rect(0f, 0f, inRect.width - 600f, keys.Count * 24);
			Widgets.BeginScrollView(rect, ref scrollPosition, rect2, true);
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(rect2);
			for (int num = keys.Count - 1; num >= 0; num--)
			{
				var test = weaponStates[keys[num]];
				listingStandard.CheckboxLabeled(keys[num], ref test);
				weaponStates[keys[num]] = test;
			}
			listingStandard.End();
			Widgets.EndScrollView();
			base.Write();
		}
		private static Vector2 scrollPosition = Vector2.zero;
	}

	public class AssortedArms_Mod : Mod
	{
		public static AssortedArms_Settings settings;
		public AssortedArms_Mod(ModContentPack content) : base(content)
		{
			settings = GetSettings<AssortedArms_Settings>();
		}
		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);
			var weapons = DefDatabase<ThingDef>.AllDefsListForReading;
			foreach (var weapon in weapons)
			{
				if (settings.weaponStates == null) settings.weaponStates = new Dictionary<string, bool>();
				if (!settings.weaponStates.ContainsKey(weapon.defName))
				{
					settings.weaponStates[weapon.defName] = true;
				}
			}
			settings.DoSettingsWindowContents(inRect);
		}
		public override string SettingsCategory()
		{
			return "Assorted Arms";
		}

		public override void WriteSettings()
		{
			base.WriteSettings();
			DefsRemover.DoDefsRemoval();
		}
	}
	[StaticConstructorOnStartup]
	public static class DefsRemover
	{
		static DefsRemover()
		{
			DoDefsRemoval();
		}
		public static void RemoveDef(ThingDef def)
		{
			try
			{
				if (DefDatabase<ThingDef>.AllDefsListForReading.Contains(def))
				{
					DefDatabase<ThingDef>.AllDefsListForReading.Remove(def);
				}
			}
			catch { };
		}
		public static void DoDefsRemoval()
		{
			foreach (var psycastState in AssortedArms_Mod.settings.weaponStates)
			{
				if (!psycastState.Value)
				{
					var defToRemove = DefDatabase<ThingDef>.GetNamedSilentFail(psycastState.Key);
					if (defToRemove != null)
					{
						RemoveDef(defToRemove);
					}
				}
			}
		}
	}
}
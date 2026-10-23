namespace HandOfFate2Access.Localization {
	/// <summary>
	/// Descriptions of the character-customisation options the game presents only as
	/// card art (hairstyles, outfit colours), keyed by the card's art name. Written from
	/// the card art itself. Several art names share one look, so they share a description.
	/// Null for art with no description yet; the caller falls back to a numbered option.
	/// </summary>
	public static class ArtDescriptions {
		public static string Describe(string artName) {
			if (string.IsNullOrEmpty(artName)) return null;
			string key = artName.ToLowerInvariant();
			int slash = key.LastIndexOfAny(new[] { '/', '\\' });
			if (slash >= 0) key = key.Substring(slash + 1);
			// Art is referenced as "bundle:Path/To/c_cus_name.png:guid"; keep the file
			// name without its extension.
			int dot = key.IndexOf('.');
			if (dot >= 0) key = key.Substring(0, dot);
			if (key.StartsWith("c_cus_")) key = key.Substring(6);
			StringTable t = Strings.Active;
			switch (key) {
				case "m_afro_hair_01": return t.StyleShaved;
				case "m_afro_hair_02": return t.StyleShortCrop;
				case "m_afro_hair_03": return t.StyleCurlyTop;
				case "m_asia_hair_01": return t.StyleTiedBack;
				case "m_asia_hair_02": return t.StyleHighBun;
				case "m_asia_hair_03": return t.StyleSweptBack;
				case "m_cauc_hair_01": return t.StyleBuzzCut;
				case "m_cauc_hair_02": return t.StyleShortSpiky;
				case "m_cauc_hair_03": return t.StyleMessyFringe;
				case "m_hair_bald":
				case "f_hair_bald": return t.StyleBald;
				case "m_hair_hood_hof1":
				case "f_hair_hood_hof1": return t.StyleHoodBlueMask;
				case "m_hair_hood_hof2":
				case "f_hair_hood_hof2": return t.StyleHoodSandMask;
				case "m_hair_mohawk": return t.StyleMohawk;
				case "m_hair_samurai": return t.StyleTopknot;
				case "m_hair_slickback": return t.StyleSlickedBack;
				case "m_hisp_hair_01": return t.StyleShortNeat;
				case "m_hisp_hair_02": return t.StyleWavyFringe;
				case "m_hisp_hair_03": return t.StyleCombedBack;
				case "m_war_hair_01": return t.StyleWildBeard;
				case "m_war_hair_02":
				case "f_war_01": return t.StyleSpikedCrest;
				case "m_war_hair_03": return t.StyleSpikedTufts;
				case "f_afro_hair_01":
				case "f_asia_hair_01":
				case "f_cauc_hair_01":
				case "f_hisp_hair_01":
				case "f_hair_starter": return t.StyleBraidedCrest;
				case "f_afro_hair_02": return t.StyleCornrows;
				case "f_afro_hair_03":
				case "f_asia_hair_02": return t.StyleCurlyBun;
				case "f_asia_hair_03": return t.StyleTwoBuns;
				case "f_cauc_hair_02": return t.StyleShortBraidedCrest;
				case "f_cauc_hair_03":
				case "f_hair_wild": return t.StyleBraidedKnot;
				case "f_hair_short_tail": return t.StyleBraidedShortTail;
				case "f_hair_ponytail": return t.StyleBraidedPonytail;
				case "f_hair_hood_blue": return t.StyleHoodBlue;
				case "f_hair_hood_brown": return t.StyleHoodBrown;
				case "f_hair_hood_green": return t.StyleHoodDarkRed;
				case "f_hair_hood_white": return t.StyleHoodGrey;
				case "f_hisp_hair_02": return t.StyleLongWavy;
				case "f_hisp_hair_03": return t.StyleWavyBun;
				case "f_war_02": return t.StyleBraidedSpikes;
				case "f_war_03": return t.StyleChoppy;
				case "colour_1_woodland": return t.ColourWoodland;
				case "colour_2_ranger": return t.ColourRanger;
				case "colour_3_crusade": return t.ColourCrusade;
				case "colour_4_twilight": return t.ColourTwilight;
				case "colour_5_dealer_hof1": return t.ColourNavyRed;
				case "colour_6_dealer_hof2": return t.ColourCreamGold;
				default: return null;
			}
		}
	}
}

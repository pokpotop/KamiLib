﻿using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace KamiLib.Classes;

public static class WondrousTailsTaskResolver {
		public static List<uint> GetTerritoriesFromOrderId(IDataManager dataManger, uint orderDataId) {
		var bingoOrderData = dataManger.GetExcelSheet<WeeklyBingoOrderData>().GetRow(orderDataId);

        switch (bingoOrderData.Type) {
			// Specific Duty
			case 0:
				return dataManger.GetExcelSheet<ContentFinderCondition>()
					.Where(c => c.Content.RowId == bingoOrderData.Data.RowId)
					.OrderBy(row => row.SortKey)
					.Select(c => c.TerritoryType.RowId)
					.ToList();

			// Specific Level Dungeon
			case 1:
				return dataManger.GetExcelSheet<ContentFinderCondition>()
					.Where(m => m.ContentType.RowId is 2)
					.Where(m => m.ClassJobLevelRequired == bingoOrderData.Data.RowId)
					.OrderBy(row => row.SortKey)
					.Select(m => m.TerritoryType.RowId)
					.ToList();

            // Level Range Dungeon: Dungeons (Lv. 1-49) or Dungeons (Lv. 51-59)
            case 2:
				return dataManger.GetExcelSheet<ContentFinderCondition>()
					.Where(m => m.ContentType.RowId is 2)
					.Where(m => (bingoOrderData.Data.RowId - (bingoOrderData.Data.RowId > 50 ? 8 : 48) <= m.ClassJobLevelRequired) && m.ClassJobLevelRequired <= bingoOrderData.Data.RowId)
					.OrderBy(row => row.SortKey)
					.Select(m => m.TerritoryType.RowId)
					.ToList();

			// Special categories
			case 3:
                return bingoOrderData.Unknown1 switch {
					// Treasure Map Instances are Not Supported
					1 => [],

					// PvP Categories are Not Supported
					2 => [],

					// Deep Dungeons
					3 => dataManger.GetExcelSheet<ContentFinderCondition>()
						.Where(m => m.ContentType.RowId is 21)
						.OrderBy(row => row.SortKey)
						.Select(m => m.TerritoryType.RowId)
						.ToList(),

					_ => [],
				};

			// Multi-instance raids
			case 4:
				return bingoOrderData.Data.RowId switch {
					// Binding Coil, Second Coil, Final Coil
					2 => [ 241, 242, 243, 244, 245 ],
					3 => [ 355, 356, 357, 358 ],
					4 => [ 193, 194, 195, 196 ],

					// Gordias, Midas, The Creator
					5 => [ 442, 443, 444, 445 ],
					6 => [ 520, 521, 522, 523 ],
					7 => [ 580, 581, 582, 583 ],

					// Deltascape, Sigmascape, Alphascape
					8 => [ 691, 692, 693, 694 ],
					9 => [ 748, 749, 750, 751 ],
					10 => [ 798, 799, 800, 801 ],

					// Eden's Gate: Resurrection or Descent
					11 => [ 849, 850 ],
					// Eden's Gate: Inundation or Sepulture
					12 => [ 851, 852 ],
					// Eden's Verse: Fulmination or Furor
					13 => [ 902, 903 ],
					// Eden's Verse: Iconoclasm or Refulgence
					14 => [ 904, 905 ],
					// Eden's Promise: Umbra or Litany
					15 => [ 942, 943 ],
					// Eden's Promise: Anamorphosis or Eternity
					16 => [ 944, 945 ],

					// Asphodelos: First or Second Circles
					17 => [ 1002, 1004 ],
					// Asphodelos: Third or Fourth Circles
					18 => [ 1006, 1008 ],
					// Abyssos: Fifth or Sixth Circles
					19 => [ 1081, 1083 ],
					// Abyssos: Seventh or Eight Circles
					20 => [ 1085, 1087 ],
					// Anabaseios: Ninth or Tenth Circles
					21 => [ 1147, 1149 ],
					// Anabaseios: Eleventh or Twelwth Circles
					22 => [ 1151, 1153 ],

					// Eden's Gate
					23 => [ 849, 850, 851, 852 ],
					// Eden's Verse
					24 => [ 902, 903, 904, 905 ],
					// Eden's Promise
					25 => [ 942, 943, 944, 945 ],

					// Alliance Raids (A Realm Reborn)
					26 => [ 174, 372, 151 ],
					// Alliance Raids (Heavensward)
					27 => [ 508, 556, 627 ],
					// Alliance Raids (Stormblood)
					28 => [ 734, 776, 826 ],
					// Alliance Raids (Shadowbringers)
					29 => [ 882, 917, 966 ],
					// Alliance Raids (Endwalker)
					30 => [ 1054, 1118, 1178 ],

                    // Asphodelos: First to Fourth Circles
					31 => [ 1002, 1004, 1006, 1008 ],

                    // Abyssos: Fifth to Eighth Circles
					32 => [ 1081, 1083, 1085, 1086 ],

                    // Anabaseios: Ninth to Twelfth Circles
					33 => [ 1147, 1149, 1151, 1153 ],

                    // AAC Light-heavyweight M1 or M2
					34 => [ 1225, 1227 ],

                    // AAC Light-heavyweight M3 or M4
					35 => [ 1229, 1231 ],

					// Future: Jeuno: The First Walk
					// ?? => [1248],

                    _ => [],
				};

			// Leveling Dungeons
			case 5:
				return bingoOrderData.Data.RowId switch
				{
                    // Leveling Dungeons (Lv. 1-49)
                    49 => dataManger.GetExcelSheet<ContentFinderCondition>()
                        .Where(m => m.ContentType.RowId is 2)
                        .Where(m => m.ClassJobLevelRequired <= bingoOrderData.Data.RowId)
                        .OrderBy(row => row.SortKey)
                        .Select(m => m.TerritoryType.RowId)
                        .ToList(),

                    // Leveling Dungeons (Lv. 51-59)
                    59 => dataManger.GetExcelSheet<ContentFinderCondition>()
                        .Where(m => m.ContentType.RowId is 2)
                        .Where(m => (m.ClassJobLevelRequired % 10 != 0) && 51 <= m.ClassJobLevelRequired && m.ClassJobLevelRequired <= bingoOrderData.Data.RowId)
                        .OrderBy(row => row.SortKey)
                        .Select(m => m.TerritoryType.RowId)
                        .ToList(),

                    // Leveling Dungeons (Lv. 51-79)
                    79 => dataManger.GetExcelSheet<ContentFinderCondition>()
                        .Where(m => m.ContentType.RowId is 2)
                        .Where(m => (m.ClassJobLevelRequired % 10 != 0) && 51 <= m.ClassJobLevelRequired && m.ClassJobLevelRequired <= bingoOrderData.Data.RowId)
                        .OrderBy(row => row.SortKey)
                        .Select(m => m.TerritoryType.RowId)
                        .ToList(),

                    // Leveling Dungeons (Lv. 81-99)
                    99 => dataManger.GetExcelSheet<ContentFinderCondition>()
						.Where(m => m.ContentType.RowId is 2)
						.Where(m => (m.ClassJobLevelRequired % 10 != 0) && 81 <= m.ClassJobLevelRequired && m.ClassJobLevelRequired <= bingoOrderData.Data.RowId)
						.OrderBy(row => row.SortKey)
						.Select(m => m.TerritoryType.RowId)
						.ToList(),

                    _ => [],
                };

            // High Level Dungeons
            case 6:
                return bingoOrderData.Data.RowId switch {
                    // High-level Dungeons (lv.50)
                    50 => dataManger.GetExcelSheet<ContentFinderCondition>()
                    .Where(m => m.ContentType.RowId is 2)
                    .Where(m => m.ClassJobLevelRequired == 50)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.RowId)
                    .ToList(),

                    // High-level Dungeons (lv.60) also does High-level Dungeons (lv.50-60)
                    60 => dataManger.GetExcelSheet<ContentFinderCondition>()
                    .Where(m => m.ContentType.RowId is 2)
                    .Where(m => m.ClassJobLevelRequired == 60 || (m.ClassJobLevelRequired == 50 && "High-level Dungeons (Lv. 50-60)".Equals(bingoOrderData.Text.Value.Description.ToString())))
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.RowId)
                    .ToList(),

                    // High-level Dungeons (lv.70-80)
                    80 => dataManger.GetExcelSheet<ContentFinderCondition>()
                    .Where(m => m.ContentType.RowId is 2)
                    .Where(m => m.ClassJobLevelRequired == 70 || m.ClassJobLevelRequired == 80)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.RowId)
                    .ToList(),

                    // High-level Dungeons (lv.90)
                    90 => dataManger.GetExcelSheet<ContentFinderCondition>()
                    .Where(m => m.ContentType.RowId is 2)
                    .Where(m => m.ClassJobLevelRequired == 90)
                    .OrderBy(row => row.SortKey)
                    .Select(m => m.TerritoryType.RowId)
                    .ToList(),

                    _ => [],
				};          

            // Trials
            case 7:
				return [];
			
			// Alliance Raids
			case 8:
				return [];
			
			// Normal Raids
			case 9:
				return [];
		}
        
		return [];
	}
}
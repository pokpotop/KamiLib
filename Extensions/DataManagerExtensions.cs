﻿using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Extensions;

public enum DutyType {
	Unknown,
	Savage,
	Ultimate,
	Extreme,
	Unreal,
	Criterion,
	Alliance,
}

public static class DataManagerExtensions {
	public static IEnumerable<ContentFinderCondition> GetSavageDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.Korean)?
			   .Where(cfc => cfc is { ContentType.Row: 5 })
			   .Where(cfc => cfc.Name.RawString.Contains("(영웅)")) ?? [];

	public static IEnumerable<ContentFinderCondition> GetUltimateDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()?
			   .Where(cfc => cfc is { ContentType.Row: 28 }) ?? [];

	public static IEnumerable<ContentFinderCondition> GetExtremeDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.Korean)?
			   .Where(cfc => cfc is { ContentType.Row: 4, HighEndDuty: false })
			   .Where(cfc => cfc.Name.RawString.Contains("극 ") || cfc.Name.ToString().Contains("종극의") || cfc.Name.ToString().Contains("궁극의 환상")) ?? [];

	public static IEnumerable<ContentFinderCondition> GetUnrealDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.Korean)?
			   .Where(cfc => cfc is { ContentType.Row: 4, HighEndDuty: true }) ?? [];

	public static IEnumerable<ContentFinderCondition> GetCriterionDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()?
			   .Where(cfc => cfc is { ContentType.Row: 30, AllowUndersized: false }) ?? [];

	public static IEnumerable<ContentFinderCondition> GetAllianceDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()?
			   .Where(cfc => cfc is { ContentType.Row: 5, ContentMemberType.Row: 4 }) ?? [];

	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedAllianceDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => cfc is { ContentType.Row: 5, ContentMemberType.Row: 4 });

	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedSavageDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => cfc is { ContentType.Row: 5 })
			.Where(cfc => cfc.Name.RawString.Contains("(영웅)"));
    
	private static IEnumerable<ContentFinderCondition> GetLimitedDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()?
			   .Where(cfc => dataManager.GetExcelSheet<InstanceContent>()?
				                 .Where(instanceContent => instanceContent is { WeekRestriction: 1 })
				                 .Select(instanceContent => instanceContent.RowId)
				                 .Contains(cfc.Content) ?? false) ?? [];

	public static DutyType GetDutyType(this IDataManager dataManager, ContentFinderCondition cfc) {
		var englishCfc = dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.Korean)!.GetRow(cfc.RowId);

		return englishCfc switch {
			{ ContentType.Row: 5 } when englishCfc.Name.ToString().Contains("(영웅)") => DutyType.Savage,
			{ ContentType.Row: 28 } => DutyType.Ultimate,
			{ ContentType.Row: 4, HighEndDuty: false } when englishCfc.Name.ToString().Contains("극 ") || englishCfc.Name.ToString().Contains("종극의") || englishCfc.Name.ToString().Contains("궁극의 환상") => DutyType.Extreme,
			{ ContentType.Row: 4, HighEndDuty: true } => DutyType.Unreal,
			{ ContentType.Row: 30, AllowUndersized: false } => DutyType.Criterion,
			{ ContentType.Row: 5, ContentMemberType.Row: 4 } => DutyType.Alliance,
			_ => DutyType.Unknown,
		};
	}

	public static unsafe DutyType GetCurrentDutyType(this IDataManager dataManager) {
		var cfc = dataManager.GetExcelSheet<ContentFinderCondition>()!.GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
		if (cfc is null) return DutyType.Unknown;

		return dataManager.GetDutyType(cfc);
	}
}
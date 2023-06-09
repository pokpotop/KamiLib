﻿using KamiLib.Commands.Abstracts;

namespace KamiLib.Commands.temp;

/// <summary>
/// Simple Commands with No Arguments<br/>
/// Example: <b>/dailyduty</b>
/// </summary>
public class BaseCommandHandler : CommandAttribute
{
    public BaseCommandHandler(string helpText) : base(helpText) { }
}
﻿using ChatBotPrime.Core.Interfaces.Chat;
using ChatBotPrime.Core.Interfaces.Stream;
using System;
using Message = ChatBotPrime.Core.Events.EventArguments.ChatMessage;

namespace ChatBotPrime.Core.Services.CommandHandler.Commands
{
	public class UpTimeCommand : StreamCommand
	{
		public override string CommandText => "Uptime";
		public override TimeSpan Cooldown => TimeSpan.FromSeconds(300);

		public override string Response(IChatService streamService, Message chatMessage)
		{
			if (CanRun())
			{
				var service = (IStreamService)streamService;

				var upTime = service.UpTime();

				SetLastRun();

				if (upTime.ToLower() == "offline")
				{
					return $"The Stream is currently offline and has no uptime.";
				}

				return $"The Stream has been running for { upTime }";
			}

			return $"Command is on cooldown please wait {GetTimeToRun()}";
		}
	}
}

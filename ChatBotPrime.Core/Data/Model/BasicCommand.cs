﻿using ChatBotPrime.Core.Interfaces.Chat;
using ChatBotPrime.Core.Services.CommandHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using Message = ChatBotPrime.Core.Events.EventArguments.ChatMessage;
using ChatBotPrime.Core.Extensions;

namespace ChatBotPrime.Core.Data.Model
{
	public class BasicCommand	: DataEntity , IChatCommand
	{
		public BasicCommand()
		{ }
		public BasicCommand(string commandText, string response, bool isAllowedToRun = true  )
		{
			CommandText = commandText;
			Response = response;
			IsAllowedToRun = IsAllowedToRun;
		}
			
		public TimeSpan Cooldown { get; set; } = TimeSpan.Zero;
		public virtual ICollection<CommandAlias> Aliases { get; set; }

		public string CommandText { get; private set; }

		public bool IsAllowedToRun { get; private set; }

		public DateTime LastRun { get; set; }

		public bool IsMatch(string command)
		{
			return command.Equals(CommandText, StringComparison.InvariantCultureIgnoreCase) || Aliases.Where(a => a.Word.Equals(command,StringComparison.InvariantCultureIgnoreCase)).Any();
		}

		public string Response { get; private set; }

		string IChatCommand.Response(IChatService service,Message chatMessage)
		{
			if (CanRun())
			{
				IEnumerable<string> findTokens = Response.FindTokens();
				string textToSend = ReplaceTokens(Response, findTokens, chatMessage);
				SetLastRun();
				return textToSend;
			}
			return $"Command is on cooldown please wait {GetTimeToRun()}";
		}

		private string ReplaceTokens(string textToSend, IEnumerable<string> tokens, Message chatMessage)
		{
			string newText = textToSend;
			var replaceableTokens = TokenReplacer.ListAll.Where(x => tokens.Contains(x.ReplacementToken));
			foreach (var rt in replaceableTokens)
			{
				newText = rt.ReplaceValues(newText, chatMessage);
			}

			return newText;
		}

		private   string GetTimeToRun()
		{
			var time = (Cooldown - (DateTime.UtcNow - LastRun));
			if (time.Minutes > 0)
			{
				return $"{time.Minutes}M{time.Seconds}S";
			}
			 else
			{
				return $"{time.Seconds}S";
			}

		}

		private   bool CanRun()
		{
			if (Cooldown == TimeSpan.Zero)
				return true;

			return DateTime.UtcNow - LastRun <= Cooldown;
		}

		private  void SetLastRun()
		{
			LastRun = DateTime.UtcNow;
		}
	}
}

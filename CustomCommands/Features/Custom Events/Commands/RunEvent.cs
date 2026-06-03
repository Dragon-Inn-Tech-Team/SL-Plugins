using CommandSystem;
using RedRightHand;
using RedRightHand.Commands;
using System;

namespace CustomCommands.Features.CustomEvents.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RunEvent : ICustomCommand
	{
		public string Command => "event";

		public string[] Aliases => null;

		public string Description => "Queue an event to run next round (Or cancel the currently queued event)";

		public string[] Usage { get; } = { "event" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.event";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _) || !arguments.TryGetCommandArgument(0, string.Empty, out var eventType, out response))
				return false;

			if (EventManager.QueueEvent(eventType))
			{
				response = $"{eventType} has been queued for the next round";
				return true;
			}

			response = $"No event found for {eventType} ";
			return false;
		}
	}
}

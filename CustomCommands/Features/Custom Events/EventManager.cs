using CustomCommands.Core;
using CustomCommands.Core.Event;
using CustomCommands.Features.CustomEvents.Events;
using LabApi.Events.Arguments.ObjectiveEvents;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp0492Events;
using LabApi.Events.Arguments.Scp049Events;
using LabApi.Events.Arguments.Scp079Events;
using LabApi.Events.Arguments.Scp096Events;
using LabApi.Events.Arguments.Scp106Events;
using LabApi.Events.Arguments.Scp127Events;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Arguments.Scp3114Events;
using LabApi.Events.Arguments.Scp914Events;
using LabApi.Events.Arguments.Scp939Events;
using LabApi.Events.Arguments.ScpEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MapGeneration;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.CustomEvents
{
	public class EventManager : CustomFeatureBase
	{
		private static Dictionary<string, CustomEventRound> EventHandlers = new Dictionary<string, CustomEventRound>()
		{
			{ "hideandseek", new HeavyHideAndSeek() },
			{ "oitc", new OneInTheChamber() },
			{ "infection", new SCPInfection() },
			{ "tdm", new TeamDeathMatch() },
			{ "potato", new HotPotato() }
		};

		private static KeyValuePair<string, CustomEventRound>? QueuedEvent = null;
		public static bool EventInProgress => CurrentEvent == null && (Round.IsRoundStarted && !Round.IsRoundEnded);
		private static CustomEventRound CurrentEvent;

		public EventManager(bool configSetting) : base(configSetting)
		{
		}

		public static bool QueueEvent(string type)
		{
			if (EventHandlers.ContainsKey(type))
			{
				QueuedEvent = new KeyValuePair<string, CustomEventRound>(type, EventHandlers[type]);

				if (!Round.IsRoundStarted)
					StartEvent();

				return true;
			}
			return false;
		}

		public static void StartEvent()
		{
			if (QueuedEvent.HasValue)
			{
				Server.SendBroadcast($"The {QueuedEvent.Value.Key} event will run this round", 5);

				//Round.IsLobbyLocked = true;
				CurrentEvent = QueuedEvent.Value.Value;

				//MEC.Timing.CallDelayed(15f, () =>
				//{
				//	Round.IsLobbyLocked = false;
				//});

				QueuedEvent = null;
			}
			else
				CurrentEvent = null;
		}

		public override void OnServerWaitingForPlayers() => StartEvent();

		public static Room GetRandomRoom(params RoomName[] rooms)
		{
			var roomIndex = 0;
			if (rooms.Length > 1)
				roomIndex = new System.Random().Next(rooms.Length);

			return Room.Get(rooms[roomIndex]).First();
		}

		public override void OnObjectiveCompleting(ObjectiveCompletingBaseEventArgs ev) => CurrentEvent?.OnObjectiveCompleting(ev);
		public override void OnObjectiveCompleted(ObjectiveCompletedBaseEventArgs ev) => CurrentEvent?.OnObjectiveCompleted(ev);
		public override void OnObjectiveKillingEnemyCompleting(EnemyKillingObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveKillingEnemyCompleting(ev);
		public override void OnObjectiveKilledEnemyCompleted(EnemyKilledObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveKilledEnemyCompleted(ev);
		public override void OnObjectiveEscapingCompleting(EscapingObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveEscapingCompleting(ev);
		public override void OnObjectiveEscapedCompleted(EscapedObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveEscapedCompleted(ev);
		public override void OnObjectiveActivatingGeneratorCompleting(GeneratorActivatingObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveActivatingGeneratorCompleting(ev);
		public override void OnObjectiveActivatedGeneratorCompleted(GeneratorActivatedObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveActivatedGeneratorCompleted(ev);
		public override void OnObjectiveDamagingScpCompleting(ScpDamagingObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveDamagingScpCompleting(ev);
		public override void OnObjectiveDamagedScpCompleted(ScpDamagedObjectiveEventArgs ev) => CurrentEvent?.OnObjectiveDamagedScpCompleted(ev);
		public override void OnObjectivePickingScpItemCompleting(ScpItemPickingObjectiveEventArgs ev) => CurrentEvent?.OnObjectivePickingScpItemCompleting(ev);
		public override void OnObjectivePickedScpItemCompleted(ScpItemPickedObjectiveEventArgs ev) => CurrentEvent?.OnObjectivePickedScpItemCompleted(ev);
		public override void OnPlayerJoined(PlayerJoinedEventArgs ev) => CurrentEvent?.OnPlayerJoined(ev);
		public override void OnPlayerLeft(PlayerLeftEventArgs ev) => CurrentEvent?.OnPlayerLeft(ev);
		public override void OnPlayerReceivingVoiceMessage(PlayerReceivingVoiceMessageEventArgs ev) => CurrentEvent?.OnPlayerReceivingVoiceMessage(ev);
		public override void OnPlayerSendingVoiceMessage(PlayerSendingVoiceMessageEventArgs ev) => CurrentEvent?.OnPlayerSendingVoiceMessage(ev);
		public override void OnPlayerPreAuthenticating(PlayerPreAuthenticatingEventArgs ev) => CurrentEvent?.OnPlayerPreAuthenticating(ev);
		public override void OnPlayerPreAuthenticated(PlayerPreAuthenticatedEventArgs ev) => CurrentEvent?.OnPlayerPreAuthenticated(ev);
		public override void OnPlayerUsingIntercom(PlayerUsingIntercomEventArgs ev) => CurrentEvent?.OnPlayerUsingIntercom(ev);
		public override void OnPlayerUsedIntercom(PlayerUsedIntercomEventArgs ev) => CurrentEvent?.OnPlayerUsedIntercom(ev);
		public override void OnPlayerBanning(PlayerBanningEventArgs ev) => CurrentEvent?.OnPlayerBanning(ev);
		public override void OnPlayerBanned(PlayerBannedEventArgs ev) => CurrentEvent?.OnPlayerBanned(ev);
		public override void OnPlayerKicking(PlayerKickingEventArgs ev) => CurrentEvent?.OnPlayerKicking(ev);
		public override void OnPlayerKicked(PlayerKickedEventArgs ev) => CurrentEvent?.OnPlayerKicked(ev);
		public override void OnPlayerMuting(PlayerMutingEventArgs ev) => CurrentEvent?.OnPlayerMuting(ev);
		public override void OnPlayerMuted(PlayerMutedEventArgs ev) => CurrentEvent?.OnPlayerMuted(ev);
		public override void OnPlayerUnmuting(PlayerUnmutingEventArgs ev) => CurrentEvent?.OnPlayerUnmuting(ev);
		public override void OnPlayerUnmuted(PlayerUnmutedEventArgs ev) => CurrentEvent?.OnPlayerUnmuted(ev);
		public override void OnPlayerReportingCheater(PlayerReportingCheaterEventArgs ev) => CurrentEvent?.OnPlayerReportingCheater(ev);
		public override void OnPlayerReportedCheater(PlayerReportedCheaterEventArgs ev) => CurrentEvent?.OnPlayerReportedCheater(ev);
		public override void OnPlayerReportingPlayer(PlayerReportingPlayerEventArgs ev) => CurrentEvent?.OnPlayerReportingPlayer(ev);
		public override void OnPlayerReportedPlayer(PlayerReportedPlayerEventArgs ev) => CurrentEvent?.OnPlayerReportedPlayer(ev);
		public override void OnPlayerTogglingNoclip(PlayerTogglingNoclipEventArgs ev) => CurrentEvent?.OnPlayerTogglingNoclip(ev);
		public override void OnPlayerToggledNoclip(PlayerToggledNoclipEventArgs ev) => CurrentEvent?.OnPlayerToggledNoclip(ev);
		public override void OnPlayerRequestingRaPlayerList(PlayerRequestingRaPlayerListEventArgs ev) => CurrentEvent?.OnPlayerRequestingRaPlayerList(ev);
		public override void OnPlayerRequestedRaPlayerList(PlayerRequestedRaPlayerListEventArgs ev) => CurrentEvent?.OnPlayerRequestedRaPlayerList(ev);
		public override void OnPlayerRaPlayerListAddingPlayer(PlayerRaPlayerListAddingPlayerEventArgs ev) => CurrentEvent?.OnPlayerRaPlayerListAddingPlayer(ev);
		public override void OnPlayerRaPlayerListAddedPlayer(PlayerRaPlayerListAddedPlayerEventArgs ev) => CurrentEvent?.OnPlayerRaPlayerListAddedPlayer(ev);
		public override void OnPlayerRequestedCustomRaInfo(PlayerRequestedCustomRaInfoEventArgs ev) => CurrentEvent?.OnPlayerRequestedCustomRaInfo(ev);
		public override void OnPlayerRequestingRaPlayersInfo(PlayerRequestingRaPlayersInfoEventArgs ev) => CurrentEvent?.OnPlayerRequestingRaPlayersInfo(ev);
		public override void OnPlayerRequestedRaPlayersInfo(PlayerRequestedRaPlayersInfoEventArgs ev) => CurrentEvent?.OnPlayerRequestedRaPlayersInfo(ev);
		public override void OnPlayerRequestingRaPlayerInfo(PlayerRequestingRaPlayerInfoEventArgs ev) => CurrentEvent?.OnPlayerRequestingRaPlayerInfo(ev);
		public override void OnPlayerRequestedRaPlayerInfo(PlayerRequestedRaPlayerInfoEventArgs ev) => CurrentEvent?.OnPlayerRequestedRaPlayerInfo(ev);
		public override void OnPlayerChangingBadgeVisibility(PlayerChangingBadgeVisibilityEventArgs ev) => CurrentEvent?.OnPlayerChangingBadgeVisibility(ev);
		public override void OnPlayerChangedBadgeVisibility(PlayerChangedBadgeVisibilityEventArgs ev) => CurrentEvent?.OnPlayerChangedBadgeVisibility(ev);
		public override void OnPlayerChangingNickname(PlayerChangingNicknameEventArgs ev) => CurrentEvent?.OnPlayerChangingNickname(ev);
		public override void OnPlayerChangedNickname(PlayerChangedNicknameEventArgs ev) => CurrentEvent?.OnPlayerChangedNickname(ev);
		public override void OnPlayerGroupChanging(PlayerGroupChangingEventArgs ev) => CurrentEvent?.OnPlayerGroupChanging(ev);
		public override void OnPlayerGroupChanged(PlayerGroupChangedEventArgs ev) => CurrentEvent?.OnPlayerGroupChanged(ev);
		public override void OnPlayerUpdatingEffect(PlayerEffectUpdatingEventArgs ev) => CurrentEvent?.OnPlayerUpdatingEffect(ev);
		public override void OnPlayerUpdatedEffect(PlayerEffectUpdatedEventArgs ev) => CurrentEvent?.OnPlayerUpdatedEffect(ev);
		public override void OnPlayerDying(PlayerDyingEventArgs ev) => CurrentEvent?.OnPlayerDying(ev);
		public override void OnPlayerDeath(PlayerDeathEventArgs ev) => CurrentEvent?.OnPlayerDeath(ev);
		public override void OnPlayerHurting(PlayerHurtingEventArgs ev) => CurrentEvent?.OnPlayerHurting(ev);
		public override void OnPlayerHurt(PlayerHurtEventArgs ev) => CurrentEvent?.OnPlayerHurt(ev);
		public override void OnPlayerChangingRole(PlayerChangingRoleEventArgs ev) => CurrentEvent?.OnPlayerChangingRole(ev);
		public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev) => CurrentEvent?.OnPlayerChangedRole(ev);
		public override void OnPlayerCuffing(PlayerCuffingEventArgs ev) => CurrentEvent?.OnPlayerCuffing(ev);
		public override void OnPlayerCuffed(PlayerCuffedEventArgs ev) => CurrentEvent?.OnPlayerCuffed(ev);
		public override void OnPlayerUncuffing(PlayerUncuffingEventArgs ev) => CurrentEvent?.OnPlayerUncuffing(ev);
		public override void OnPlayerUncuffed(PlayerUncuffedEventArgs ev) => CurrentEvent?.OnPlayerUncuffed(ev);
		public override void OnPlayerReceivingLoadout(PlayerReceivingLoadoutEventArgs ev) => CurrentEvent?.OnPlayerReceivingLoadout(ev);
		public override void OnPlayerReceivedLoadout(PlayerReceivedLoadoutEventArgs ev) => CurrentEvent?.OnPlayerReceivedLoadout(ev);
		public override void OnPlayerSpawning(PlayerSpawningEventArgs ev) => CurrentEvent?.OnPlayerSpawning(ev);
		public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev) => CurrentEvent?.OnPlayerSpawned(ev);
		public override void OnPlayerChangingItem(PlayerChangingItemEventArgs ev) => CurrentEvent?.OnPlayerChangingItem(ev);
		public override void OnPlayerChangedItem(PlayerChangedItemEventArgs ev) => CurrentEvent?.OnPlayerChangedItem(ev);
		public override void OnPlayerDroppingAmmo(PlayerDroppingAmmoEventArgs ev) => CurrentEvent?.OnPlayerDroppingAmmo(ev);
		public override void OnPlayerDroppedAmmo(PlayerDroppedAmmoEventArgs ev) => CurrentEvent?.OnPlayerDroppedAmmo(ev);
		public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev) => CurrentEvent?.OnPlayerDroppingItem(ev);
		public override void OnPlayerDroppedItem(PlayerDroppedItemEventArgs ev) => CurrentEvent?.OnPlayerDroppedItem(ev);
		public override void OnPlayerPickingUpAmmo(PlayerPickingUpAmmoEventArgs ev) => CurrentEvent?.OnPlayerPickingUpAmmo(ev);
		public override void OnPlayerPickedUpAmmo(PlayerPickedUpAmmoEventArgs ev) => CurrentEvent?.OnPlayerPickedUpAmmo(ev);
		public override void OnPlayerPickingUpArmor(PlayerPickingUpArmorEventArgs ev) => CurrentEvent?.OnPlayerPickingUpArmor(ev);
		public override void OnPlayerPickedUpArmor(PlayerPickedUpArmorEventArgs ev) => CurrentEvent?.OnPlayerPickedUpArmor(ev);
		public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev) => CurrentEvent?.OnPlayerPickingUpItem(ev);
		public override void OnPlayerPickedUpItem(PlayerPickedUpItemEventArgs ev) => CurrentEvent?.OnPlayerPickedUpItem(ev);
		public override void OnPlayerPickingUpScp330(PlayerPickingUpScp330EventArgs ev) => CurrentEvent?.OnPlayerPickingUpScp330(ev);
		public override void OnPlayerPickedUpScp330(PlayerPickedUpScp330EventArgs ev) => CurrentEvent?.OnPlayerPickedUpScp330(ev);
		public override void OnPlayerSearchedAmmo(PlayerSearchedAmmoEventArgs ev) => CurrentEvent?.OnPlayerSearchedAmmo(ev);
		public override void OnPlayerSearchingArmor(PlayerSearchingArmorEventArgs ev) => CurrentEvent?.OnPlayerSearchingArmor(ev);
		public override void OnPlayerSearchedArmor(PlayerSearchedArmorEventArgs ev) => CurrentEvent?.OnPlayerSearchedArmor(ev);
		public override void OnPlayerSearchingPickup(PlayerSearchingPickupEventArgs ev) => CurrentEvent?.OnPlayerSearchingPickup(ev);
		public override void OnPlayerInteractedToy(PlayerInteractedToyEventArgs ev) => CurrentEvent?.OnPlayerInteractedToy(ev);
		public override void OnPlayerSearchedPickup(PlayerSearchedPickupEventArgs ev) => CurrentEvent?.OnPlayerSearchedPickup(ev);
		public override void OnPlayerSearchingAmmo(PlayerSearchingAmmoEventArgs ev) => CurrentEvent?.OnPlayerSearchingAmmo(ev);
		public override void OnPlayerThrowingItem(PlayerThrowingItemEventArgs ev) => CurrentEvent?.OnPlayerThrowingItem(ev);
		public override void OnPlayerThrewItem(PlayerThrewItemEventArgs ev) => CurrentEvent?.OnPlayerThrewItem(ev);
		public override void OnPlayerThrowingProjectile(PlayerThrowingProjectileEventArgs ev) => CurrentEvent?.OnPlayerThrowingProjectile(ev);
		public override void OnPlayerThrewProjectile(PlayerThrewProjectileEventArgs ev) => CurrentEvent?.OnPlayerThrewProjectile(ev);
		public override void OnPlayerInspectingKeycard(PlayerInspectingKeycardEventArgs ev) => CurrentEvent?.OnPlayerInspectingKeycard(ev);
		public override void OnPlayerInspectedKeycard(PlayerInspectedKeycardEventArgs ev) => CurrentEvent?.OnPlayerInspectedKeycard(ev);
		public override void OnPlayerSpinningRevolver(PlayerSpinningRevolverEventArgs ev) => CurrentEvent?.OnPlayerSpinningRevolver(ev);
		public override void OnPlayerSpinnedRevolver(PlayerSpinnedRevolverEventArgs ev) => CurrentEvent?.OnPlayerSpinnedRevolver(ev);
		public override void OnPlayerToggledDisruptorFiringMode(PlayerToggledDisruptorFiringModeEventArgs ev) => CurrentEvent?.OnPlayerToggledDisruptorFiringMode(ev);
		public override void OnPlayerInspectingItem(PlayerInspectingItemEventArgs ev) => CurrentEvent?.OnPlayerInspectingItem(ev);
		public override void OnPlayerInspectedItem(PlayerInspectedItemEventArgs ev) => CurrentEvent?.OnPlayerInspectedItem(ev);
		public override void OnPlayerUsingItem(PlayerUsingItemEventArgs ev) => CurrentEvent?.OnPlayerUsingItem(ev);
		public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev) => CurrentEvent?.OnPlayerUsedItem(ev);
		public override void OnPlayerItemUsageEffectsApplying(PlayerItemUsageEffectsApplyingEventArgs ev) => CurrentEvent?.OnPlayerItemUsageEffectsApplying(ev);
		public override void OnPlayerUsingRadio(PlayerUsingRadioEventArgs ev) => CurrentEvent?.OnPlayerUsingRadio(ev);
		public override void OnPlayerUsedRadio(PlayerUsedRadioEventArgs ev) => CurrentEvent?.OnPlayerUsedRadio(ev);
		public override void OnPlayerAimedWeapon(PlayerAimedWeaponEventArgs ev) => CurrentEvent?.OnPlayerAimedWeapon(ev);
		public override void OnPlayerDryFiringWeapon(PlayerDryFiringWeaponEventArgs ev) => CurrentEvent?.OnPlayerDryFiringWeapon(ev);
		public override void OnPlayerDryFiredWeapon(PlayerDryFiredWeaponEventArgs ev) => CurrentEvent?.OnPlayerDryFiredWeapon(ev);
		public override void OnPlayerUnloadingWeapon(PlayerUnloadingWeaponEventArgs ev) => CurrentEvent?.OnPlayerUnloadingWeapon(ev);
		public override void OnPlayerUnloadedWeapon(PlayerUnloadedWeaponEventArgs ev) => CurrentEvent?.OnPlayerUnloadedWeapon(ev);
		public override void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev) => CurrentEvent?.OnPlayerReloadingWeapon(ev);
		public override void OnPlayerReloadedWeapon(PlayerReloadedWeaponEventArgs ev) => CurrentEvent?.OnPlayerReloadedWeapon(ev);
		public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev) => CurrentEvent?.OnPlayerShootingWeapon(ev);
		public override void OnPlayerShotWeapon(PlayerShotWeaponEventArgs ev) => CurrentEvent?.OnPlayerShotWeapon(ev);
		public override void OnPlayerChangingAttachments(PlayerChangingAttachmentsEventArgs ev) => CurrentEvent?.OnPlayerChangingAttachments(ev);
		public override void OnPlayerChangedAttachments(PlayerChangedAttachmentsEventArgs ev) => CurrentEvent?.OnPlayerChangedAttachments(ev);
		public override void OnPlayerSendingAttachmentsPrefs(PlayerSendingAttachmentsPrefsEventArgs ev) => CurrentEvent?.OnPlayerSendingAttachmentsPrefs(ev);
		public override void OnPlayerSentAttachmentsPrefs(PlayerSentAttachmentsPrefsEventArgs ev) => CurrentEvent?.OnPlayerSentAttachmentsPrefs(ev);
		public override void OnPlayerCancellingUsingItem(PlayerCancellingUsingItemEventArgs ev) => CurrentEvent?.OnPlayerCancellingUsingItem(ev);
		public override void OnPlayerCancelledUsingItem(PlayerCancelledUsingItemEventArgs ev) => CurrentEvent?.OnPlayerCancelledUsingItem(ev);
		public override void OnPlayerChangingRadioRange(PlayerChangingRadioRangeEventArgs ev) => CurrentEvent?.OnPlayerChangingRadioRange(ev);
		public override void OnPlayerChangedRadioRange(PlayerChangedRadioRangeEventArgs ev) => CurrentEvent?.OnPlayerChangedRadioRange(ev);
		public override void OnPlayerProcessingJailbirdMessage(PlayerProcessingJailbirdMessageEventArgs ev) => CurrentEvent?.OnPlayerProcessingJailbirdMessage(ev);
		public override void OnPlayerProcessedJailbirdMessage(PlayerProcessedJailbirdMessageEventArgs ev) => CurrentEvent?.OnPlayerProcessedJailbirdMessage(ev);
		public override void OnPlayerTogglingFlashlight(PlayerTogglingFlashlightEventArgs ev) => CurrentEvent?.OnPlayerTogglingFlashlight(ev);
		public override void OnPlayerToggledFlashlight(PlayerToggledFlashlightEventArgs ev) => CurrentEvent?.OnPlayerToggledFlashlight(ev);
		public override void OnPlayerTogglingWeaponFlashlight(PlayerTogglingWeaponFlashlightEventArgs ev) => CurrentEvent?.OnPlayerTogglingWeaponFlashlight(ev);
		public override void OnPlayerToggledWeaponFlashlight(PlayerToggledWeaponFlashlightEventArgs ev) => CurrentEvent?.OnPlayerToggledWeaponFlashlight(ev);
		public override void OnPlayerTogglingRadio(PlayerTogglingRadioEventArgs ev) => CurrentEvent?.OnPlayerTogglingRadio(ev);
		public override void OnPlayerToggledRadio(PlayerToggledRadioEventArgs ev) => CurrentEvent?.OnPlayerToggledRadio(ev);
		public override void OnPlayerJumped(PlayerJumpedEventArgs ev) => CurrentEvent?.OnPlayerJumped(ev);
		public override void OnPlayerMovementStateChanged(PlayerMovementStateChangedEventArgs ev) => CurrentEvent?.OnPlayerMovementStateChanged(ev);
		public override void OnPlayerProcessingScp1509Message(PlayerProcessingScp1509MessageEventArgs ev) => CurrentEvent?.OnPlayerProcessingScp1509Message(ev);
		public override void OnPlayerProcessedScp1509Message(PlayerProcessedScp1509MessageEventArgs ev) => CurrentEvent?.OnPlayerProcessedScp1509Message(ev);
		public override void OnPlayerScp1509Resurrecting(PlayerScp1509ResurrectingEventArgs ev) => CurrentEvent?.OnPlayerScp1509Resurrecting(ev);
		public override void OnPlayerScp1509Resurrected(PlayerScp1509ResurrectedEventArgs ev) => CurrentEvent?.OnPlayerScp1509Resurrected(ev);
		public override void OnPlayerDamagingShootingTarget(PlayerDamagingShootingTargetEventArgs ev) => CurrentEvent?.OnPlayerDamagingShootingTarget(ev);
		public override void OnPlayerDamagedShootingTarget(PlayerDamagedShootingTargetEventArgs ev) => CurrentEvent?.OnPlayerDamagedShootingTarget(ev);
		public override void OnPlayerDamagingWindow(PlayerDamagingWindowEventArgs ev) => CurrentEvent?.OnPlayerDamagingWindow(ev);
		public override void OnPlayerDamagedWindow(PlayerDamagedWindowEventArgs ev) => CurrentEvent?.OnPlayerDamagedWindow(ev);
		public override void OnPlayerEnteringPocketDimension(PlayerEnteringPocketDimensionEventArgs ev) => CurrentEvent?.OnPlayerEnteringPocketDimension(ev);
		public override void OnPlayerEnteredPocketDimension(PlayerEnteredPocketDimensionEventArgs ev) => CurrentEvent?.OnPlayerEnteredPocketDimension(ev);
		public override void OnPlayerLeavingPocketDimension(PlayerLeavingPocketDimensionEventArgs ev) => CurrentEvent?.OnPlayerLeavingPocketDimension(ev);
		public override void OnPlayerLeftPocketDimension(PlayerLeftPocketDimensionEventArgs ev) => CurrentEvent?.OnPlayerLeftPocketDimension(ev);
		public override void OnPlayerTriggeringTesla(PlayerTriggeringTeslaEventArgs ev) => CurrentEvent?.OnPlayerTriggeringTesla(ev);
		public override void OnPlayerTriggeredTesla(PlayerTriggeredTeslaEventArgs ev) => CurrentEvent?.OnPlayerTriggeredTesla(ev);
		public override void OnPlayerEscaping(PlayerEscapingEventArgs ev) => CurrentEvent?.OnPlayerEscaping(ev);
		public override void OnPlayerEscaped(PlayerEscapedEventArgs ev) => CurrentEvent?.OnPlayerEscaped(ev);
		public override void OnPlayerFlippingCoin(PlayerFlippingCoinEventArgs ev) => CurrentEvent?.OnPlayerFlippingCoin(ev);
		public override void OnPlayerFlippedCoin(PlayerFlippedCoinEventArgs ev) => CurrentEvent?.OnPlayerFlippedCoin(ev);
		public override void OnPlayerSearchingToy(PlayerSearchingToyEventArgs ev) => CurrentEvent?.OnPlayerSearchingToy(ev);
		public override void OnPlayerSearchedToy(PlayerSearchedToyEventArgs ev) => CurrentEvent?.OnPlayerSearchedToy(ev);
		public override void OnPlayerSearchToyAborted(PlayerSearchToyAbortedEventArgs ev) => CurrentEvent?.OnPlayerSearchToyAborted(ev);
		public override void OnPlayerIdlingTesla(PlayerIdlingTeslaEventArgs ev) => CurrentEvent?.OnPlayerIdlingTesla(ev);
		public override void OnPlayerIdledTesla(PlayerIdledTeslaEventArgs ev) => CurrentEvent?.OnPlayerIdledTesla(ev);
		public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev) => CurrentEvent?.OnPlayerInteractingDoor(ev);
		public override void OnPlayerInteractedDoor(PlayerInteractedDoorEventArgs ev) => CurrentEvent?.OnPlayerInteractedDoor(ev);
		public override void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs ev) => CurrentEvent?.OnPlayerInteractingElevator(ev);
		public override void OnPlayerInteractedElevator(PlayerInteractedElevatorEventArgs ev) => CurrentEvent?.OnPlayerInteractedElevator(ev);
		public override void OnPlayerInteractingGenerator(PlayerInteractingGeneratorEventArgs ev) => CurrentEvent?.OnPlayerInteractingGenerator(ev);
		public override void OnPlayerInteractedGenerator(PlayerInteractedGeneratorEventArgs ev) => CurrentEvent?.OnPlayerInteractedGenerator(ev);
		public override void OnPlayerOpeningGenerator(PlayerOpeningGeneratorEventArgs ev) => CurrentEvent?.OnPlayerOpeningGenerator(ev);
		public override void OnPlayerOpenedGenerator(PlayerOpenedGeneratorEventArgs ev) => CurrentEvent?.OnPlayerOpenedGenerator(ev);
		public override void OnPlayerActivatingGenerator(PlayerActivatingGeneratorEventArgs ev) => CurrentEvent?.OnPlayerActivatingGenerator(ev);
		public override void OnPlayerActivatedGenerator(PlayerActivatedGeneratorEventArgs ev) => CurrentEvent?.OnPlayerActivatedGenerator(ev);
		public override void OnPlayerDeactivatingGenerator(PlayerDeactivatingGeneratorEventArgs ev) => CurrentEvent?.OnPlayerDeactivatingGenerator(ev);
		public override void OnPlayerDeactivatedGenerator(PlayerDeactivatedGeneratorEventArgs ev) => CurrentEvent?.OnPlayerDeactivatedGenerator(ev);
		public override void OnPlayerUnlockingGenerator(PlayerUnlockingGeneratorEventArgs ev) => CurrentEvent?.OnPlayerUnlockingGenerator(ev);
		public override void OnPlayerUnlockedGenerator(PlayerUnlockedGeneratorEventArgs ev) => CurrentEvent?.OnPlayerUnlockedGenerator(ev);
		public override void OnPlayerClosingGenerator(PlayerClosingGeneratorEventArgs ev) => CurrentEvent?.OnPlayerClosingGenerator(ev);
		public override void OnPlayerClosedGenerator(PlayerClosedGeneratorEventArgs ev) => CurrentEvent?.OnPlayerClosedGenerator(ev);
		public override void OnPlayerInteractingLocker(PlayerInteractingLockerEventArgs ev) => CurrentEvent?.OnPlayerInteractingLocker(ev);
		public override void OnPlayerInteractedLocker(PlayerInteractedLockerEventArgs ev) => CurrentEvent?.OnPlayerInteractedLocker(ev);
		public override void OnPlayerInteractingScp330(PlayerInteractingScp330EventArgs ev) => CurrentEvent?.OnPlayerInteractingScp330(ev);
		public override void OnPlayerInteractedScp330(PlayerInteractedScp330EventArgs ev) => CurrentEvent?.OnPlayerInteractedScp330(ev);
		public override void OnPlayerInteractingShootingTarget(PlayerInteractingShootingTargetEventArgs ev) => CurrentEvent?.OnPlayerInteractingShootingTarget(ev);
		public override void OnPlayerInteractedShootingTarget(PlayerInteractedShootingTargetEventArgs ev) => CurrentEvent?.OnPlayerInteractedShootingTarget(ev);
		public override void OnPlayerPlacingBlood(PlayerPlacingBloodEventArgs ev) => CurrentEvent?.OnPlayerPlacingBlood(ev);
		public override void OnPlayerPlacedBlood(PlayerPlacedBloodEventArgs ev) => CurrentEvent?.OnPlayerPlacedBlood(ev);
		public override void OnPlayerPlacingBulletHole(PlayerPlacingBulletHoleEventArgs ev) => CurrentEvent?.OnPlayerPlacingBulletHole(ev);
		public override void OnPlayerPlacedBulletHole(PlayerPlacedBulletHoleEventArgs ev) => CurrentEvent?.OnPlayerPlacedBulletHole(ev);
		public override void OnPlayerSpawningRagdoll(PlayerSpawningRagdollEventArgs ev) => CurrentEvent?.OnPlayerSpawningRagdoll(ev);
		public override void OnPlayerSpawnedRagdoll(PlayerSpawnedRagdollEventArgs ev) => CurrentEvent?.OnPlayerSpawnedRagdoll(ev);
		public override void OnPlayerUnlockingWarheadButton(PlayerUnlockingWarheadButtonEventArgs ev) => CurrentEvent?.OnPlayerUnlockingWarheadButton(ev);
		public override void OnPlayerUnlockedWarheadButton(PlayerUnlockedWarheadButtonEventArgs ev) => CurrentEvent?.OnPlayerUnlockedWarheadButton(ev);
		public override void OnPlayerReceivedAchievement(PlayerReceivedAchievementEventArgs ev) => CurrentEvent?.OnPlayerReceivedAchievement(ev);
		public override void OnPlayerRoomChanged(PlayerRoomChangedEventArgs ev) => CurrentEvent?.OnPlayerRoomChanged(ev);
		public override void OnPlayerZoneChanged(PlayerZoneChangedEventArgs ev) => CurrentEvent?.OnPlayerZoneChanged(ev);
		public override void OnPlayerInteractingWarheadLever(PlayerInteractingWarheadLeverEventArgs ev) => CurrentEvent?.OnPlayerInteractingWarheadLever(ev);
		public override void OnPlayerInteractedWarheadLever(PlayerInteractedWarheadLeverEventArgs ev) => CurrentEvent?.OnPlayerInteractedWarheadLever(ev);
		public override void OnPlayerSendingHitmarker(PlayerSendingHitmarkerEventArgs ev) => CurrentEvent?.OnPlayerSendingHitmarker(ev);
		public override void OnPlayerSentHitmarker(PlayerSentHitmarkerEventArgs ev) => CurrentEvent?.OnPlayerSentHitmarker(ev);
		public override void OnPlayerCheckedHitmarker(PlayerCheckedHitmarkerEventArgs ev) => CurrentEvent?.OnPlayerCheckedHitmarker(ev);
		public override void OnPlayerChangedSpectator(PlayerChangedSpectatorEventArgs ev) => CurrentEvent?.OnPlayerChangedSpectator(ev);
		public override void OnPlayerEnteringHazard(PlayerEnteringHazardEventArgs ev) => CurrentEvent?.OnPlayerEnteringHazard(ev);
		public override void OnPlayerEnteredHazard(PlayerEnteredHazardEventArgs ev) => CurrentEvent?.OnPlayerEnteredHazard(ev);
		public override void OnPlayerStayingInHazard(PlayersStayingInHazardEventArgs ev) => CurrentEvent?.OnPlayerStayingInHazard(ev);
		public override void OnPlayerLeavingHazard(PlayerLeavingHazardEventArgs ev) => CurrentEvent?.OnPlayerLeavingHazard(ev);
		public override void OnPlayerLeftHazard(PlayerLeftHazardEventArgs ev) => CurrentEvent?.OnPlayerLeftHazard(ev);
		public override void OnPlayerValidatedVisibility(PlayerValidatedVisibilityEventArgs ev) => CurrentEvent?.OnPlayerValidatedVisibility(ev);
		public override void OnPlayerDetectedByScp1344(PlayerDetectedByScp1344EventArgs ev) => CurrentEvent?.OnPlayerDetectedByScp1344(ev);
		public override void OnScp0492StartingConsumingCorpse(Scp0492StartingConsumingCorpseEventArgs ev) => CurrentEvent?.OnScp0492StartingConsumingCorpse(ev);
		public override void OnScp0492StartedConsumingCorpse(Scp0492StartedConsumingCorpseEventArgs ev) => CurrentEvent?.OnScp0492StartedConsumingCorpse(ev);
		public override void OnScp0492ConsumingCorpse(Scp0492ConsumingCorpseEventArgs ev) => CurrentEvent?.OnScp0492ConsumingCorpse(ev);
		public override void OnScp0492ConsumedCorpse(Scp0492ConsumedCorpseEventArgs ev) => CurrentEvent?.OnScp0492ConsumedCorpse(ev);
		public override void OnScp049StartingResurrection(Scp049StartingResurrectionEventArgs ev) => CurrentEvent?.OnScp049StartingResurrection(ev);
		public override void OnScp049ResurrectingBody(Scp049ResurrectingBodyEventArgs ev) => CurrentEvent?.OnScp049ResurrectingBody(ev);
		public override void OnScp049ResurrectedBody(Scp049ResurrectedBodyEventArgs ev) => CurrentEvent?.OnScp049ResurrectedBody(ev);
		public override void OnScp049UsingDoctorsCall(Scp049UsingDoctorsCallEventArgs ev) => CurrentEvent?.OnScp049UsingDoctorsCall(ev);
		public override void OnScp049UsedDoctorsCall(Scp049UsedDoctorsCallEventArgs ev) => CurrentEvent?.OnScp049UsedDoctorsCall(ev);
		public override void OnScp049UsingSense(Scp049UsingSenseEventArgs ev) => CurrentEvent?.OnScp049UsingSense(ev);
		public override void OnScp049UsedSense(Scp049UsedSenseEventArgs ev) => CurrentEvent?.OnScp049UsedSense(ev);
		public override void OnScp049Attacking(Scp049AttackingEventArgs ev) => CurrentEvent?.OnScp049Attacking(ev);
		public override void OnScp049Attacked(Scp049AttackedEventArgs ev) => CurrentEvent?.OnScp049Attacked(ev);
		public override void OnScp049SenseLostTarget(Scp049SenseLostTargetEventArgs ev) => CurrentEvent?.OnScp049SenseLostTarget(ev);
		public override void OnScp049SenseKilledTarget(Scp049SenseKilledTargetEventArgs ev) => CurrentEvent?.OnScp049SenseKilledTarget(ev);
		public override void OnScp079BlackingOutRoom(Scp079BlackingOutRoomEventsArgs ev) => CurrentEvent?.OnScp079BlackingOutRoom(ev);
		public override void OnScp079BlackedOutRoom(Scp079BlackedOutRoomEventArgs ev) => CurrentEvent?.OnScp079BlackedOutRoom(ev);
		public override void OnScp079BlackingOutZone(Scp079BlackingOutZoneEventArgs ev) => CurrentEvent?.OnScp079BlackingOutZone(ev);
		public override void OnScp079BlackedOutZone(Scp079BlackedOutZoneEventArgs ev) => CurrentEvent?.OnScp079BlackedOutZone(ev);
		public override void OnScp079ChangingCamera(Scp079ChangingCameraEventArgs ev) => CurrentEvent?.OnScp079ChangingCamera(ev);
		public override void OnScp079ChangedCamera(Scp079ChangedCameraEventArgs ev) => CurrentEvent?.OnScp079ChangedCamera(ev);
		public override void OnScp079CancellingRoomLockdown(Scp079CancellingRoomLockdownEventArgs ev) => CurrentEvent?.OnScp079CancellingRoomLockdown(ev);
		public override void OnScp079CancelledRoomLockdown(Scp079CancelledRoomLockdownEventArgs ev) => CurrentEvent?.OnScp079CancelledRoomLockdown(ev);
		public override void OnScp079GainingExperience(Scp079GainingExperienceEventArgs ev) => CurrentEvent?.OnScp079GainingExperience(ev);
		public override void OnScp079GainedExperience(Scp079GainedExperienceEventArgs ev) => CurrentEvent?.OnScp079GainedExperience(ev);
		public override void OnScp079LevelingUp(Scp079LevelingUpEventArgs ev) => CurrentEvent?.OnScp079LevelingUp(ev);
		public override void OnScp079LeveledUp(Scp079LeveledUpEventArgs ev) => CurrentEvent?.OnScp079LeveledUp(ev);
		public override void OnScp079LockingDoor(Scp079LockingDoorEventArgs ev) => CurrentEvent?.OnScp079LockingDoor(ev);
		public override void OnScp079LockedDoor(Scp079LockedDoorEventArgs ev) => CurrentEvent?.OnScp079LockedDoor(ev);
		public override void OnScp079LockingDownRoom(Scp079LockingDownRoomEventArgs ev) => CurrentEvent?.OnScp079LockingDownRoom(ev);
		public override void OnScp079LockedDownRoom(Scp079LockedDownRoomEventArgs ev) => CurrentEvent?.OnScp079LockedDownRoom(ev);
		public override void OnScp079Recontaining(Scp079RecontainingEventArgs ev) => CurrentEvent?.OnScp079Recontaining(ev);
		public override void OnScp079Recontained(Scp079RecontainedEventArgs ev) => CurrentEvent?.OnScp079Recontained(ev);
		public override void OnScp079UnlockingDoor(Scp079UnlockingDoorEventArgs ev) => CurrentEvent?.OnScp079UnlockingDoor(ev);
		public override void OnScp079UnlockedDoor(Scp079UnlockedDoorEventArgs ev) => CurrentEvent?.OnScp079UnlockedDoor(ev);
		public override void OnScp079UsingTesla(Scp079UsingTeslaEventArgs ev) => CurrentEvent?.OnScp079UsingTesla(ev);
		public override void OnScp079UsedTesla(Scp079UsedTeslaEventArgs ev) => CurrentEvent?.OnScp079UsedTesla(ev);
		public override void OnScp079Pinging(Scp079PingingEventArgs ev) => CurrentEvent?.OnScp079Pinging(ev);
		public override void OnScp079Pinged(Scp079PingedEventArgs ev) => CurrentEvent?.OnScp079Pinged(ev);
		public override void OnScp096AddingTarget(Scp096AddingTargetEventArgs ev) => CurrentEvent?.OnScp096AddingTarget(ev);
		public override void OnScp096AddedTarget(Scp096AddedTargetEventArgs ev) => CurrentEvent?.OnScp096AddedTarget(ev);
		public override void OnScp096ChangingState(Scp096ChangingStateEventArgs ev) => CurrentEvent?.OnScp096ChangingState(ev);
		public override void OnScp096ChangedState(Scp096ChangedStateEventArgs ev) => CurrentEvent?.OnScp096ChangedState(ev);
		public override void OnScp096Charging(Scp096ChargingEventArgs ev) => CurrentEvent?.OnScp096Charging(ev);
		public override void OnScp096Charged(Scp096ChargedEventArgs ev) => CurrentEvent?.OnScp096Charged(ev);
		public override void OnScp096Enraging(Scp096EnragingEventArgs ev) => CurrentEvent?.OnScp096Enraging(ev);
		public override void OnScp096Enraged(Scp096EnragedEventArgs ev) => CurrentEvent?.OnScp096Enraged(ev);
		public override void OnScp096PryingGate(Scp096PryingGateEventArgs ev) => CurrentEvent?.OnScp096PryingGate(ev);
		public override void OnScp096PriedGate(Scp096PriedGateEventArgs ev) => CurrentEvent?.OnScp096PriedGate(ev);
		public override void OnScp096StartCrying(Scp096StartCryingEventArgs ev) => CurrentEvent?.OnScp096StartCrying(ev);
		public override void OnScp096StartedCrying(Scp096StartedCryingEventArgs ev) => CurrentEvent?.OnScp096StartedCrying(ev);
		public override void OnScp096TryingNotToCry(Scp096TryingNotToCryEventArgs ev) => CurrentEvent?.OnScp096TryingNotToCry(ev);
		public override void OnScp096TriedNotToCry(Scp096TriedNotToCryEventArgs ev) => CurrentEvent?.OnScp096TriedNotToCry(ev);
		public override void OnScp106ChangingStalkMode(Scp106ChangingStalkModeEventArgs ev) => CurrentEvent?.OnScp106ChangingStalkMode(ev);
		public override void OnScp106ChangedStalkMode(Scp106ChangedStalkModeEventArgs ev) => CurrentEvent?.OnScp106ChangedStalkMode(ev);
		public override void OnScp106ChangingVigor(Scp106ChangingVigorEventArgs ev) => CurrentEvent?.OnScp106ChangingVigor(ev);
		public override void OnScp106ChangedVigor(Scp106ChangedVigorEventArgs ev) => CurrentEvent?.OnScp106ChangedVigor(ev);
		public override void OnScp106UsedHunterAtlas(Scp106UsedHunterAtlasEventArgs ev) => CurrentEvent?.OnScp106UsedHunterAtlas(ev);
		public override void OnScp106UsingHunterAtlas(Scp106UsingHunterAtlasEventArgs ev) => CurrentEvent?.OnScp106UsingHunterAtlas(ev);
		public override void OnScp106ChangingSubmersionStatus(Scp106ChangingSubmersionStatusEventArgs ev) => CurrentEvent?.OnScp106ChangingSubmersionStatus(ev);
		public override void OnScp106ChangedSubmersionStatus(Scp106ChangedSubmersionStatusEventArgs ev) => CurrentEvent?.OnScp106ChangedSubmersionStatus(ev);
		public override void OnScp106TeleportingPlayer(Scp106TeleportingPlayerEvent ev) => CurrentEvent?.OnScp106TeleportingPlayer(ev);
		public override void OnScp106TeleportedPlayer(Scp106TeleportedPlayerEvent ev) => CurrentEvent?.OnScp106TeleportedPlayer(ev);
		public override void OnScp127GainingExperience(Scp127GainingExperienceEventArgs ev) => CurrentEvent?.OnScp127GainingExperience(ev);
		public override void OnScp127GainExperience(Scp127GainExperienceEventArgs ev) => CurrentEvent?.OnScp127GainExperience(ev);
		public override void OnScp127LevellingUp(Scp127LevellingUpEventArgs ev) => CurrentEvent?.OnScp127LevellingUp(ev);
		public override void OnScp127LevelUp(Scp127LevelUpEventArgs ev) => CurrentEvent?.OnScp127LevelUp(ev);
		public override void OnScp127Talking(Scp127TalkingEventArgs ev) => CurrentEvent?.OnScp127Talking(ev);
		public override void OnScp127Talked(Scp127TalkedEventArgs ev) => CurrentEvent?.OnScp127Talked(ev);
		public override void OnScp173BreakneckSpeedChanging(Scp173BreakneckSpeedChangingEventArgs ev) => CurrentEvent?.OnScp173BreakneckSpeedChanging(ev);
		public override void OnScp173BreakneckSpeedChanged(Scp173BreakneckSpeedChangedEventArgs ev) => CurrentEvent?.OnScp173BreakneckSpeedChanged(ev);
		public override void OnScp173AddingObserver(Scp173AddingObserverEventArgs ev) => CurrentEvent?.OnScp173AddingObserver(ev);
		public override void OnScp173AddedObserver(Scp173AddedObserverEventArgs ev) => CurrentEvent?.OnScp173AddedObserver(ev);
		public override void OnScp173RemovingObserver(Scp173RemovingObserverEventArgs ev) => CurrentEvent?.OnScp173RemovingObserver(ev);
		public override void OnScp173RemovedObserver(Scp173RemovedObserverEventArgs ev) => CurrentEvent?.OnScp173RemovedObserver(ev);
		public override void OnScp173CreatingTantrum(Scp173CreatingTantrumEventArgs ev) => CurrentEvent?.OnScp173CreatingTantrum(ev);
		public override void OnScp173CreatedTantrum(Scp173CreatedTantrumEventArgs ev) => CurrentEvent?.OnScp173CreatedTantrum(ev);
		public override void OnScp173PlayingSound(Scp173PlayingSoundEventArgs ev) => CurrentEvent?.OnScp173PlayingSound(ev);
		public override void OnScp173PlayedSound(Scp173PlayedSoundEventArgs ev) => CurrentEvent?.OnScp173PlayedSound(ev);
		public override void OnScp173Teleporting(Scp173TeleportingEventArgs ev) => CurrentEvent?.OnScp173Teleporting(ev);
		public override void OnScp173Teleported(Scp173TeleportedEventArgs ev) => CurrentEvent?.OnScp173Teleported(ev);
		public override void OnScp173Snapping(Scp173SnappingEventArgs ev) => CurrentEvent?.OnScp173Snapping(ev);
		public override void OnScp173Snapped(Scp173SnappedEventArgs ev) => CurrentEvent?.OnScp173Snapped(ev);
		public override void OnScp3114Disguising(Scp3114DisguisingEventArgs ev) => CurrentEvent?.OnScp3114Disguising(ev);
		public override void OnScp3114Disguised(Scp3114DisguisedEventArgs ev) => CurrentEvent?.OnScp3114Disguised(ev);
		public override void OnScp3114Revealing(Scp3114RevealingEventArgs ev) => CurrentEvent?.OnScp3114Revealing(ev);
		public override void OnScp3114Revealed(Scp3114RevealedEventArgs ev) => CurrentEvent?.OnScp3114Revealed(ev);
		public override void OnScp3114StrangleStarting(Scp3114StrangleStartingEventArgs ev) => CurrentEvent?.OnScp3114StrangleStarting(ev);
		public override void OnScp3114StrangleStarted(Scp3114StrangleStartedEventArgs ev) => CurrentEvent?.OnScp3114StrangleStarted(ev);
		public override void OnScp3114Dance(Scp3114StartedDanceEventArgs ev) => CurrentEvent?.OnScp3114Dance(ev);
		public override void OnScp3114StartDancing(Scp3114StartingDanceEventArgs ev) => CurrentEvent?.OnScp3114StartDancing(ev);
		public override void OnScp3114StrangleAborting(Scp3114StrangleAbortingEventArgs ev) => CurrentEvent?.OnScp3114StrangleAborting(ev);
		public override void OnScp3114StrangleAborted(Scp3114StrangleAbortedEventArgs ev) => CurrentEvent?.OnScp3114StrangleAborted(ev);
		public override void OnScp914Activating(Scp914ActivatingEventArgs ev) => CurrentEvent?.OnScp914Activating(ev);
		public override void OnScp914Activated(Scp914ActivatedEventArgs ev) => CurrentEvent?.OnScp914Activated(ev);
		public override void OnScp914KnobChanging(Scp914KnobChangingEventArgs ev) => CurrentEvent?.OnScp914KnobChanging(ev);
		public override void OnScp914KnobChanged(Scp914KnobChangedEventArgs ev) => CurrentEvent?.OnScp914KnobChanged(ev);
		public override void OnScp914ProcessingPickup(Scp914ProcessingPickupEventArgs ev) => CurrentEvent?.OnScp914ProcessingPickup(ev);
		public override void OnScp914ProcessedPickup(Scp914ProcessedPickupEventArgs ev) => CurrentEvent?.OnScp914ProcessedPickup(ev);
		public override void OnScp914ProcessingPlayer(Scp914ProcessingPlayerEventArgs ev) => CurrentEvent?.OnScp914ProcessingPlayer(ev);
		public override void OnScp914ProcessedPlayer(Scp914ProcessedPlayerEventArgs ev) => CurrentEvent?.OnScp914ProcessedPlayer(ev);
		public override void OnScp914ProcessingInventoryItem(Scp914ProcessingInventoryItemEventArgs ev) => CurrentEvent?.OnScp914ProcessingInventoryItem(ev);
		public override void OnScp914ProcessedInventoryItem(Scp914ProcessedInventoryItemEventArgs ev) => CurrentEvent?.OnScp914ProcessedInventoryItem(ev);
		public override void OnScp939Attacking(Scp939AttackingEventArgs ev) => CurrentEvent?.OnScp939Attacking(ev);
		public override void OnScp939Attacked(Scp939AttackedEventArgs ev) => CurrentEvent?.OnScp939Attacked(ev);
		public override void OnScp939CreatingAmnesticCloud(Scp939CreatingAmnesticCloudEventArgs ev) => CurrentEvent?.OnScp939CreatingAmnesticCloud(ev);
		public override void OnScp939CreatedAmnesticCloud(Scp939CreatedAmnesticCloudEventArgs ev) => CurrentEvent?.OnScp939CreatedAmnesticCloud(ev);
		public override void OnScp939Lunging(Scp939LungingEventArgs ev) => CurrentEvent?.OnScp939Lunging(ev);
		public override void OnScp939Lunged(Scp939LungedEventArgs ev) => CurrentEvent?.OnScp939Lunged(ev);
		public override void OnScp939Focused(Scp939FocusedEventArgs ev) => CurrentEvent?.OnScp939Focused(ev);
		public override void OnScp939MimickingEnvironment(Scp939MimickingEnvironmentEventArgs ev) => CurrentEvent?.OnScp939MimickingEnvironment(ev);
		public override void OnScp939MimickedEnvironment(Scp939MimickedEnvironmentEventArgs ev) => CurrentEvent?.OnScp939MimickedEnvironment(ev);
		public override void OnScpHumeShieldBroken(ScpHumeShieldBrokenEventArgs ev) => CurrentEvent?.OnScpHumeShieldBroken(ev);
		public override void OnServerRoundRestarted() => CurrentEvent?.OnServerRoundRestarted();
		public override void OnServerShutdown() => CurrentEvent?.OnServerShutdown();
		public override void OnServerDeadmanSequenceActivated() => CurrentEvent?.OnServerDeadmanSequenceActivated();
		public override void OnServerDeadmanSequenceActivating(DeadmanSequenceActivatingEventArgs ev) => CurrentEvent?.OnServerDeadmanSequenceActivating(ev);
		public override void OnServerRoundEndingConditionsCheck(RoundEndingConditionsCheckEventArgs ev) => CurrentEvent?.OnServerRoundEndingConditionsCheck(ev);
		public override void OnServerRoundEnding(RoundEndingEventArgs ev) => CurrentEvent?.OnServerRoundEnding(ev);
		public override void OnServerRoundEnded(RoundEndedEventArgs ev) => CurrentEvent?.OnServerRoundEnded(ev);
		public override void OnServerRoundStarting(RoundStartingEventArgs ev) => CurrentEvent?.OnServerRoundStarting(ev);
		public override void OnServerRoundStarted() => CurrentEvent?.OnServerRoundStarted();
		public override void OnServerBanIssuing(BanIssuingEventArgs ev) => CurrentEvent?.OnServerBanIssuing(ev);
		public override void OnServerBanIssued(BanIssuedEventArgs ev) => CurrentEvent?.OnServerBanIssued(ev);
		public override void OnServerBanRevoking(BanRevokingEventArgs ev) => CurrentEvent?.OnServerBanRevoking(ev);
		public override void OnServerBanRevoked(BanRevokedEventArgs ev) => CurrentEvent?.OnServerBanRevoked(ev);
		public override void OnServerBanUpdating(BanUpdatingEventArgs ev) => CurrentEvent?.OnServerBanUpdating(ev);
		public override void OnServerBanUpdated(BanUpdatedEventArgs ev) => CurrentEvent?.OnServerBanUpdated(ev);
		public override void OnServerCommandExecuting(CommandExecutingEventArgs ev) => CurrentEvent?.OnServerCommandExecuting(ev);
		public override void OnServerCommandExecuted(CommandExecutedEventArgs ev) => CurrentEvent?.OnServerCommandExecuted(ev);
		public override void OnServerCassieQueuingScpTermination(CassieQueuingScpTerminationEventArgs ev) => CurrentEvent?.OnServerCassieQueuingScpTermination(ev);
		public override void OnServerCassieQueuedScpTermination(CassieQueuedScpTerminationEventArgs ev) => CurrentEvent?.OnServerCassieQueuedScpTermination(ev);
		public override void OnServerWaveRespawning(WaveRespawningEventArgs ev) => CurrentEvent?.OnServerWaveRespawning(ev);
		public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev) => CurrentEvent?.OnServerWaveRespawned(ev);
		public override void OnServerWaveTeamSelecting(WaveTeamSelectingEventArgs ev) => CurrentEvent?.OnServerWaveTeamSelecting(ev);
		public override void OnServerWaveTeamSelected(WaveTeamSelectedEventArgs ev) => CurrentEvent?.OnServerWaveTeamSelected(ev);
		public override void OnServerLczDecontaminationAnnounced(LczDecontaminationAnnouncedEventArgs ev) => CurrentEvent?.OnServerLczDecontaminationAnnounced(ev);
		public override void OnServerLczDecontaminationStarting(LczDecontaminationStartingEventArgs ev) => CurrentEvent?.OnServerLczDecontaminationStarting(ev);
		public override void OnServerLczDecontaminationStarted() => CurrentEvent?.OnServerLczDecontaminationStarted();
		public override void OnServerMapGenerating(MapGeneratingEventArgs ev) => CurrentEvent?.OnServerMapGenerating(ev);
		public override void OnServerMapGenerated(MapGeneratedEventArgs ev) => CurrentEvent?.OnServerMapGenerated(ev);
		public override void OnServerPickupCreated(PickupCreatedEventArgs ev) => CurrentEvent?.OnServerPickupCreated(ev);
		public override void OnServerPickupDestroyed(PickupDestroyedEventArgs ev) => CurrentEvent?.OnServerPickupDestroyed(ev);
		public override void OnServerSendingAdminChat(SendingAdminChatEventArgs ev) => CurrentEvent?.OnServerSendingAdminChat(ev);
		public override void OnServerSentAdminChat(SentAdminChatEventArgs ev) => CurrentEvent?.OnServerSentAdminChat(ev);
		public override void OnServerItemSpawning(ItemSpawningEventArgs ev) => CurrentEvent?.OnServerItemSpawning(ev);
		public override void OnServerItemSpawned(ItemSpawnedEventArgs ev) => CurrentEvent?.OnServerItemSpawned(ev);
		public override void OnServerCassieAnnouncing(CassieAnnouncingEventArgs ev) => CurrentEvent?.OnServerCassieAnnouncing(ev);
		public override void OnServerCassieAnnounced(CassieAnnouncedEventArgs ev) => CurrentEvent?.OnServerCassieAnnounced(ev);
		public override void OnServerProjectileExploding(ProjectileExplodingEventArgs ev) => CurrentEvent?.OnServerProjectileExploding(ev);
		public override void OnServerProjectileExploded(ProjectileExplodedEventArgs ev) => CurrentEvent?.OnServerProjectileExploded(ev);
		public override void OnServerExplosionSpawning(ExplosionSpawningEventArgs ev) => CurrentEvent?.OnServerExplosionSpawning(ev);
		public override void OnServerExplosionSpawned(ExplosionSpawnedEventArgs ev) => CurrentEvent?.OnServerExplosionSpawned(ev);
		public override void OnServerGeneratorActivating(GeneratorActivatingEventArgs ev) => CurrentEvent?.OnServerGeneratorActivating(ev);
		public override void OnServerGeneratorActivated(GeneratorActivatedEventArgs ev) => CurrentEvent?.OnServerGeneratorActivated(ev);
		public override void OnServerElevatorSequenceChanged(ElevatorSequenceChangedEventArgs ev) => CurrentEvent?.OnServerElevatorSequenceChanged(ev);
		public override void OnServerModifyingFactionInfluence(ModifyingFactionInfluenceEventArgs ev) => CurrentEvent?.OnServerModifyingFactionInfluence(ev);
		public override void OnServerModifiedFactionInfluence(ModifiedFactionInfluenceEventArgs ev) => CurrentEvent?.OnServerModifiedFactionInfluence(ev);
		public override void OnServerAchievingMilestone(AchievingMilestoneEventArgs ev) => CurrentEvent?.OnServerAchievingMilestone(ev);
		public override void OnServerAchievedMilestone(AchievedMilestoneEventArgs ev) => CurrentEvent?.OnServerAchievedMilestone(ev);
		public override void OnServerBlastDoorChanging(BlastDoorChangingEventArgs ev) => CurrentEvent?.OnServerBlastDoorChanging(ev);
		public override void OnServerBlastDoorChanged(BlastDoorChangedEventArgs ev) => CurrentEvent?.OnServerBlastDoorChanged(ev);
		public override void OnServerRoomLightChanged(RoomLightChangedEventArgs ev) => CurrentEvent?.OnServerRoomLightChanged(ev);
		public override void OnServerRoomColorChanged(RoomColorChangedEventArgs ev) => CurrentEvent?.OnServerRoomColorChanged(ev);
		public override void OnServerDoorLockChanged(DoorLockChangedEventArgs ev) => CurrentEvent?.OnServerDoorLockChanged(ev);
		public override void OnServerDoorRepairing(DoorRepairingEventArgs ev) => CurrentEvent?.OnServerDoorRepairing(ev);
		public override void OnServerDoorRepaired(DoorRepairedEventArgs ev) => CurrentEvent?.OnServerDoorRepaired(ev);
		public override void OnServerDoorDamaging(DoorDamagingEventArgs ev) => CurrentEvent?.OnServerDoorDamaging(ev);
		public override void OnServerDoorDamaged(DoorDamagedEventArgs ev) => CurrentEvent?.OnServerDoorDamaged(ev);
		public override void OnServerCheckpointDoorSequenceChanging(CheckpointDoorSequenceChangingEventArgs ev) => CurrentEvent?.OnServerCheckpointDoorSequenceChanging(ev);
		public override void OnServerCheckpointDoorSequenceChanged(CheckpointDoorSequenceChangedEventArgs ev) => CurrentEvent?.OnServerCheckpointDoorSequenceChanged(ev);
		public override void OnServerPluginsEnabled() => CurrentEvent?.OnServerPluginsEnabled();
		public override void OnWarheadStarting(WarheadStartingEventArgs ev) => CurrentEvent?.OnWarheadStarting(ev);
		public override void OnWarheadStarted(WarheadStartedEventArgs ev) => CurrentEvent?.OnWarheadStarted(ev);
		public override void OnWarheadStopping(WarheadStoppingEventArgs ev) => CurrentEvent?.OnWarheadStopping(ev);
		public override void OnWarheadStopped(WarheadStoppedEventArgs ev) => CurrentEvent?.OnWarheadStopped(ev);
		public override void OnWarheadDetonating(WarheadDetonatingEventArgs ev) => CurrentEvent?.OnWarheadDetonating(ev);
		public override void OnWarheadDetonated(WarheadDetonatedEventArgs ev) => CurrentEvent?.OnWarheadDetonated(ev);
	}
}

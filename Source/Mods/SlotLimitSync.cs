using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace MultiplayerAdaptiveStorageFrameworkPatch.Source.Mods;

/// <summary>
///     Syncs the slot-limit slider (<c>ContentsITab.TryDrawSlider</c> assigns
///     <c>ThingClass.CurrentSlotLimit</c> directly from the UI).
/// </summary>
internal static class SlotLimitSync
{
    internal static void Register()
    {
        // Without sync each client sets a different limit locally and desyncs.
        // Syncing the setter makes the slider assignment replicate to all clients.
        var thingClassType = AccessTools.TypeByName("AdaptiveStorage.ThingClass");
        if (thingClassType == null)
        {
            Log.Error(
                $"{AdaptiveStorageFramework.LogPrefix} Could not find type AdaptiveStorage.ThingClass for slot limit sync.");
            return;
        }

        var setter = AccessTools.PropertySetter(thingClassType, "CurrentSlotLimit");
        if (setter == null)
        {
            Log.Error(
                $"{AdaptiveStorageFramework.LogPrefix} Could not find setter AdaptiveStorage.ThingClass.CurrentSlotLimit for sync.");
            return;
        }

        try
        {
            MP.RegisterSyncMethod(setter)
                .SetContext(SyncContext.MapSelected)
                .CancelIfNoSelectedMapObjects();
        }
        catch (Exception exception)
        {
            Log.Error($"{AdaptiveStorageFramework.LogPrefix} Failed to register slot limit sync: {exception}");
        }
    }
}
using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace MultiplayerAdaptiveStorageFrameworkPatch.Source.Mods;

/// <summary>
///     Fallback sync for <c>AdaptiveStorage.ContentsITab.EjectThing</c>.
/// </summary>
internal static class ContentsTabSync
{
    internal static void Register()
    {
        // Built-in ASF support already registers ContentsITab.EjectThing via MP.RegisterAll
        // (see AdaptiveStorage.ModCompatibility.Multiplayer). Registering again keeps older
        // ASF versions without built-in support working. Duplicate registration is harmless
        // (MP logs/ignores it), so attempt it defensively.
        var contentsTabType = AccessTools.TypeByName("AdaptiveStorage.ContentsITab");
        if (contentsTabType == null)
        {
            Log.Error(
                $"{AdaptiveStorageFramework.LogPrefix} Could not find type AdaptiveStorage.ContentsITab for eject sync.");
            return;
        }

        var ejectMethod = AccessTools.DeclaredMethod(contentsTabType, "EjectThing");
        if (ejectMethod == null)
        {
            Log.Error(
                $"{AdaptiveStorageFramework.LogPrefix} Could not find method AdaptiveStorage.ContentsITab.EjectThing for sync.");
            return;
        }

        try
        {
            MP.RegisterSyncMethod(ejectMethod)
                .SetContext(SyncContext.MapSelected)
                .CancelIfAnyArgNull()
                .CancelIfNoSelectedMapObjects();
        }
        catch (Exception exception)
        {
            Log.Message(
                $"{AdaptiveStorageFramework.LogPrefix} Skipping EjectThing re-registration (likely already synced by the mod itself): {exception.Message}");
        }
    }
}
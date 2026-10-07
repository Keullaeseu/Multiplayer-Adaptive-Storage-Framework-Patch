using HarmonyLib;
using Multiplayer.API;
using Verse;

namespace MultiplayerAdaptiveStorageFrameworkPatch.Source.Mods;

/// <summary>
///     Fallback sync for <c>AdaptiveStorage.GodModeGizmos.AddStackFor</c> (debug-only).
/// </summary>
internal static class GodModeSync
{
    internal static void Register()
    {
        // Built-in ASF support already registers GodModeGizmos.AddStackFor with debugOnly.
        // Re-register defensively for older ASF versions. Right-click spawn lambdas on
        // Command_AddStack bypass AddStackFor, but they are god-mode/dev-only and MP
        // blocks non-debug execution, so they are out of scope for normal gameplay sync.
        var godModeType = AccessTools.TypeByName("AdaptiveStorage.GodModeGizmos");
        if (godModeType == null)
        {
            Log.Error(
                $"{AdaptiveStorageFramework.LogPrefix} Could not find type AdaptiveStorage.GodModeGizmos for god-mode sync.");
            return;
        }

        var addStackMethod = AccessTools.DeclaredMethod(godModeType, "AddStackFor");
        if (addStackMethod == null)
        {
            Log.Error(
                $"{AdaptiveStorageFramework.LogPrefix} Could not find method AdaptiveStorage.GodModeGizmos.AddStackFor for sync.");
            return;
        }

        try
        {
            MP.RegisterSyncMethod(addStackMethod).SetDebugOnly();
        }
        catch (Exception exception)
        {
            Log.Message(
                $"{AdaptiveStorageFramework.LogPrefix} Skipping AddStackFor re-registration (likely already synced by the mod itself): {exception.Message}");
        }
    }
}
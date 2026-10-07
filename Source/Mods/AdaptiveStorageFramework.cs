using Multiplayer.Compat;
using Verse;

namespace MultiplayerAdaptiveStorageFrameworkPatch.Source.Mods;

/// <summary>
///     Multiplayer Patch for Adaptive Storage Framework by Soul, Phaneron, Bradson,
///     Last Update: 14 Jun, 2025 @ 3:22pm
///     <see href="https://steamcommunity.com/sharedfiles/filedetails/?id=3033901359" />
///     <see href="https://github.com/bbradson/Adaptive-Storage-Framework" />
/// </summary>
[MpCompatFor("adaptive.storage.framework")]
public class AdaptiveStorageFramework
{
    internal const string LogPrefix = "[Multiplayer Adaptive Storage Framework Patch]";

    public AdaptiveStorageFramework(ModContentPack content)
    {
        // AdaptiveStorage.ThingExtensions static constructor reads DefDatabase<ThingDef>,
        // so touching AdaptiveStorage types too early throws. Defer everything until defs are loaded.
        LongEventHandler.ExecuteWhenFinished(LatePatch);
    }

    private static void LatePatch()
    {
        Log.Message($"{LogPrefix} Initializing...");

        try
        {
            MpCompatPatchLoader.LoadPatch(typeof(StorageRendererPatch));
        }
        catch (Exception exception)
        {
            Log.Error($"{LogPrefix} Failed to load transpiler patches: {exception}");
        }

        SlotLimitSync.Register();
        ContentsTabSync.Register();
        GodModeSync.Register();

        Log.Message($"{LogPrefix} Initialized.");
    }
}
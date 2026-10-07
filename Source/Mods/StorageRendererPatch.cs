using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Multiplayer.API;
using Multiplayer.Compat;
using Verse;

namespace MultiplayerAdaptiveStorageFrameworkPatch.Source.Mods;

/// <summary>
///     Thread-safety fix for <c>AdaptiveStorage.StorageRenderer.SetPrintDataDirty</c>.
///     Mirrors bbradson/Adaptive-Storage-Framework#30 and rwmt/Multiplayer-Compatibility@23be30e by Sokyran.
/// </summary>
internal static class StorageRendererPatch
{
    private static readonly MethodInfo IsInMainThreadGetter =
        AccessTools.PropertyGetter(typeof(UnityData), nameof(UnityData.IsInMainThread));

    // SetPrintDataDirty creates Unity Graphic objects when called on the main thread. During a
    // multiplayer session those must be deferred (pass null, rebuild later) to avoid touching
    // Unity objects from synced code paths. Original compat used !MP.enabled, we use the more
    // precise !MP.IsInMultiplayer so singleplayer with MP installed keeps the fast path.
    [MpCompatTranspiler("AdaptiveStorage.StorageRenderer", "SetPrintDataDirty")]
    private static IEnumerable<CodeInstruction> SetPrintDataDirty_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
            if (instruction.Calls(IsInMainThreadGetter))
                yield return new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(StorageRendererPatch), nameof(IsInMainThreadAndNotMp)));
            else
                yield return instruction;
    }

    private static bool IsInMainThreadAndNotMp()
    {
        return UnityData.IsInMainThread && !MP.IsInMultiplayer;
    }
}
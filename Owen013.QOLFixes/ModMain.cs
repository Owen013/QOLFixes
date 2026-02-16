using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Reflection;

namespace QOLFixes;

public class ModMain : ModBehaviour
{
    public static ModMain Instance { get; private set; }

    public override void Configure(IModConfig config)
    {
        Config.Configure(config);
    }

    private void Awake()
    {
        Instance = this;
        new Harmony("Owen013.QOLFixes").PatchAll(Assembly.GetExecutingAssembly());
    }

    private void Start()
    {
        ModHelper.Console.WriteLine("Quality of Life Changes is ready to go!", MessageType.Success);
    }
}
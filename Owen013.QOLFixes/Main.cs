using HarmonyLib;
using OWML.ModHelper;
using OWML.Common;
using System.Reflection;

namespace QOLFixes;

public class Main : ModBehaviour
{
    public static Main Instance { get; private set; }

    public delegate void ConfigureEvent();

    public event ConfigureEvent OnConfigure;

    public override void Configure(IModConfig config)
    {
        Config.UpdateConfig(config);
        OnConfigure?.Invoke();
    }

    private void Awake()
    {
        Instance = this;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void Start()
    {
        Log("Quality of Life Changes is ready to go!", MessageType.Success);
    }

    public void Log(string text, MessageType type = MessageType.Message)
    {
        ModHelper.Console.WriteLine(text, type);
    }
}
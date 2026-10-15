using System;
using System.Reflection;
using UnityEditor;

public static class GameViewResolution
{
	private const string QaSizeLabelPrefix = "QA ";
	private const BindingFlags AnyInstanceMember = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	private const BindingFlags AnyStaticMember = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	private static readonly Assembly EditorAssembly = typeof(Editor).Assembly;
	private static readonly Type GameViewType = EditorAssembly.GetType("UnityEditor.GameView");
	private static readonly Type GameViewSizesType = EditorAssembly.GetType("UnityEditor.GameViewSizes");
	private static readonly Type GameViewSizeType = EditorAssembly.GetType("UnityEditor.GameViewSize");
	private static readonly Type GameViewSizeKindType = EditorAssembly.GetType("UnityEditor.GameViewSizeType");

	public static string Apply(int width, int height, string label)
	{
		var openSimulatorWindow = FindOpenSimulatorWindowTitle();
		if (openSimulatorWindow != null)
		{
			return $"ERROR: close the Device Simulator window '{openSimulatorWindow}' first; while it is open Screen and safe-area values come from the simulated device, not the Game view.";
		}

		var gameView = GetGameView();
		var previousIndex = GetSelectedIndex(gameView);
		var group = GetCurrentGroup();
		var index = FindQaSizeIndex(group, width, height);
		if (index < 0)
		{
			var fixedResolution = Enum.Parse(GameViewSizeKindType, "FixedResolution");
			var size = Activator.CreateInstance(GameViewSizeType, fixedResolution, width, height, QaSizeLabelPrefix + label);
			Invoke(group, "AddCustomSize", size);
			index = (int)Invoke(group, "GetTotalCount") - 1;
		}

		SetSelectedIndex(gameView, index);
		return $"OK: selected index {index} ({width}x{height} '{label}'), previous index {previousIndex}";
	}

	public static string Select(int index)
	{
		var gameView = GetGameView();
		SetSelectedIndex(gameView, index);
		return $"OK: selected index {index} ({DescribeSize(GetCurrentGroup(), index)})";
	}

	public static string Describe()
	{
		var gameView = GetGameView();
		var index = GetSelectedIndex(gameView);
		var simulatorWindow = FindOpenSimulatorWindowTitle() ?? "none";
		return $"selected index {index} ({DescribeSize(GetCurrentGroup(), index)}), Device Simulator open: {simulatorWindow}";
	}

	public static string RemoveQaSizes()
	{
		var group = GetCurrentGroup();
		var builtinCount = (int)Invoke(group, "GetBuiltinCount");
		var removedCount = 0;
		for (var index = (int)Invoke(group, "GetTotalCount") - 1; index >= builtinCount; index--)
		{
			var size = Invoke(group, "GetGameViewSize", index);
			if (((string)GetProperty(size, "baseText")).StartsWith(QaSizeLabelPrefix, StringComparison.Ordinal))
			{
				Invoke(group, "RemoveCustomSize", index);
				removedCount++;
			}
		}

		Invoke(GetSizesInstance(), "SaveToHDD");
		return $"OK: removed {removedCount} QA sizes";
	}

	private static int FindQaSizeIndex(object group, int width, int height)
	{
		var builtinCount = (int)Invoke(group, "GetBuiltinCount");
		var totalCount = (int)Invoke(group, "GetTotalCount");
		for (var index = builtinCount; index < totalCount; index++)
		{
			var size = Invoke(group, "GetGameViewSize", index);
			var isQaSize = ((string)GetProperty(size, "baseText")).StartsWith(QaSizeLabelPrefix, StringComparison.Ordinal);
			var isFixedResolution = GetProperty(size, "sizeType").ToString() == "FixedResolution";
			if (isQaSize && isFixedResolution && (int)GetProperty(size, "width") == width && (int)GetProperty(size, "height") == height)
			{
				return index;
			}
		}

		return -1;
	}

	private static string DescribeSize(object group, int index)
	{
		var size = Invoke(group, "GetGameViewSize", index);
		return $"{GetProperty(size, "sizeType")} {GetProperty(size, "width")}x{GetProperty(size, "height")} '{GetProperty(size, "baseText")}'";
	}

	private static EditorWindow GetGameView() => EditorWindow.GetWindow(GameViewType, false, "Game", false);

	private static int GetSelectedIndex(EditorWindow gameView) => (int)GameViewType.GetProperty("selectedSizeIndex", AnyInstanceMember).GetValue(gameView);

	private static void SetSelectedIndex(EditorWindow gameView, int index)
	{
		GameViewType.GetProperty("selectedSizeIndex", AnyInstanceMember).SetValue(gameView, index);
		gameView.Repaint();
	}

	private static object GetSizesInstance()
	{
		var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(GameViewSizesType);
		return singletonType.GetProperty("instance", AnyStaticMember).GetValue(null);
	}

	private static object GetCurrentGroup() => GetProperty(GetSizesInstance(), "currentGroup");

	private static string FindOpenSimulatorWindowTitle()
	{
		foreach (var window in UnityEngine.Resources.FindObjectsOfTypeAll<EditorWindow>())
		{
			if (window.GetType().Name == "SimulatorWindow")
			{
				return window.titleContent.text;
			}
		}

		return null;
	}

	private static object GetProperty(object target, string propertyName) => target.GetType().GetProperty(propertyName, AnyInstanceMember).GetValue(target);

	private static object Invoke(object target, string methodName, params object[] arguments) => target.GetType().GetMethod(methodName, AnyInstanceMember).Invoke(target, arguments);
}

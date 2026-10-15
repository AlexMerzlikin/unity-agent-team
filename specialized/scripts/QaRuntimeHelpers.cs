using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Project-independent Play Mode helpers for <c>unity command run_script</c>. Project-specific entries
/// (start a level, force win or lose) belong in a sibling script next to this one.
/// </summary>
public static class QaRuntimeHelpers
{
	private const BindingFlags PublicInstanceMember = BindingFlags.Instance | BindingFlags.Public;

	public static string Mark(string text)
	{
		Debug.Log(text);
		return "ok";
	}

	public static string Where()
	{
		var report = new StringBuilder();
		report.Append("playing=").Append(Application.isPlaying);
		foreach (var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (canvas.isRootCanvas)
			{
				report.Append(" | ").Append(canvas.name).Append(' ').Append(canvas.pixelRect.width).Append('x').Append(canvas.pixelRect.height);
			}
		}

		report.Append(" | scenes:");
		for (var sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
		{
			report.Append(' ').Append(SceneManager.GetSceneAt(sceneIndex).name);
		}

		report.Append(" | timeScale=").Append(Time.timeScale);
		return report.ToString();
	}

	public static string Reach(string name)
	{
		var target = FindActive(name);
		if (target == null)
		{
			return "NOT_FOUND";
		}

		var rect = (RectTransform)target.transform;
		var point = ScreenCenter(rect);
		var view = rect.GetComponentInParent<Canvas>().rootCanvas.pixelRect;
		var isInsideView = point.x >= view.xMin && point.y >= view.yMin && point.x <= view.xMax && point.y <= view.yMax;
		var firstHit = RaycastTop(point).gameObject;
		var isHitOnTarget = firstHit != null && (firstHit == target || firstHit.transform.IsChildOf(target.transform));
		var verdict = isHitOnTarget && isInsideView ? "REACHABLE" : "BLOCKED";
		return verdict + " center=" + point + " inside=" + isInsideView + " first=" + (firstHit != null ? Path(firstHit) : "none");
	}

	public static string Tap(string name, int times)
	{
		var reachReport = Reach(name);
		if (!reachReport.StartsWith("REACHABLE", StringComparison.Ordinal))
		{
			return "TAP_ABORTED " + reachReport;
		}

		var target = FindActive(name);
		var point = ScreenCenter((RectTransform)target.transform);
		var topHit = RaycastTop(point);
		GameObject pressHandler = null;
		for (var tapIndex = 0; tapIndex < times; tapIndex++)
		{
			pressHandler = TapOnce(target, topHit, point);
		}

		return "TAPPED x" + times + " pressHandler=" + PathOrNone(pressHandler) + " " + reachReport;
	}

	public static string Pointer(string name, string phase)
	{
		if (phase != "down" && phase != "up")
		{
			return "UNKNOWN_PHASE " + phase + " (use down|up)";
		}

		var reachReport = Reach(name);
		if (reachReport == "NOT_FOUND")
		{
			return reachReport;
		}

		var isReachable = reachReport.StartsWith("REACHABLE", StringComparison.Ordinal);
		if (phase == "down" && !isReachable)
		{
			return "PRESS_ABORTED " + reachReport;
		}

		var target = FindActive(name);
		var point = ScreenCenter((RectTransform)target.transform);
		var topHit = RaycastTop(point);
		var currentOverGameObject = isReachable ? topHit.gameObject : target;
		var pointer = CreatePointer(topHit, point);
		pointer.rawPointerPress = currentOverGameObject;
		if (phase == "down")
		{
			ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerEnterHandler);
			var pressHandler = PressDown(currentOverGameObject, pointer);
			return "DOWN pressHandler=" + PathOrNone(pressHandler) + " " + reachReport;
		}

		var releaseHandler = ResolvePressHandler(currentOverGameObject);
		pointer.pointerPress = releaseHandler;
		ExecuteEvents.Execute(releaseHandler, pointer, ExecuteEvents.pointerUpHandler);
		ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerExitHandler);
		return "UP pressHandler=" + PathOrNone(releaseHandler) + " " + reachReport;
	}

	public static string ScreenRect(string name)
	{
		var target = FindActive(name);
		if (target == null)
		{
			return "NONE";
		}

		var rect = (RectTransform)target.transform;
		var corners = new Vector3[4];
		rect.GetWorldCorners(corners);
		var camera = CanvasCamera(rect);
		var bottomLeft = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
		var topRight = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);
		var view = rect.GetComponentInParent<Canvas>().rootCanvas.pixelRect;
		return bottomLeft.x.ToString("0") + "," + bottomLeft.y.ToString("0") + "," + topRight.x.ToString("0") + "," + topRight.y.ToString("0") + "," + view.width.ToString("0") + "," + view.height.ToString("0");
	}

	public static string Buttons()
	{
		var report = new StringBuilder();
		foreach (var button in UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (button.interactable)
			{
				report.AppendLine(Path(button.gameObject));
			}
		}

		return report.ToString();
	}

	public static string Find(string namePrefix)
	{
		var report = new StringBuilder();
		foreach (var rect in UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (rect.name.StartsWith(namePrefix, StringComparison.Ordinal))
			{
				report.AppendLine(Path(rect.gameObject) + " activeInHierarchy=" + rect.gameObject.activeInHierarchy + " size=" + rect.rect.size + " screen=" + ScreenCenter(rect));
			}
		}

		return report.ToString();
	}

	public static string Dump(string rootName)
	{
		var root = FindActive(rootName);
		if (root == null)
		{
			return "NOT_FOUND " + rootName;
		}

		var report = new StringBuilder();
		AppendHierarchy(root.transform, 0, report);
		return report.ToString();
	}

	public static string SetInputText(string inputFieldName, string text)
	{
		foreach (var inputField in UnityEngine.Object.FindObjectsByType<TMPro.TMP_InputField>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (inputField.name == inputFieldName)
			{
				inputField.text = text;
				return "text set -> '" + inputField.text + "'";
			}
		}

		return "input not found";
	}

	public static object Resolve(string containerOwnerTypeName, Type contract)
	{
		foreach (var component in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (component.GetType().Name != containerOwnerTypeName)
			{
				continue;
			}

			var containerProperty = component.GetType().GetProperty("Container", PublicInstanceMember);
			var container = containerProperty?.GetValue(component);
			return container != null ? ResolveFromContainer(container, containerProperty.PropertyType, contract) : null;
		}

		return null;
	}

	public static string Path(GameObject gameObject)
	{
		var path = new StringBuilder(gameObject.name);
		var parent = gameObject.transform.parent;
		while (parent != null)
		{
			path.Insert(0, parent.name + "/");
			parent = parent.parent;
		}

		return path.ToString();
	}

	private static object ResolveFromContainer(object container, Type containerContract, Type contract)
	{
		foreach (var method in containerContract.GetMethods(PublicInstanceMember))
		{
			var arguments = BuildResolveByTypeArguments(method, contract);
			if (arguments != null)
			{
				return method.Invoke(container, arguments);
			}
		}

		return null;
	}

	private static object[] BuildResolveByTypeArguments(MethodInfo method, Type contract)
	{
		var parameters = method.GetParameters();
		var isResolveByType = method.Name == "Resolve" && !method.IsGenericMethodDefinition && parameters.Length > 0 && parameters[0].ParameterType == typeof(Type);
		if (!isResolveByType)
		{
			return null;
		}

		var arguments = new object[parameters.Length];
		arguments[0] = contract;
		for (var parameterIndex = 1; parameterIndex < parameters.Length; parameterIndex++)
		{
			if (!parameters[parameterIndex].IsOptional)
			{
				return null;
			}

			arguments[parameterIndex] = parameters[parameterIndex].DefaultValue;
		}

		return arguments;
	}

	private static GameObject TapOnce(GameObject target, RaycastResult topHit, Vector2 point)
	{
		var currentOverGameObject = topHit.gameObject;
		var pointer = CreatePointer(topHit, point);
		ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerEnterHandler);
		var pressHandler = PressDown(currentOverGameObject, pointer);
		pointer.rawPointerPress = currentOverGameObject;
		ExecuteEvents.Execute(pressHandler, pointer, ExecuteEvents.pointerUpHandler);
		var clickHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGameObject);
		if (clickHandler != null && clickHandler == pressHandler)
		{
			ExecuteEvents.Execute(pressHandler, pointer, ExecuteEvents.pointerClickHandler);
		}

		ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerExitHandler);
		return pressHandler;
	}

	private static PointerEventData CreatePointer(RaycastResult topHit, Vector2 point) => new PointerEventData(EventSystem.current)
	{
		button = PointerEventData.InputButton.Left,
		pointerId = -1,
		position = point,
		pressPosition = point,
		pointerCurrentRaycast = topHit,
		pointerPressRaycast = topHit,
		eligibleForClick = true
	};

	private static GameObject PressDown(GameObject currentOverGameObject, PointerEventData pointer)
	{
		var pressHandler = ExecuteEvents.ExecuteHierarchy(currentOverGameObject, pointer, ExecuteEvents.pointerDownHandler);
		if (pressHandler == null)
		{
			pressHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGameObject);
		}

		pointer.pointerPress = pressHandler;
		return pressHandler;
	}

	private static GameObject ResolvePressHandler(GameObject currentOverGameObject)
	{
		var pressHandler = ExecuteEvents.GetEventHandler<IPointerDownHandler>(currentOverGameObject);
		return pressHandler != null ? pressHandler : ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGameObject);
	}

	private static string PathOrNone(GameObject gameObject) => gameObject != null ? Path(gameObject) : "none";

	private static RaycastResult RaycastTop(Vector2 point)
	{
		var pointer = new PointerEventData(EventSystem.current) { position = point };
		var hits = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, hits);
		return hits.Count > 0 ? hits[0] : default;
	}

	private static GameObject FindActive(string name)
	{
		foreach (var rect in UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
		{
			if (rect.name == name && rect.gameObject.activeInHierarchy)
			{
				return rect.gameObject;
			}
		}

		return null;
	}

	private static Vector2 ScreenCenter(RectTransform rect) => RectTransformUtility.WorldToScreenPoint(CanvasCamera(rect), rect.TransformPoint(rect.rect.center));

	private static Camera CanvasCamera(RectTransform rect)
	{
		var canvas = rect.GetComponentInParent<Canvas>();
		return canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
	}

	private static void AppendHierarchy(Transform node, int depth, StringBuilder report)
	{
		report.Append(new string(' ', depth * 2)).Append(node.name).Append(node.gameObject.activeSelf ? "" : " [inactive]");
		var graphic = node.GetComponent<Graphic>();
		if (graphic != null)
		{
			report.Append(' ').Append(graphic.GetType().Name).Append(" a=").Append(graphic.color.a.ToString("0.00"));
		}

		var button = node.GetComponent<Button>();
		if (button != null)
		{
			report.Append(" Button(interactable=").Append(button.interactable).Append(')');
		}

		if (node is RectTransform rectTransform)
		{
			report.Append(" size=").Append(rectTransform.rect.size);
		}

		report.AppendLine();
		for (var childIndex = 0; childIndex < node.childCount; childIndex++)
		{
			AppendHierarchy(node.GetChild(childIndex), depth + 1, report);
		}
	}
}

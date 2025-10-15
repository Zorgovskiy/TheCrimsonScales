using System;
using System.Collections.Generic;
using Godot;

public static class NodeUtils
{
	public static List<T> GetChildrenOfType<T>(this Node node, bool includeInternal = false)
	{
		List<T> list = new List<T>();

		node.GetChildrenOfType(list, includeInternal);

		return list;
	}

	public static void GetChildrenOfType<T>(this Node node, List<T> list, bool includeInternal = false)
	{
		int childCount = node.GetChildCount(includeInternal);
		for(int i = 0; i < childCount; i++)
		{
			Node child = node.GetChild(i, includeInternal);
			if(child is T castChild)
			{
				list.Add(castChild);
			}

			child.GetChildrenOfType(list, includeInternal);
		}
	}

	public static T GetChildOfType<T>(this Node node, bool includeInternal = false)
	{
		int childCount = node.GetChildCount(includeInternal);
		for(int i = 0; i < childCount; i++)
		{
			Node child = node.GetChild(i, includeInternal);
			if(child is T castChild)
			{
				return castChild;
			}

			T recurseValue = child.GetChildOfType<T>(includeInternal);
			if(recurseValue != null)
			{
				return recurseValue;
			}
		}

		return default;
	}

	public static T GetParentOfType<T>(this Node node, bool includeInternal = false)
	{
		while(node != null)
		{
			if(node is T castNode)
			{
				return castNode;
			}

			node = node.GetParent();
		}

		return default;
	}

	public static void QueueFree(this Node node, float delay)
	{
		DelayedCall(node, node.QueueFree, delay);
	}

	public static void DelayedCall(this Node node, Action action, float delay = 0.01f)
	{
		if(delay <= 0f)
		{
			action?.Invoke();
			return;
		}

		Timer timer = new Timer();
		node.AddChild(timer);
		timer.SetOneShot(true);
		timer.SetWaitTime(delay);
		timer.Start();
		timer.Timeout += timer.QueueFree;
		timer.Timeout += action;
	}
}
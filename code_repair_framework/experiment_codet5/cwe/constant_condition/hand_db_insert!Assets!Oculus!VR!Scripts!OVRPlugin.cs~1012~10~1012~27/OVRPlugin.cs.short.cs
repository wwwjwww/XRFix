using System;
using System.Runtime.InteropServices;
using UnityEngine;

	public enum BoneId
	{
		// BUG: Constant condition
		// MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
		// 		Max = ((int)Hand_End > 50) ? (int)Hand_End : 50,

		//Avoid constant conditions where possible, and either eliminate the conditions or replace them.
		// FIXED CODE:

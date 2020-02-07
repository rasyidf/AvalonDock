﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;

namespace AvalonDock
{
	internal static class MathHelper
	{
		public static double MinMax(double value, double min, double max)
		{
			if (min > max) throw new ArgumentException("The minimum should not be greater then the maximum", nameof(min));
			if (value < min) return min;
			return value > max ? max : value;
		}

		public static void AssertIsPositiveOrZero(double value)
		{
			if (value < 0.0) throw new ArgumentException("Invalid value, must be a positive number or equal to zero", nameof(value));
		}
	}
}

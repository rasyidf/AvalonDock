﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;
using System.Windows.Controls;
using System.Windows.Markup;

namespace AvalonDock.Layout
{
	[ContentProperty(nameof(Children))]
	[Serializable]
	public class LayoutDocumentPaneGroup : LayoutPositionableGroup<ILayoutDocumentPane>, ILayoutDocumentPane, ILayoutOrientableGroup
	{
		#region fields
		private Orientation _orientation;
		#endregion fields

		#region Constructors

		public LayoutDocumentPaneGroup()
		{
		}

		public LayoutDocumentPaneGroup(LayoutDocumentPane documentPane)
		{
			Children.Add(documentPane);
		}

		#endregion Constructors

		#region Properties

		public Orientation Orientation
		{
			get => _orientation;
			set
			{
				if (value == _orientation) return;
				RaisePropertyChanging(nameof(Orientation));
				_orientation = value;
				RaisePropertyChanged(nameof(Orientation));
			}
		}

		#endregion Properties

		#region Overrides

		/// <inheritdoc />
		protected override bool GetVisibility() => true;

		/// <inheritdoc />
		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(nameof(Orientation), Orientation.ToString());
			base.WriteXml(writer);
		}

		public override void ReadXml(System.Xml.XmlReader reader)
		{
			if (reader.MoveToAttribute(nameof(Orientation))) Orientation = (Orientation)Enum.Parse(typeof(Orientation), reader.Value, true);
			base.ReadXml(reader);
		}

#if TRACE
		public override void ConsoleDump(int tab)
		{
			System.Diagnostics.Trace.Write(new string(' ', tab * 4));
			System.Diagnostics.Trace.WriteLine(string.Format("DocumentPaneGroup({0})", Orientation));

			foreach (LayoutElement child in Children)
				child.ConsoleDump(tab + 1);
		}
#endif

		#endregion Overrides
	}
}

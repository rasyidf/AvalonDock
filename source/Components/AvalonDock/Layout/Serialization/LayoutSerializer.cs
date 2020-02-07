﻿/************************************************************************
   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at https://opensource.org/licenses/MS-PL
 ************************************************************************/

using System;
using System.Linq;

namespace AvalonDock.Layout.Serialization
{
	public abstract class LayoutSerializer
	{
		#region fields

		private LayoutAnchorable[] _previousAnchorables = null;
		private LayoutDocument[] _previousDocuments = null;
		#endregion fields

		#region Constructors

		public LayoutSerializer(DockingManager manager)
		{
			Manager = manager ?? throw new ArgumentNullException(nameof(manager));
			_previousAnchorables = Manager.Layout.Descendents().OfType<LayoutAnchorable>().ToArray();
			_previousDocuments = Manager.Layout.Descendents().OfType<LayoutDocument>().ToArray();
		}

		#endregion Constructors

		#region Properties

		public DockingManager Manager { get; }

		#endregion Properties

		#region Events

		public event EventHandler<LayoutSerializationCallbackEventArgs> LayoutSerializationCallback;

		#endregion Events

		#region Methods

		protected virtual void FixupLayout(LayoutRoot layout)
		{
			//fix container panes
			foreach (var lcToAttach in layout.Descendents().OfType<ILayoutPreviousContainer>().Where(lc => lc.PreviousContainerId != null))
			{
				var paneContainerToAttach = layout.Descendents().OfType<ILayoutPaneSerializable>().FirstOrDefault(lps => lps.Id == lcToAttach.PreviousContainerId);
				if (paneContainerToAttach == null)
					throw new ArgumentException($"Unable to find a pane with id ='{lcToAttach.PreviousContainerId}'");
				lcToAttach.PreviousContainer = paneContainerToAttach as ILayoutContainer;
			}

			//now fix the content of the layoutcontents
			foreach (var lcToFix in layout.Descendents().OfType<LayoutAnchorable>().Where(lc => lc.Content == null).ToArray())
			{
				LayoutAnchorable previousAchorable = null;
				if (lcToFix.ContentId != null)
				{
					//try find the content in replaced layout
					previousAchorable = _previousAnchorables.FirstOrDefault(a => a.ContentId == lcToFix.ContentId);
				}

				if (LayoutSerializationCallback != null)
				{
					var args = new LayoutSerializationCallbackEventArgs(lcToFix, previousAchorable?.Content);
					LayoutSerializationCallback(this, args);
					if (args.Cancel)
						lcToFix.Close();
					else if (args.Content != null)
						lcToFix.Content = args.Content;
					else if (args.Model.Content != null)
						lcToFix.Hide(false);
				}
				else if (previousAchorable == null)
					lcToFix.Hide(false);
				else
				{
					lcToFix.Content = previousAchorable.Content;
					lcToFix.IconSource = previousAchorable.IconSource;
				}
			}

			foreach (var lcToFix in layout.Descendents().OfType<LayoutDocument>().Where(lc => lc.Content == null).ToArray())
			{
				LayoutDocument previousDocument = null;
				if (lcToFix.ContentId != null)
				{
					//try find the content in replaced layout
					previousDocument = _previousDocuments.FirstOrDefault(a => a.ContentId == lcToFix.ContentId);
				}

				if (LayoutSerializationCallback != null)
				{
					var args = new LayoutSerializationCallbackEventArgs(lcToFix, previousDocument?.Content);
					LayoutSerializationCallback(this, args);

					if (args.Cancel)
						lcToFix.Close();
					else if (args.Content != null)
						lcToFix.Content = args.Content;
					else if (args.Model.Content != null)
						lcToFix.Close();
				}
				else if (previousDocument == null)
					lcToFix.Close();
				else
				{
					lcToFix.Content = previousDocument.Content;
					lcToFix.IconSource = previousDocument.IconSource;
				}
			}

			layout.CollectGarbage();
		}

		protected void StartDeserialization()
		{
			Manager.SuspendDocumentsSourceBinding = true;
			Manager.SuspendAnchorablesSourceBinding = true;
		}

		protected void EndDeserialization()
		{
			Manager.SuspendDocumentsSourceBinding = false;
			Manager.SuspendAnchorablesSourceBinding = false;
		}
		#endregion Methods
	}
}

#pragma warning disable 8600
#pragma warning disable 8602
#pragma warning disable 8603
#pragma warning disable 8604
#pragma warning disable 8618
#pragma warning disable 8619
#pragma warning disable 8622

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace HananokiLib {
	
	/////////////////////////////////////////
	public static class TextBoxHelper {
		public static string def = "def";
		//TextBox m_textBox;
		public static void setText( this TextBox textBox, string path ) {
			WindowsFormExtended.DoSomethingWithoutEvents(
					textBox,
					() => textBox.Text = path.IsEmpty() ? def : path
					);

			updateTextStatus( textBox );
		}

		public static void updateTextStatus( this TextBox textBox ) {
			if( textBox.Text.IsEmpty() ) {
				textBox.ForeColor = Color.Silver;
				textBox.BackColor = SystemColors.Window;
			}
			else {
				textBox.ForeColor = SystemColors.WindowText;
				if( textBox.Text.IsExistsFile() ) {
					textBox.BackColor = SystemColors.Window;
				}
				else if( def != textBox.Text ) {
					textBox.BackColor = Color.Pink;
				}
			}
		}
	}


	/////////////////////////////////////////
	public static class WindowsFormsExtension {
		public static void SetCenterLocation( this Form form, Form parent ) {
			form.StartPosition = FormStartPosition.Manual;
			form.Left = parent.Left + ( parent.Width - form.Width ) / 2;
			form.Top = parent.Top + ( parent.Height - form.Height ) / 2;
			form.Owner = parent;
}
}


/// 引用 https://tercel-tech.hatenablog.com/entry/2015/07/11/135658
	public static class WindowsFormExtended {
		/// <summary>
		/// 指定したコントロールのイベントを一時的に無効化した状態で、
		/// 所定の処理を実行します
		/// </summary>
		/// <param name="control">対象コントロール</param>
		/// <param name="action">実行したいイベント</param>
		public static void DoSomethingWithoutEvents( Control control, Action action ) {
			if( control == null ) throw new ArgumentNullException();
			if( action == null ) throw new ArgumentNullException();

			var eventHandlerInfo = RemoveAllEvents( control );
			try { action(); }
			finally { RestoreEvents( eventHandlerInfo ); }
		}

		private static List<EventHandlerInfo> RemoveAllEvents( Control root ) {
			var ret = new List<EventHandlerInfo>();
			GetAllControls( root ).ForEach( ( x ) => ret.AddRange( RemoveEvents( x ) ) );
			return ret;
		}

		private static List<Control> GetAllControls( Control root ) {
			var ret = new List<Control>() { root };
			ret.AddRange( GetInnerControls( root ) );
			return ret;
		}

		private static List<Control> GetInnerControls( Control root ) {
			var ret = new List<Control>();
			foreach( Control control in root.Controls ) {
				ret.Add( control );
				ret.AddRange( GetInnerControls( control ) );
			}
			return ret;
		}

		private static EventHandlerList GetEventHandlerList( Control control ) {
			const string EVENTS = "EVENTS";
			const BindingFlags FLAG = BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.IgnoreCase;

			return (EventHandlerList) control.GetType().GetProperty( EVENTS, FLAG )
					.GetValue( control, null );
		}

		private static List<object> GetEvents( Control control ) {
			return GetEvents( control, control.GetType() );
		}

		private static List<object> GetEvents( Control control, Type type ) {
			const string EVENT = "EVENT";
			const BindingFlags FLAG = BindingFlags.Static |
					BindingFlags.NonPublic |
					BindingFlags.DeclaredOnly;

			var ret = type.GetFields( FLAG )
					.Where( ( x ) => x.Name.ToUpper().StartsWith( EVENT ) )
					.Select( ( x ) => x.GetValue( control ) ).ToList();

			if( !type.Equals( typeof( Control ) ) ) ret.AddRange( GetEvents( control, type.BaseType ) );

			return ret;
		}

		private static List<EventHandlerInfo> RemoveEvents( Control control ) {
			var ret = new List<EventHandlerInfo>();
			var list = GetEventHandlerList( control );

			foreach( var x in GetEvents( control ) ) {
				ret.Add( new EventHandlerInfo( x, list, list[ x ] ) );
				list.RemoveHandler( x, list[ x ] );
			}
			return ret;
		}

		private static void RestoreEvents( List<EventHandlerInfo> eventInfoList ) {
			if( eventInfoList == null ) return;
			eventInfoList.ForEach( ( x ) => x.EventHandlerList.AddHandler( x.Key, x.EventHandler ) );
		}

		private sealed class EventHandlerInfo {
			public EventHandlerInfo( object key, EventHandlerList eventHandlerList, Delegate eventHandler ) {
				Key = key;
				EventHandlerList = eventHandlerList;
				EventHandler = eventHandler;
			}
			public object Key { get; private set; }
			public EventHandlerList EventHandlerList { get; private set; }
			public Delegate EventHandler { get; private set; }
		}
	}
}


namespace PropertyGridExtensionHacks {
	/// 引用 https://stackoverflow.com/questions/14550468/propertygrid-control-modify-the-position-of-the-central-splitting-vertical-lin
	public static class PropertyGridExtensions {
		/// <summary>
		/// Gets the (private) PropertyGridView instance.
		/// </summary>
		/// <param name="propertyGrid">The property grid.</param>
		/// <returns>The PropertyGridView instance.</returns>
		private static object GetPropertyGridView( PropertyGrid propertyGrid ) {
			//private PropertyGridView GetPropertyGridView();
			//PropertyGridView is an internal class...
			MethodInfo methodInfo = typeof( PropertyGrid ).GetMethod( "GetPropertyGridView", BindingFlags.NonPublic | BindingFlags.Instance );
			return methodInfo.Invoke( propertyGrid, new object[] { } );
		}

		/// <summary>
		/// Gets the width of the left column.
		/// </summary>
		/// <param name="propertyGrid">The property grid.</param>
		/// <returns>
		/// The width of the left column.
		/// </returns>
		public static int GetInternalLabelWidth( this PropertyGrid propertyGrid ) {
			//System.Windows.Forms.PropertyGridInternal.PropertyGridView
			object gridView = GetPropertyGridView( propertyGrid );

			//protected int InternalLabelWidth
			PropertyInfo propInfo = gridView.GetType().GetProperty( "InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance );
			return (int) propInfo.GetValue( gridView );
		}

		/// <summary>
		/// Moves the splitter to the supplied horizontal position.
		/// </summary>
		/// <param name="propertyGrid">The property grid.</param>
		/// <param name="xpos">The horizontal position.</param>
		public static void MoveSplitterTo( this PropertyGrid propertyGrid, int xpos ) {
			//System.Windows.Forms.PropertyGridInternal.PropertyGridView
			object gridView = GetPropertyGridView( propertyGrid );

			//private void MoveSplitterTo(int xpos);
			MethodInfo methodInfo = gridView.GetType().GetMethod( "MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance );
			methodInfo.Invoke( gridView, new object[] { xpos } );
		}
	}
}


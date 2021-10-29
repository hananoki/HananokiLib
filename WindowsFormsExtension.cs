using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace HananokiLib {

	public static class WindowsFormsExtension {
		public static void SetDoubleBuffered( this ListView listview, bool b ) {
			PropertyInfo prop = listview.GetType().GetProperty( "DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic );
			prop.SetValue( listview, b, null );
		}

		public static void invalidate( this ListViewItem item ) {
			item.ListView?.Invalidate( item.Bounds );
		}

		public static void invertChecked( this ListViewItem item ) {
			item.Checked = !item.Checked;
			item.ListView?.Invalidate( item.Bounds );
		}

	}


	/**
	https://tercel-tech.hatenablog.com/entry/2015/07/11/135658 から引用
	*/
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
				this.Key = key;
				this.EventHandlerList = eventHandlerList;
				this.EventHandler = eventHandler;
			}
			public object Key { get; private set; }
			public EventHandlerList EventHandlerList { get; private set; }
			public Delegate EventHandler { get; private set; }
		}
	}
}

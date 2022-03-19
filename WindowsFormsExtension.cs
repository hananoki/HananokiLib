using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace HananokiLib {

	//////////////////////////////////////////////////////////////////////////////////
	public class StatusBarMessage {
		ToolStripStatusLabel m_label;
		Form m_form;
		public Timer m_timer;

		public enum NotifyType {
			None,
			Info,
			Warning,
			Error,
		}


		public StatusBarMessage( Form form, ToolStripStatusLabel label ) {
			m_label = label;
			m_form = form;

			m_label.Text = "";

			m_timer = new Timer();
			m_timer.Tick += new EventHandler( ( s, ee ) => {
				m_label.Text = "";
				m_label.Image = null;
				m_timer.Stop();
			} );
		}


		/////////////////////////////////////////
		public void setNotifyText( string text = "", NotifyType type = NotifyType.Info, int interval = 10000 ) {

			m_form.Invoke( new Action( () => {
				m_label.Text = text;
				switch( type ) {
				case NotifyType.None:
					m_label.Image = null;
					break;
				case NotifyType.Info:
					m_label.Image = icon.info;
					break;
				case NotifyType.Warning:
					m_label.Image = icon.warning;
					break;
				case NotifyType.Error:
					m_label.Image = icon.error;
					break;
				}
				m_timer.Stop();
				if( 1 <= interval ) {
					m_timer.Interval = interval;
					m_timer.Start();
				}
			} ) );
		}


		/////////////////////////////////////////
		public void clearNotifyText() {
			m_label.Text = "";
			m_label.Image = null;
		}

	}

	//////////////////////////////////////////////////////////////////////////////////
	public class TListView<TItem> : ListView where TItem : ListViewItem {
		public List<TItem> m_items = new List<TItem>();

		public TListView() {
			this.VirtualMode = true;
			this.SetDoubleBuffered( true );
			this.RetrieveVirtualItem += OnRetrieveVirtualItem;
			this.MouseClick += OnMouseClick;
			this.DoubleClick += OnDoubleClick;
		}

		public void AddItem( TItem item ) {
			m_items.Add( item );
		}
		public void ClearItems() {
			//Items.Clear();
			m_items.Clear();
		}

		public TItem GetSelectItem() {
			return m_items[ SelectedIndices[ 0 ] ];
		}

		public void ApplyVirtualListSize() {
			VirtualListSize = m_items.Count;
		}

		void OnRetrieveVirtualItem( object sender, RetrieveVirtualItemEventArgs e ) {
			var lstView = (ListView) sender;
			//var items = (List<TItem>) lstView.Tag;

			if( m_items.Count <= e.ItemIndex ) return;
			//if( items == null ) return;

			e.Item = m_items[ e.ItemIndex ];
		}

		public virtual void OnDoubleClick( object sender, EventArgs e ) {
			OnDoubleClicked( m_items[ SelectedIndices[ 0 ] ] as TItem );
		}
		public virtual void OnDoubleClicked( TItem item ) { }

		void OnMouseClick( object sender, MouseEventArgs e ) {
			var listview = (ListView) sender;
			var item = listview.GetItemAt( e.X, e.Y ) as TItem;

			if( item != null ) {
				if( e.X < ( item.Bounds.Left + 16 ) ) {
					item.invertChecked();
					OnMouseClicked( item );
				}
			}
		}
		public virtual void OnMouseClicked( TItem item ) {
		}
	}


	//////////////////////////////////////////////////////////////////////////////////
	public class TextBoxGuide : TextBox {
		//TextBox m_txtbox;
		public string def = "def";
		Func<string, string> m_setAction;
		public bool checkMode;

		public Func<TextBoxGuide, bool> onValidate;

		public TextBoxGuide() {
			onValidate = ( self ) => {
				return Text.isExistsFile() || Text.isExistsDirectory();
			};
		}

		public void init( string msg, Func<string, string> setText = null ) {
			//m_txtbox = t;
			AllowDrop = true;
			if( setText == null ) {
				m_setAction = InText => InText;
			}
			else {
				m_setAction = setText;
			}
			TextChanged += onTextChanged;
			DragEnter += onDragEnter;
			DragDrop += onDragDrop;
			Enter += onEnter;
			Leave += onLeave;
			def = msg;
			checkMode = true;

			//if( m_dic == null ) {
			//	m_dic = new Dictionary<TextBox, TextBoxGuide>();
			//}
			//m_dic.Add( this, this );
		}


		//TextBox m_textBox;
		public void setText( string path ) {
			WindowsFormExtended.DoSomethingWithoutEvents(
					this,
					() => Text = path.isEmpty() ? def : path
					);

			updateTextStatus();
		}


		public void updateTextStatus() {
			if( Text.isEmpty() || Text == def ) {
				ForeColor = Color.Silver;
				BackColor = SystemColors.Window;
			}
			else {
				ForeColor = SystemColors.WindowText;
				if( onValidate( this ) ) {
					BackColor = SystemColors.Window;
				}
				else if( def != Text && checkMode ) {
					BackColor = Color.Pink;
				}
			}
		}


		void onLeave( object sender, EventArgs e ) {
			var txtbox = (TextBoxGuide) sender;

			//var te = m_dic[ txtbox ];

			if( txtbox.Text.Length <= 0 || txtbox.Text == txtbox.def ) {
				txtbox.Text = txtbox.def;
				txtbox.ForeColor = Color.Silver;
			}
			else {
				txtbox.ForeColor = SystemColors.WindowText;
			}
		}

		void onEnter( object sender, EventArgs e ) {
			var txtbox = (TextBoxGuide) sender;

			//var te = m_dic[ txtbox ];

			if( txtbox.Text == txtbox.def ) {
				txtbox.Text = string.Empty;
				txtbox.ForeColor = SystemColors.WindowText;
			}
		}


		void onDragEnter( object sender, DragEventArgs e ) {
			var txtbox = (TextBoxGuide) sender;

			//var te = m_dic[ txtbox ];
			if( e.Data.GetDataPresent( DataFormats.FileDrop ) ) {

				// ドラッグ中のファイルやディレクトリの取得
				var drags = (string[]) e.Data.GetData( DataFormats.FileDrop );

				//foreach( var d in drags ) {
				//	if( !File.Exists( d ) ) {
				//		// ファイル以外であればイベント・ハンドラを抜ける
				//		return;
				//	}
				//}

				e.Effect = DragDropEffects.Link;
			}
		}


		void onDragDrop( object sender, DragEventArgs e ) {
			var txtbox = (TextBoxGuide) sender;

			//var te = m_dic[ txtbox ];
			string[] files = (string[]) e.Data.GetData( DataFormats.FileDrop );

			//listBox1.Items.AddRange( files ); // リストボックスに表示
			if( txtbox.m_setAction != null ) {
				txtbox.Text = txtbox.m_setAction?.Invoke( files[ 0 ] );
			}
			else {
				txtbox.Text = files[ 0 ];
			}
		}


		void onTextChanged( object sender, EventArgs e ) {
			var txtbox = (TextBoxGuide) sender;

			//var te = m_dic[ txtbox ];

			if( txtbox.Text != txtbox.def ) {
				//MainForm.config.sevenZipPath = txtbox.Text;
				var ss = m_setAction?.Invoke( txtbox.Text );
				WindowsFormExtended.DoSomethingWithoutEvents(
					txtbox,
					() => txtbox.Text = ss
					);
			}
			txtbox.updateTextStatus();
			//MainForm.config.save();
		}
	}


	/////////////////////////////////////////
	public static class TextBoxHelper {
		public static string def = "def";
		//TextBox m_textBox;
		public static void setText( this TextBox textBox, string path ) {
			WindowsFormExtended.DoSomethingWithoutEvents(
					textBox,
					() => textBox.Text = path.isEmpty() ? def : path
					);

			updateTextStatus( textBox );
		}

		public static void updateTextStatus( this TextBox textBox ) {
			if( textBox.Text.isEmpty() ) {
				textBox.ForeColor = Color.Silver;
				textBox.BackColor = SystemColors.Window;
			}
			else {
				textBox.ForeColor = SystemColors.WindowText;
				if( textBox.Text.isExistsFile() ) {
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
		public static void SetDoubleBuffered( this ListView listview, bool b ) {
			PropertyInfo prop = listview.GetType().GetProperty( "DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic );
			prop.SetValue( listview, b, null );
		}

		public static void ApplyVirtualListSize( this ListView listview ) {
			var items = GetListViewItems( listview );
			if( items == null ) return;
			listview.VirtualListSize = items.Count();
		}

		public static List<ListViewItem> GetListViewItems( this ListView listview ) {
			return listview.Tag as List<ListViewItem>;
		}

		public static void invalidate( this ListViewItem item ) {
			item.ListView?.Invalidate( item.Bounds );
		}

		public static void invertChecked( this ListViewItem item ) {
			item.Checked = !item.Checked;
			item.ListView?.Invalidate( item.Bounds );
		}

		public static string[] GetCheckedStringArray( this CheckedListBox chkListBox ) {
			var lst = new List<string>( 64 );
			foreach( var a in chkListBox.CheckedItems ) lst.Add( a.ToString() );
			return lst.ToArray();
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

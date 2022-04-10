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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace HananokiLib {

	//////////////////////////////////////////////////////////////////////////////////
	public class StatusBarMessage {
		ToolStripStatusLabel m_label;
		Form m_form;
		public System.Windows.Forms.Timer m_timer;

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

			m_timer = new System.Windows.Forms.Timer();
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
	public class TextBoxGuide : TextBox {
		//TextBox m_txtbox;
		public string def = "def";
		Func<TextBoxGuide, string, string> m_setAction;
		public bool checkMode;

		public Func<TextBoxGuide, bool> onValidate;

		/////////////////////////////////////////
		public TextBoxGuide() {
			onValidate = ( self ) => {
				return Text.IsExistsFile() || Text.IsExistsDirectory();
			};
		}

		/////////////////////////////////////////
		public void Init( string msg, Func<TextBoxGuide,string, string>? setText = null ) {
			//m_txtbox = t;
			AllowDrop = true;
			if( setText == null ) {
				m_setAction = (self, InText) => InText;
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


		/////////////////////////////////////////
		public bool IsValidText() {
			if( Text == def ) return false;
			if( Text.IsEmpty() ) return false;
			return true;
		}


		/////////////////////////////////////////
		public void SetText( string path ) {
			WindowsFormExtended.DoSomethingWithoutEvents(
					this,
					() => Text = path.IsEmpty() ? def : path
					);

			UpdateTextStatus();
		}


		/////////////////////////////////////////
		public void UpdateTextStatus() {
			if( Text.IsEmpty() || Text == def ) {
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


		/////////////////////////////////////////
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

				// �h���b�O���̃t�@�C����f�B���N�g���̎擾
				var drags = (string[]) e.Data.GetData( DataFormats.FileDrop );

				//foreach( var d in drags ) {
				//	if( !File.Exists( d ) ) {
				//		// �t�@�C���ȊO�ł���΃C�x���g�E�n���h���𔲂���
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

			//listBox1.Items.AddRange( files ); // ���X�g�{�b�N�X�ɕ\��
			if( txtbox.m_setAction != null ) {
				txtbox.Text = txtbox.m_setAction?.Invoke( this, files[ 0 ] );
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
				var ss = m_setAction?.Invoke( this, txtbox.Text );
				WindowsFormExtended.DoSomethingWithoutEvents(
					txtbox,
					() => txtbox.Text = ss
					);
			}
			txtbox.UpdateTextStatus();
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

		/// <summary>
		/// �`�F�b�N���X�g�{�b�N�X����`�F�b�N���ꂽ�A�C�e���̕������z��ŕԂ�
		/// </summary>
		/// <param name="chkListBox"></param>
		/// <returns></returns>
		public static string[] GetCheckedStringArray( this CheckedListBox chkListBox ) {
			var lst = new List<string>( 64 );
			foreach( var a in chkListBox.CheckedItems ) lst.Add( a.ToString() );
			return lst.ToArray();
		}

		/// <summary>
		/// �w�肵��������̔z�񂩂�Y������`�F�b�N���X�g�{�b�N�X�̃A�C�e�����`�F�b�N����
		/// </summary>
		/// <param name="chkListBox"></param>
		/// <param name="checkedStrings"></param>
		public static void SetCheckedFromStringArray( this CheckedListBox chkListBox, string[] checkedStrings ) {
			SetCheckedFromStringList( chkListBox, new List<string>( checkedStrings ) );
		}

		public static void SetCheckedFromStringList( this CheckedListBox chkListBox, List<string> checkedStrings ) {
			for( int i = 0; i < chkListBox.Items.Count; i++ ) {
				var p = chkListBox.Items[ i ] as string;

				var result = 0 <= checkedStrings.IndexOf( p );
				chkListBox.SetItemChecked( i, result );
			}
		}

	}


	/**
	https://tercel-tech.hatenablog.com/entry/2015/07/11/135658 ������p
	*/
	public static class WindowsFormExtended {
		/// <summary>
		/// �w�肵���R���g���[���̃C�x���g���ꎞ�I�ɖ�����������ԂŁA
		/// ����̏��������s���܂�
		/// </summary>
		/// <param name="control">�ΏۃR���g���[��</param>
		/// <param name="action">���s�������C�x���g</param>
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


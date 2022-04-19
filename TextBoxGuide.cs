
using System.Drawing;
using System.Windows.Forms;
using System;

namespace HananokiLib {

	public class TextBoxGuide : TextBox {

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
		public void Init( string msg, Func<TextBoxGuide, string, string>? setText = null ) {
			//m_txtbox = t;
			AllowDrop = true;
			if( setText == null ) {
				m_setAction = ( self, InText ) => InText;
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
}

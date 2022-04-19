
using System.Windows.Forms;
using System;

namespace HananokiLib {

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


		/////////////////////////////////////////
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
		public void SetNotifyText( string text = "", NotifyType type = NotifyType.Info, int interval = 10000 ) {
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
		public void ClearNotifyText() {
			m_label.Text = "";
			m_label.Image = null;
		}
	}
}

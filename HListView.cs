#pragma warning disable 8622

using System.Collections.Generic;
using System.Windows.Forms;
using System;


namespace HananokiLib {

	public class HListView<TItem> : ListView where TItem : ListViewItem {

		public List<TItem> m_items = new List<TItem>();
		public int doubleClickIndex;


		/////////////////////////////////////////
		public HListView() {
			this.VirtualMode = true;
			this.SetDoubleBuffered( true );
			this.RetrieveVirtualItem += OnRetrieveVirtualItem;
			this.MouseClick += OnMouseClick;
			this.DoubleClick += OnDoubleClick;
			this.KeyUp += OnKeyUp;
		}


		/////////////////////////////////////////
		public void AddItem( TItem item ) {
			m_items.Add( item );
		}


		/////////////////////////////////////////
		public void ClearItems() {
			//Items.Clear();
			m_items.Clear();
		}


		/////////////////////////////////////////
		public TItem GetSelectItem() {
			return m_items[ SelectedIndices[ 0 ] ];
		}


		/////////////////////////////////////////
		public TItem GetDoubleClickItem() {
			return m_items[ doubleClickIndex ];
		}


		/////////////////////////////////////////
		public TItem[] GetSelectItems() {
			var lst = new List<TItem>();
			foreach( var i in SelectedIndices ) lst.Add( m_items[ (int) i ] );
			return lst.ToArray();
		}


		/////////////////////////////////////////
		public void ApplyVirtualListSize() {
			VirtualListSize = 0;
			VirtualListSize = m_items.Count;
		}


		/////////////////////////////////////////
		void OnRetrieveVirtualItem( object sender, RetrieveVirtualItemEventArgs e ) {
			var lstView = (ListView) sender;
			//var items = (List<TItem>) lstView.Tag;

			if( m_items.Count <= e.ItemIndex ) return;
			//if( items == null ) return;

			e.Item = m_items[ e.ItemIndex ];
		}


		/////////////////////////////////////////
		void OnKeyUp( object sender, KeyEventArgs e ) {
			if( e.KeyCode == Keys.F2 && FocusedItem != null && LabelEdit ) {
				FocusedItem.BeginEdit();
			}
		}


		/////////////////////////////////////////
		public virtual void OnDoubleClick( object sender, EventArgs e ) {
			doubleClickIndex = SelectedIndices[ 0 ];
			OnDoubleClicked( m_items[ doubleClickIndex ] as TItem );
		}


		/////////////////////////////////////////
		public virtual void OnDoubleClicked( TItem item ) { }


		/////////////////////////////////////////
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


		/////////////////////////////////////////
		public virtual void OnMouseClicked( TItem item ) {
		}
	}
}

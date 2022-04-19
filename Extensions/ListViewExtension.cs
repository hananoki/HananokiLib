#pragma warning disable 8600 // Null ���e�����܂��� Null �̉\��������l�� Null �񋖗e�^�ɕϊ����Ă��܂��B
#pragma warning disable 8602 // null �Q�Ƃ̉\����������̂̋t�Q�Ƃł��B
#pragma warning disable 8603 // Null �Q�Ɩ߂�l�ł���\��������܂��B
#pragma warning disable 8605 // null �̉\��������l���{�b�N�X���������Ă��܂��B


using HananokiLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;


namespace HananokiLib {

	public static class ListViewExtensions {
		public static void SetDoubleBuffered( this ListView listview, bool b ) {
			if( listview == null ) return;
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
	}
}

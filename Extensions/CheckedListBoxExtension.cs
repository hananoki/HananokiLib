
using System.Collections.Generic;
using System.Windows.Forms;

namespace HananokiLib {
	public static class CheckedListBoxExtensions {

		/// <summary>
		/// チェックリストボックスからチェックされたアイテムの文字列を配列で返す
		/// </summary>
		/// <param name="chkListBox"></param>
		/// <returns></returns>
		public static string[] GetCheckedStringArray( this CheckedListBox chkListBox ) {
			var lst = new List<string>( 64 );
			foreach( var a in chkListBox.CheckedItems ) lst.Add( a.ToString() );
			return lst.ToArray();
		}


		/// <summary>
		/// 指定した文字列の配列から該当するチェックリストボックスのアイテムをチェックする
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
}


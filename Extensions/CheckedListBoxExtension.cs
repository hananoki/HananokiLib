
using System.Collections.Generic;
using System.Windows.Forms;

namespace HananokiLib {
	public static class CheckedListBoxExtensions {

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
}


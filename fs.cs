
using System.IO;


namespace HananokiLib {
	public static class fs {

		public static void mkDir( string path ) {
			if( !Directory.Exists( path ) ) {
				Directory.CreateDirectory( path );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dst"></param>
		/// <param name="overwrite">上書きする場合はtrue</param>
		public static void cp( string src, string dst, bool overwrite = false ) {

			if( !overwrite ) {
				if( File.Exists( dst ) ) return;
			}
			else {
				// ファイル属性によって上書き出来ないので変更する
				if( File.Exists( dst ) ) {
					// ファイル属性を取得
					var fa = File.GetAttributes( dst );

					// 読み取り専用属性を削除（他の属性は変更しない）
					fa = fa & ~FileAttributes.ReadOnly;
					fa = fa & ~FileAttributes.Hidden;
					File.SetAttributes( dst, fa );
				}
			}

			var dname = Path.GetDirectoryName( dst );
			if( !dname.isEmpty() ) {
				if( !Directory.Exists( dname ) ) {
					Directory.CreateDirectory( dname );
				}
			}

			if( File.Exists( src ) ) {
				File.Copy( src, dst, overwrite );
			}
			if( Directory.Exists( src ) ) {
				//DirectoryUtils.DirectoryCopy( src, dst );
				Debug.Warning( "DirectoryUtils.DirectoryCopy" );
			}
		}


		public static void mv( string src, string dst ) {

			if( File.Exists( src ) ) {
				if( File.Exists( dst ) ) {
					File.Delete( dst );
				}
				File.Move( src, dst );
			}

			if( Directory.Exists( src ) ) {
				if( Directory.Exists( dst ) ) {
					Directory.Delete( dst );
				}
				Directory.Move( src, dst );
			}
		}


		public static void rm( string path, bool recursive = false ) {
			try {
				if( Directory.Exists( path ) ) {
					Directory.Delete( path, recursive );
				}
				else if( File.Exists( path ) ) {
					File.Delete( path );
				}
			}
			//catch( DirectoryNotFoundException e ) {
			//	Debug.LogException( e );
			//}
			catch( System.Exception e ) {
				Debug.Exception( e );
			}
		}

		public static int rm2( string fullPath ) {

			var sf = new Win32.SHFILEOPSTRUCT();

			sf.wFunc = Win32.FileFuncFlags.FO_DELETE; // 削除を指示します。
			sf.fFlags = Win32.FILEOP_FLAGS.FOF_ALLOWUNDO; //「元に戻す」を有効にします。
																										//sf.fFlags = sf.fFlags | FILEOP_FLAGS.FOF_NOERRORUI; //エラー画面を表示しません。
			// 単体ファイルの削除なので複数ファイル削除で使用されると進捗ダイアログがOn/Offが頻発する
			// 結果的にメインウィンドウのフォーカスが入れ食い状態になる
			sf.fFlags = sf.fFlags | Win32.FILEOP_FLAGS.FOF_SILENT; //進捗ダイアログを表示しません。
																											 //sf.fFlags = sf.fFlags | FILEOP_FLAGS.FOF_NOCONFIRMATION; //削除確認ダイアログを表示しません。
			sf.pFrom = fullPath + "\0";

			return Win32.SHFileOperation( ref sf );
		}



		/// 引用 https://www.curict.com/item/48/480f4d2.html
		public static bool isWritableFile( string filePath ) {
			try {
				using( FileStream fp = File.Open( filePath, FileMode.Open, FileAccess.Write ) ) {
					return true;
				}
			}
			catch {
				return false;
			}
		}
	}
}


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
		/// <param name="overwrite">�㏑������ꍇ��true</param>
		public static void cp( string src, string dst, bool overwrite = false ) {

			if( !overwrite ) {
				if( File.Exists( dst ) ) return;
			}
			else {
				// �t�@�C�������ɂ���ď㏑���o���Ȃ��̂ŕύX����
				if( File.Exists( dst ) ) {
					// �t�@�C���������擾
					var fa = File.GetAttributes( dst );

					// �ǂݎ���p�������폜�i���̑����͕ύX���Ȃ��j
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
	}
}

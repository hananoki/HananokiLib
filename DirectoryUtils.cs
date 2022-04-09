using System.IO;
using System.Linq;


namespace HananokiLib {
	public static class DirectoryUtils {
		public static string Prettyfy( string dir ) {
			return dir.separatorToOS().TrimEnd( Path.DirectorySeparatorChar );
		}


		public static string[] GetFiles( string path, string searchPattern = "", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
			var _path = path.separatorToOS();

			if( !Directory.Exists( _path ) ) return new string[ 0 ];

			return Directory
					.GetFiles( _path, searchPattern, searchOption )
					.Select( c => Prettyfy( c ) )
					.ToArray();
		}


		public static string[] GetDirectories( string path, string searchPattern = "", SearchOption searchOption = SearchOption.TopDirectoryOnly ) {
			var _path = path.separatorToOS();

			if( !Directory.Exists( _path ) ) return new string[ 0 ];

			return Directory
					.GetDirectories( _path, searchPattern, searchOption )
					.Select( c => Prettyfy( c ) )
					.ToArray();
		}

	}
}

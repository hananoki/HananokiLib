using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;


namespace HananokiLib {

	class Helper {

		public static string s_appName;
		public static string s_appPath;
		public static ParallelOptions s_parallelOptions = new ParallelOptions();

		public static readonly Color LVBKColor = Color.FromArgb( 241, 244, 247 );

		public static string configPath {
			get {
				return $"{s_appPath}\\{s_appName}.json";
			}
		}

		public static void _init() {
			var location = Assembly.GetExecutingAssembly().Location;
			s_appName = location.getBaseName();

			var exePath = Directory.GetParent( location );
			s_appPath = exePath.FullName;

			var info = new Win32.SystemInfo();
			Win32.GetSystemInfo( out info );

			if( 1 < info.dwNumberOfProcessors ) {
				s_parallelOptions.MaxDegreeOfParallelism = (int) info.dwNumberOfProcessors - 1;
			}
		}

		

		public static bool hasDependUpdate( string srcFilePath, string dstFilePath ) {
			var dst = new FileInfo( dstFilePath );
			var src = new FileInfo( srcFilePath );
			if( src.LastWriteTime <= dst.LastWriteTime ) {
				return false;
			}
			return true;
		}


		public static void WriteJson( object obj, string filepath, bool newline = true ) {
			using( var st = new StreamWriter( filepath ) ) {
				try {
					string json = JsonUtils.ToJson( obj, newline );
					st.Write( json );
				}
				catch( Exception e ) {
					Debug.Exception( e );
					LogWindow.Visible = true;
				}
			}
		}

		public static void WriteJson( object obj, bool newline = true ) {
			WriteJson( obj, configPath, true );
		}

		public static bool ReadJson<T>( ref T obj, string filepath ) where T : new() {
			try {
				using( var st = new StreamReader( filepath ) ) {
					obj = LitJson.JsonMapper.ToObject<T>( st.ReadToEnd() );
					if( obj == null ) {
						obj = new T();
					}
				}
			}
			catch( FileNotFoundException ) {
				Debug.Log( $"FileNotFoundException: {filepath} が見つかりません" );
				return false;
			}
			catch( Exception ) {
			}
			return true;
		}
	}
}


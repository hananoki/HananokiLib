#pragma warning disable 8600
#pragma warning disable 8603
#pragma warning disable 8618
#pragma warning disable 8625

using System;
using System.Collections.Generic;
using System.IO;


namespace HananokiLib {

	public static class sound {

		static Dictionary<string, byte[]> m_cache;

		static byte[] readBinaryFile( string filePath ) {
			byte[] buf = null;
			if( File.Exists( filePath ) ) {
				using( var fs = new FileStream( filePath, FileMode.Open, FileAccess.Read ) )
				using( var r = new BinaryReader( fs ) ) {
					buf = r.ReadBytes( (int) fs.Length );
				}
			}
			return buf;
		}

		public static void clear() {
			m_cache?.Clear();
			m_cache = null;
		}

		public static void addData( string label, string filePath ) {
			if( filePath.IsEmpty() ) return;

			if( m_cache == null ) {
				m_cache = new Dictionary<string, byte[]>();
			}
			if( m_cache.ContainsKey( label ) ) {
				Debug.Warning( $"addData: {label} {filePath}" );
				return;
			}
			m_cache.Add( label, readBinaryFile( filePath ) );
		}


		public static void play( string label ) {
			if( m_cache == null ) return;

			byte[] data;
			m_cache.TryGetValue( label , out data );

			if( data == null ) return;

			Win32.PlaySound( data, IntPtr.Zero, Win32.PlaySoundFlags.SND_MEMORY | Win32.PlaySoundFlags.SND_ASYNC );
		}
	} // class

}

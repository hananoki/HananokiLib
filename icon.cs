
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace HananokiLib {

	public static class icon {

		static Bitmap m_bmpInfoCache;
		static Bitmap m_bmpWarningCache;
		static Bitmap m_bmpErrorCache;

		static Dictionary<string, Bitmap> m_bmpCache;

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static void icon2bmp( ref Bitmap bmp, Icon icon ) {
			if( bmp != null ) return;
			bmp = icon.ToBitmap();
		}

		
		public static Bitmap info {
			get {
				icon2bmp( ref m_bmpInfoCache, SystemIcons.Information );
				return m_bmpInfoCache;
			}
		}


		public static Bitmap warning {
			get {
				icon2bmp( ref m_bmpWarningCache, SystemIcons.Warning );
				return m_bmpWarningCache;
			}
		}


		public static Bitmap error {
			get {
				icon2bmp( ref m_bmpErrorCache, SystemIcons.Error );
				return m_bmpErrorCache;
			}
		}



		public static void add( string name, Icon icon ) {
			if( m_bmpCache == null ) {
				m_bmpCache = new Dictionary<string, Bitmap>();
			}
			m_bmpCache.Add( name, icon.ToBitmap() );
		}


		public static Bitmap get( string name ) {
			Bitmap bmp;
			m_bmpCache.TryGetValue( name, out bmp );
			return bmp;
		}
	}
}

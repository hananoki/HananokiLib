#pragma warning disable 8600
#pragma warning disable 8603
#pragma warning disable 8618

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

#if ENABLE_SHELL32_DLL
		/////////////////////////////////////////
		public static Bitmap file( string _filepath ) {
			string filepath = _filepath.separatorToOS();

			if( m_bmpCache == null ) {
				m_bmpCache = new Dictionary<string, Bitmap>();
			}
			if( filepath.isEmpty() ) return null;
			var ext = filepath.getExt();
			Bitmap bmp;
			m_bmpCache.TryGetValue( ext, out bmp );
			if( bmp != null ) return bmp;
			if( !filepath.isExistsFile() ) return null;

			var shinfo = new Win32.SHFILEINFO();
			IntPtr hSuccess = Win32.SHGetFileInfo( filepath, 0, ref shinfo, (uint) Marshal.SizeOf( shinfo ), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON				);
			if( hSuccess == IntPtr.Zero ) return null;
			bmp = Icon.FromHandle( shinfo.hIcon ).ToBitmap();
			m_bmpCache.Add( ext, bmp );

			return bmp;
		}
#endif
	}
}

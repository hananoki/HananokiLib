
using System.Drawing;


namespace HananokiLib {

	public static class icon {

		static Bitmap m_bmpInfoCache;

		public static Bitmap info {
			get {
				if( m_bmpInfoCache == null ) {
					m_bmpInfoCache = SystemIcons.Information.ToBitmap();
				}
				return m_bmpInfoCache;
			}
		}
	}
}

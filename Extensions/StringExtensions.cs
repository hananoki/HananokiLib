#pragma warning disable 8602
#pragma warning disable 8603
#pragma warning disable 8625
#pragma warning disable 8767

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;


namespace HananokiLib {

	public static class IntExtensions {

		public static bool Has( this int i, int chk ) {
			return 0 != ( i & chk ) ? true : false;
		}
		public static void Enable( ref this int i, int chk ) {
			i |= chk;
		}

		public static void Disable( ref this int i, int chk ) {
			i &= ~chk;
		}
		public static void Toggle( ref this int i, int flag, bool b ) {
			if( b ) i |= flag;
			else i &= ~flag;
		}

		sealed class CommonSelector<T, TKey> : IEqualityComparer<T> {
			Func<T, TKey> m_selector;

			public CommonSelector( Func<T, TKey> selector ) {
				m_selector = selector;
			}

			public bool Equals( T x, T y ) {
				return m_selector( x ).Equals( m_selector( y ) );
			}

			public int GetHashCode( T obj ) {
				return m_selector( obj ).GetHashCode();
			}
		}
		public static IEnumerable<T> Distinct<T, TKey>( this IEnumerable<T> source, Func<T, TKey> selector ) {
			return source.Distinct( new CommonSelector<T, TKey>( selector ) );
		}

		public static bool IsEmpty<T>( this T[] s ) {
			if( s == null ) return true;
			if( s.Length == 0 ) return true;
			return false;
		}

		public static bool IsEmpty<T>( this List<T> s ) {
			if( s == null ) return true;
			if( s.Count == 0 ) return true;
			return false;
		}
	}


	public static class StringExtensions {
		public static string GetValue( this Dictionary<string, string> dic, string key ) {
			return dic.ContainsKey( key ) ? dic[ key ] : "";
		}

		public static string Format( this string fm, params object[] values ) {
			return string.Format( fm, values );
		}

		public static string Quote( this string s ) {
			return '"' + s + '"';
		}

		public static string SeparatorToOS( this string s ) {
			if( s.IsEmpty() ) return string.Empty;
			return s.Replace( '/', Path.DirectorySeparatorChar );
		}

		public static bool IsEmpty( this string s ) => string.IsNullOrEmpty( s );

		public static bool IsExistsFile( this string s ) => File.Exists( s );

		public static bool IsExistsDirectory( this string s ) => Directory.Exists( s );


		public static int ToInt32( this string s ) {
			return Convert.ToInt32( s );
		}

		public static string GetExtension( this string s ) {
			if( s.IsEmpty() ) return string.Empty;
			return Path.GetExtension( s );
		}

		public static string GetDirectoryName( this string s ) {
			if( s.IsEmpty() ) return string.Empty;
			return Path.GetDirectoryName( s );
		}

		public static string GetFileName( this string s ) {
			if( s.IsEmpty() ) return string.Empty;
			return Path.GetFileName( s );
		}

		public static string ChangeExtension( this string s, string ext ) {
			if( s.IsEmpty() ) return string.Empty;
			return Path.ChangeExtension( s, ext );
		}

		public static string GetBaseName( this string s ) {
			if( s.IsEmpty() ) return string.Empty;
			return Path.GetFileNameWithoutExtension( s );
		}

		/// <summary>
		/// ファイル パスにルートが含まれているかどうかを示す値を返します
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsPathRooted( this string s ) {
			if( s.IsEmpty() ) return false;
			return Path.IsPathRooted( s );
		}

		public static string replace( this string s1, string s2, string s3 ) {
			return Regex.Replace( s1, s2, s3, RegexOptions.Singleline );
		}

		public static bool match( this string s1, string s2, Action<GroupCollection> func = null ) {
			if( s1.IsEmpty() ) return false;

			var mm = Regex.Matches( s1, s2 );
			if( 0 < mm.Count ) {
				func?.Invoke( mm[ 0 ].Groups );
				return true;
			}
			return false;
		}

		public static string match2( this string s1, string s2, Func<GroupCollection, string> func1 ) {
			var mm = Regex.Matches( s1, s2 );
			if( 0 < mm.Count ) {
				return func1( mm[ 0 ].Groups );
			}
			return null;
		}
		public static string match2( this string s1, string s2, Func<GroupCollection, string> func1, Func<string> func2 ) {
			var mm = Regex.Matches( s1, s2 );
			if( 0 < mm.Count ) {
				return func1( mm[ 0 ].Groups );
			}
			return func2();
		}


		const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;


		public static string ChangeShortPath( this string s ) {
			if( s.IsEmpty() ) return string.Empty;

			var sb = new StringBuilder( 1024 );
			uint ret = Win32.GetShortPathName( s, sb, (uint) sb.Capacity );
			if( ret == 0 ) {
				int errCode = Marshal.GetLastWin32Error();
				if( errCode != 0 ) {
					Console.WriteLine( $"Win32エラー・コード：{errCode:X8}" );

					StringBuilder message = new StringBuilder( 255 );

					Win32.FormatMessage(
						FORMAT_MESSAGE_FROM_SYSTEM,
						IntPtr.Zero,
						(uint) errCode,
						0,
						message,
						message.Capacity,
						IntPtr.Zero );

					Console.WriteLine( $"Win32エラー・メッセージ：{message.ToString()}" );

					throw new Exception( "短いファイル名の取得に失敗しました。" );
				}
			}
			return sb.ToString();
		}
	}
}

﻿#pragma warning disable 8602
#pragma warning disable 8618

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace HananokiLib {
	// Base64形式の汎用操作を提供するクラス
	public static class Base64 {
		// 指定した通常の文字列をUTF-8としてBase64文字列に変換する
		public static string Encode( string str ) {
			return Encode( str, Encoding.UTF8 );
		}
		// 上記のエンコードが指定できるバージョン
		public static string Encode( string str, Encoding encode ) {
			if( string.IsNullOrEmpty( str ) ) return "";
			return Convert.ToBase64String( encode.GetBytes( str ) );
		}

		// 指定したBase64文字列をUTF-8として通常の文字列に変換する
		public static string Decode( string base64Str ) {
			return Decode( base64Str, Encoding.UTF8 );
		}
		// 上記のエンコードが指定できるバージョン
		public static string Decode( string base64Str, Encoding encode ) {
			if( string.IsNullOrEmpty( base64Str ) ) return "";
			return encode.GetString( Convert.FromBase64String( base64Str ) );
		}
	}

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

		public static string version {
			get {
				var assembly = Assembly.GetExecutingAssembly().GetName();
				var ver = assembly.Version;
				return $"{ver.Major}.{ver.Minor}.{ver.Build}";
			}
		}


		public static void _init() {
			var location = Assembly.GetExecutingAssembly().Location;
			s_appName = location.GetBaseName();

			var exePath = Directory.GetParent( location );
			s_appPath = exePath.FullName;

			var info = new Win32.SystemInfo();
			Win32.GetSystemInfo( out info );

			if( 1 < info.dwNumberOfProcessors ) {
				s_parallelOptions.MaxDegreeOfParallelism = (int) info.dwNumberOfProcessors - 1;
			}

			var dummy = LogWindow.instance; // 初回アクセスによる生成が別スレッドで行われるのを防ぐ
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
				if( obj == null ) {
					obj = new T();
				}
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
			catch( Exception e ) {
				Debug.Exception(e);
				return false;
			}
			return true;
		}


		/// 引用 https://dobon.net/vb/dotnet/system/isadmin.html
		public static bool IsAdministrator() {
			//現在のユーザーを表すWindowsIdentityオブジェクトを取得する
			System.Security.Principal.WindowsIdentity wi =
					System.Security.Principal.WindowsIdentity.GetCurrent();
			//WindowsPrincipalオブジェクトを作成する
			System.Security.Principal.WindowsPrincipal wp =
					new System.Security.Principal.WindowsPrincipal( wi );
			//Administratorsグループに属しているか調べる
			return wp.IsInRole(
					System.Security.Principal.WindowsBuiltInRole.Administrator );
		}
	}
}


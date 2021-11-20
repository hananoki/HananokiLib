using System;


namespace HananokiLib {

	public static class Debug {

		[System.Diagnostics.Conditional( "DEBUG" )]
		public static void AllocConsole() {
			Win32.AllocConsole();
		}


		[System.Diagnostics.Conditional( "DEBUG" )]
		public static void Error( Exception e ) {
			Console.WriteLine( e.ToString() );
		}



		[System.Diagnostics.Conditional( "DEBUG" )]
		public static void Log<T>( T m ) where T : IConvertible {
#if TRACE
			//if( string.IsNullOrEmpty( m ) ) return;
			Console.WriteLine( m.ToString() ) ;
			HananokiLib.Log.Info( m.ToString() );
#endif
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		public static void Log( string m, params object[] args ) {
#if TRACE
			if( string.IsNullOrEmpty( m ) ) return;
			Console.WriteLine( string.Format( m, args ) );
			HananokiLib.Log.Info( string.Format( m, args ) );
#endif
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		public static void Warning( string m, params object[] args ) {
#if TRACE
			if( string.IsNullOrEmpty( m ) ) return;
			Console.WriteLine( string.Format( m, args ) );
			HananokiLib.Log.Warning( string.Format( m, args ) );
#endif
		}

		[System.Diagnostics.Conditional( "DEBUG" )]
		public static void Exception( Exception args ) {
#if TRACE
			//if( string.IsNullOrEmpty( m ) ) return;
			Console.WriteLine(  args  );
			HananokiLib.Log.Exception( args );
#endif
		}
	}
}

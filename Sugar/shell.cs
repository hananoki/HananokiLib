#pragma warning disable 8618

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataReceivedEventArgs = System.Diagnostics.DataReceivedEventArgs;


namespace HananokiLib {

	public class CommandOutput {
		public int exitCode;
		public string error { get; set; }
		public string stdout { get; set; }

		public CommandOutput() {
			error = "";
			stdout = "";
		}
	}

	public static class shell {

		/// <summary>
		/// このプロセスのみ有効な環境変数を設定します
		/// </summary>
		/// <param name="path"></param>
		public static void SetProcessEnvironmentPath( string path ) {
			Environment.SetEnvironmentVariable( "PATH", path, EnvironmentVariableTarget.Process );

			Debug.Log( $"SetEnvironmentVariable > {path}" );
		}

		public static void start( string fileName ) {
			System.Diagnostics.Process.Start( fileName );
		}

		public static void start( string fileName, string arguments ) {
			System.Diagnostics.Process.Start( fileName, arguments );
		}


		async public static Task<CommandOutput> startProcessAsync( string filename, string arguments ) {
			var task = Task<int>.Run( () => {
				//Thread.Sleep(5000);
				var result = startProcess( filename, arguments );

				return result;
			} );
			await task;

			return task.Result;
		}


		public static CommandOutput startProcess( string filename, string arguments, string workingDirectory = "" ) {

			Log.Info( $"{filename} {arguments}" );

			//*
			//using(  ) {
			var p = new System.Diagnostics.Process();
			p.StartInfo.FileName = filename;
			p.StartInfo.Arguments = arguments;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			if( !workingDirectory.IsEmpty() ) {
				p.StartInfo.WorkingDirectory = workingDirectory;
			}
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardInput = false;
			//p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding( "Shift_JIS" );
			//p.StartInfo.StandardErrorEncoding = Encoding.GetEncoding( "Shift_JIS" );
			p.EnableRaisingEvents = true;

			ConcurrentQueue<string> messages = new ConcurrentQueue<string>();

			p.ErrorDataReceived += ErrorDataHandler;
			p.OutputDataReceived += OutputDataHandler;

			var output = new CommandOutput();
			Outputs.Add( p, output );

			p.Start();
			//
			p.BeginErrorReadLine();
			p.BeginOutputReadLine();

			p.WaitForExit();

			Outputs.Remove( p );

			//if( ( !String.IsNullOrWhiteSpace( output.Error ) ) ) {
			//	//return output.Error.TrimEnd( '\n' );
			//	Log.Error( output.Error.TrimEnd( '\n' ) );
			//}
			//Debug.Log(  );

			if( p.ExitCode != 0 ) {
				Log.Error( $"[StandardOutput] {output.stdout.TrimEnd( '\n' )}" );
				Log.Error( $"[StandardError] {output.error.TrimEnd( '\n' )}" );
			}

			output.exitCode = p.ExitCode;
			return output;
		}


		static void ErrorDataHandler( object sendingProcess, DataReceivedEventArgs errLine ) {
			if( errLine.Data == null )
				return;

			if( !Outputs.ContainsKey( sendingProcess ) )
				return;

			var commandOutput = Outputs[ sendingProcess ];

			commandOutput.error = commandOutput.error + errLine.Data + "\n";
		}


		static void OutputDataHandler( object sendingProcess, DataReceivedEventArgs outputLine ) {
			if( outputLine.Data == null )
				return;

			if( !Outputs.ContainsKey( sendingProcess ) )
				return;

			var commandOutput = Outputs[ sendingProcess ];

			commandOutput.stdout = commandOutput.stdout + outputLine.Data + "\n";
		}

		#region Outputs Property

		private static object _outputsLockObject;
		private static object OutputsLockObject {
			get {
				if( _outputsLockObject == null )
					Interlocked.CompareExchange( ref _outputsLockObject, new object(), null );
				return _outputsLockObject;
			}
		}

		private static Dictionary<object, CommandOutput> _outputs;
		private static Dictionary<object, CommandOutput> Outputs {
			get {
				if( _outputs != null )
					return _outputs;

				lock( OutputsLockObject ) {
					_outputs = new Dictionary<object, CommandOutput>();
				}
				return _outputs;
			}
		}

		#endregion


	} // class
}

using System;
using System.Runtime.InteropServices;


public static class Win32 {
#if false
	#region dwmapi.dll

	[StructLayout( LayoutKind.Sequential )]
	public struct MARGINS {
		public int leftWidth;
		public int rightWidth;
		public int topHeight;
		public int bottomHeight;
	}

	[DllImport( "dwmapi.dll", PreserveSig = true )]
	public static extern int DwmExtendFrameIntoClientArea( IntPtr hwnd, ref MARGINS margins );

	[DllImport( "dwmapi.dll", PreserveSig = false )]
	public static extern bool DwmIsCompositionEnabled();

	[DllImport( "dwmapi.dll" )]
	public static extern int DwmIsCompositionEnabled( out bool enabled );

	#endregion
#endif


	#region winmm.dll

	//サウンドを再生するWin32 APIの宣言
	[Flags]
	public enum PlaySoundFlags : int {
		SND_SYNC = 0x0000,
		SND_ASYNC = 0x0001,
		SND_NODEFAULT = 0x0002,
		SND_MEMORY = 0x0004,
		SND_LOOP = 0x0008,
		SND_NOSTOP = 0x0010,
		SND_NOWAIT = 0x00002000,
		SND_ALIAS = 0x00010000,
		SND_ALIAS_ID = 0x00110000,
		SND_FILENAME = 0x00020000,
		SND_RESOURCE = 0x00040004,
		SND_PURGE = 0x0040,
		SND_APPLICATION = 0x0080
	}

	[DllImport( "winmm.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto )]
	public static extern bool PlaySound( string pszSound, IntPtr hmod, PlaySoundFlags fdwSound );

	[DllImport( "winmm.dll" )]
	public static extern bool PlaySound( byte[] pszSound, IntPtr hmod, PlaySoundFlags fdwSound );

	#endregion


	[StructLayout( LayoutKind.Sequential )]
	public struct SystemInfo {
		public uint dwOemId;
		public uint dwPageSize;
		public IntPtr lpMinimumApplicationAddress;
		public IntPtr lpMaximumApplicationAddress;
		public uint dwActiveProcessorMask;
		public uint dwNumberOfProcessors;
		public uint dwProcessorType;
		public uint dwAllocationGranularity;
		public uint dwProcessorLevel;
		public uint dwProcessorRevision;
	}



	[DllImport( "kernel32" )]
	public static extern bool AllocConsole();

	[DllImport( "kernel32.dll", ExactSpelling = true )]
	public static extern void GetSystemInfo( out SystemInfo ptmpsi );


#if ENABLE_SHELL32_DLL
	// SHGetFileInfo関数
	[DllImport( "shell32.dll" )]
	public static extern IntPtr SHGetFileInfo( string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags );
	// SHGetFileInfo関数で使用するフラグ
	public const uint SHGFI_ICON = 0x100; // アイコン・リソースの取得
	public const uint SHGFI_LARGEICON = 0x0; // 大きいアイコン
	public const uint SHGFI_SMALLICON = 0x1; // 小さいアイコン
																					 // SHGetFileInfo関数で使用する構造体
	public struct SHFILEINFO {
		public IntPtr hIcon;
		public IntPtr iIcon;
		public uint dwAttributes;
		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
		public string szDisplayName;
		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 80 )]
		public string szTypeName;
	};
#endif


	#region Icon

	//[DllImport( "Shell32.dll", CharSet = CharSet.Unicode )]
	//public static extern uint ExtractIconEx( string lpszFile, int nIconIndex,
	//	IntPtr[] phiconLarge, IntPtr phiconSmall, uint nIcons );

	//[DllImport( "Shell32.dll", CharSet = CharSet.Unicode )]
	//public static extern uint ExtractIconEx( string lpszFile, int nIconIndex,
	//		IntPtr phiconLarge, IntPtr phiconSmall, uint nIcons );

	// ExtractIconEx 複数の引数指定方法により、オーバーロード定義する。
	//[DllImport( "Shell32.dll", CharSet = CharSet.Unicode )]
	//public static extern uint ExtractIconEx( string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons );
	//[DllImport( "Shell32.dll", CharSet = CharSet.Unicode )]
	//public static extern uint ExtractIconEx( string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr phiconSmall, uint nIcons );
	//[DllImport( "Shell32.dll", CharSet = CharSet.Unicode )]
	//public static extern uint ExtractIconEx( string lpszFile, int nIconIndex, IntPtr phiconLarge, IntPtr[] phiconSmall, uint nIcons );
	//[DllImport( "Shell32.dll", CharSet = CharSet.Unicode )]
	//public static extern uint ExtractIconEx( string lpszFile, int nIconIndex, out IntPtr phiconLarge, out IntPtr phiconSmall, uint nIcons );

	//[DllImport( "User32.dll" )]
	//public static extern bool DestroyIcon( IntPtr hIcon );


	#endregion



	#region ゴミ箱削除

	/**
	https://www.umayadia.com/cssample/sample0201/Sample264FolderDelete.htmから引用
	*/

	/// <summary>
	/// ファイルをコピー・移動・削除・名前変更します。
	/// </summary>
	/// <param name="lpFileOp"></param>
	/// <returns>正常時0。異常時の値の意味は https://docs.microsoft.com/ja-jp/windows/win32/api/shellapi/nf-shellapi-shfileoperationa を参照。</returns>
	[DllImport( "shell32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
	public static extern int SHFileOperation( [In] ref SHFILEOPSTRUCT lpFileOp );

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct SHFILEOPSTRUCT {
		public IntPtr hwnd;
		[MarshalAs( UnmanagedType.U4 )] public FileFuncFlags wFunc;
		[MarshalAs( UnmanagedType.LPWStr )] public string pFrom;
		[MarshalAs( UnmanagedType.LPWStr )] public string pTo;
		[MarshalAs( UnmanagedType.U2 )] public FILEOP_FLAGS fFlags;
		[MarshalAs( UnmanagedType.Bool )] public bool ffAnyOperationsAborted;
		public IntPtr hNameMappings; //FOF_WANTMAPPINGHANDLEフラグとともに使用します。
		[MarshalAs( UnmanagedType.LPWStr )] public string lplpszProgressTitle; //FOF_SIMPLEPROGRESSフラグとともに使用します。
	}

	public enum FileFuncFlags {
		/// <summary>pFrom から pTo にファイルを移動します。</summary>
		FO_MOVE = 0x1,
		/// <summary>pFrom から pTo にファイルをコピーします。</summary>
		FO_COPY = 0x2,
		/// <summary>pFrom からファイルを削除します。</summary>
		FO_DELETE = 0x3,
		/// <summary>pFrom のファイルの名前を変更します。複数ファイルを対象とする場合は FO_MOVE を使用します。</summary>
		FO_RENAME = 0x4
	}

	[Flags]
	public enum FILEOP_FLAGS : short {
		/// <summary>pToにはpFromに１対１で対応する複数のコピー先を指定します。</summary>
		FOF_MULTIDESTFILES = 0x1,
		/// <summary>このフラグは使用しません。</summary>
		FOF_CONFIRMMOUSE = 0x2,
		/// <summary>進捗状況のダイアログを表示しません。</summary>
		FOF_SILENT = 0x4,
		/// <summary>同名のファイルが既に存在する場合、新しい名前を付けます。</summary>
		FOF_RENAMEONCOLLISION = 0x8,
		/// <summary>確認ダイアログを表示せず、すべて「はい」を選択したものとします。</summary>
		FOF_NOCONFIRMATION = 0x10,
		/// <summary>FOF_RENAMEONCOLLISIONフラグによるファイル名の衝突回避が発生した場合、SHFILEOPSTRUCT.hNameMappingsに新旧ファイル名の情報を格納します。この情報はSHFreeNameMappingsを使って開放する必要があります。</summary>
		FOF_WANTMAPPINGHANDLE = 0x20,
		/// <summary>可能であれば、操作を元に戻せるようにします。</summary>
		FOF_ALLOWUNDO = 0x40,
		/// <summary>ワイルドカードが使用された場合、ファイルのみを対象とします。</summary>
		FOF_FILESONLY = 0x80,
		/// <summary>進捗状況のダイアログを表示しますが、個々のファイル名は表示しません。</summary>
		FOF_SIMPLEPROGRESS = 0x100,
		/// <summary>新しいフォルダーの作成する前にユーザーに確認しません。</summary>
		FOF_NOCONFIRMMKDIR = 0x200,
		/// <summary>エラーが発生してもダイアログを表示しません。</summary>
		FOF_NOERRORUI = 0x400,
		/// <summary>ファイルのセキュリティ属性はコピーしません。コピー後のファイルはコピー先のフォルダーのセキュリティ属性を引き継ぎます。</summary>
		FOF_NOCOPYSECURITYATTRIBS = 0x800,
		/// <summary>サブディレクトリーを再帰的に処理しません。これは既定の動作です。</summary>
		FOF_NORECURSION = 0x1000,
		/// <summary>グループとして連結しているファイルは移動しません。指定されたファイルだけを移動します。</summary>
		FOF_NO_CONNECTED_ELEMENTS = 0x2000,
		/// <summary>ファイルが恒久的に削除される場合、警告を表示します。このフラグはFOF_NOCONFIRMATIONより優先されます。 </summary>
		FOF_WANTNUKEWARNING = 0x4000,
		/// <summary>UIを表示しません。</summary>
		FOF_NO_UI = FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR
	}

	#endregion

#if false
	

	

	[DllImport( "kernel32.dll" )]
	public static extern uint FormatMessage(
		uint dwFlags, IntPtr lpSource,
		uint dwMessageId, uint dwLanguageId,
		StringBuilder lpBuffer, int nSize,
		IntPtr Arguments );





	[DllImport( "kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto )]
	public static extern uint GetShortPathName( [MarshalAs( UnmanagedType.LPTStr )] string lpszLongPath, [MarshalAs( UnmanagedType.LPTStr )] StringBuilder lpszShortPath, uint cchBuffer );



	
#endif
}



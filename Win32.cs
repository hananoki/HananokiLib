using System;
using System.Runtime.InteropServices;
using System.Text;

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


	[DllImport( "kernel32.dll" )]
	public static extern uint FormatMessage(
		uint dwFlags, IntPtr lpSource,
		uint dwMessageId, uint dwLanguageId,
		StringBuilder lpBuffer, int nSize,
		IntPtr Arguments );

	[DllImport( "kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto )]
	public static extern uint GetShortPathName( [MarshalAs( UnmanagedType.LPTStr )] string lpszLongPath, [MarshalAs( UnmanagedType.LPTStr )] StringBuilder lpszShortPath, uint cchBuffer );



	//https://www.pinvoke.net/default.aspx/Enums/VK.html 引用
	public enum VK : int {
		///<summary>
		///Left mouse button
		///</summary>
		LeftButton = 0x01,
		///<summary>
		///Right mouse button
		///</summary>
		RightButton = 0x02,
		///<summary>
		///Control-break processing
		///</summary>
		Cancel = 0x03,
		///<summary>
		///Middle mouse button (three-button mouse)
		///</summary>
		Middlebutton = 0x04,
		///<summary>
		///Windows 2000/XP: X1 mouse button
		///</summary>
		XButton1 = 0x05,
		///<summary>
		///Windows 2000/XP: X2 mouse button
		///</summary>
		XButton2 = 0x06,
		///<summary>
		///BACKSPACE key
		///</summary>
		Back = 0x08,
		///<summary>
		///TAB key
		///</summary>
		Tab = 0x09,
		///<summary>
		///CLEAR key
		///</summary>
		Clear = 0x0C,
		///<summary>
		///ENTER key
		///</summary>
		Enter = 0x0D,
		///<summary>
		///SHIFT key
		///</summary>
		Shift = 0x10,
		///<summary>
		///CTRL key
		///</summary>
		Control = 0x11,
		///<summary>
		///ALT key
		///</summary>
		Alt = 0x12,
		///<summary>
		///PAUSE key
		///</summary>
		Pause = 0x13,
		///<summary>
		///CAPS LOCK key
		///</summary>
		CapsLock = 0x14,
		///<summary>
		///Input Method Editor (IME) Kana mode
		///</summary>
		Kana = 0x15,
		///<summary>
		///IME Hangul mode
		///</summary>
		Hangul = 0x15,
		///<summary>
		///IME オン
		///</summary>
		ImeOn = 0x16,
		///<summary>
		///IME Junja mode
		///</summary>
		Junja = 0x17,
		///<summary>
		///IME final mode
		///</summary>
		Final = 0x18,
		///<summary>
		///IME Hanja mode
		///</summary>
		Hanja = 0x19,
		///<summary>
		///IME Kanji mode
		///</summary>
		Kanji = 0x19,
		///<summary>
		/// IME オフ
		///</summary>
		ImeOff = 0x1A,
		///<summary>
		///ESC key
		///</summary>
		Escape = 0x1B,
		///<summary>
		///IME convert
		///</summary>
		Convert = 0x1C,
		///<summary>
		///IME nonconvert
		///</summary>
		NonConvert = 0x1D,
		///<summary>
		///IME accept
		///</summary>
		Accept = 0x1E,
		///<summary>
		///IME mode change request
		///</summary>
		ModeChange = 0x1F,
		///<summary>
		///SPACEBAR
		///</summary>
		Space = 0x20,
		///<summary>
		///PAGE UP key
		///</summary>
		PageUp = 0x21,
		///<summary>
		///PAGE DOWN key
		///</summary>
		PageDown = 0x22,
		///<summary>
		///END key
		///</summary>
		End = 0x23,
		///<summary>
		///HOME key
		///</summary>
		Home = 0x24,
		///<summary>
		///LEFT ARROW key
		///</summary>
		Left = 0x25,
		///<summary>
		///UP ARROW key
		///</summary>
		Up = 0x26,
		///<summary>
		///RIGHT ARROW key
		///</summary>
		Right = 0x27,
		///<summary>
		///DOWN ARROW key
		///</summary>
		Down = 0x28,
		///<summary>
		///SELECT key
		///</summary>
		Select = 0x29,
		///<summary>
		///PRINT key
		///</summary>
		Print = 0x2A,
		///<summary>
		///EXECUTE key
		///</summary>
		Execute = 0x2B,
		///<summary>
		///PRINT SCREEN key
		///</summary>
		PrintScreen = 0x2C,
		///<summary>
		///INS key
		///</summary>
		Insert = 0x2D,
		///<summary>
		///DEL key
		///</summary>
		Delete = 0x2E,
		///<summary>
		///HELP key
		///</summary>
		Help = 0x2F,
		///<summary>
		///0 key
		///</summary>
		Key0 = 0x30,
		///<summary>
		///1 key
		///</summary>
		Key1 = 0x31,
		///<summary>
		///2 key
		///</summary>
		Key2 = 0x32,
		///<summary>
		///3 key
		///</summary>
		Key3 = 0x33,
		///<summary>
		///4 key
		///</summary>
		Key4 = 0x34,
		///<summary>
		///5 key
		///</summary>
		Key5 = 0x35,
		///<summary>
		///6 key
		///</summary>
		Key6 = 0x36,
		///<summary>
		///7 key
		///</summary>
		Key7 = 0x37,
		///<summary>
		///8 key
		///</summary>
		Key8 = 0x38,
		///<summary>
		///9 key
		///</summary>
		Key9 = 0x39,
		///<summary>
		///A key
		///</summary>
		A = 0x41,
		///<summary>
		///B key
		///</summary>
		B = 0x42,
		///<summary>
		///C key
		///</summary>
		C = 0x43,
		///<summary>
		///D key
		///</summary>
		D = 0x44,
		///<summary>
		///E key
		///</summary>
		E = 0x45,
		///<summary>
		///F key
		///</summary>
		F = 0x46,
		///<summary>
		///G key
		///</summary>
		G = 0x47,
		///<summary>
		///H key
		///</summary>
		H = 0x48,
		///<summary>
		///I key
		///</summary>
		I = 0x49,
		///<summary>
		///J key
		///</summary>
		J = 0x4A,
		///<summary>
		///K key
		///</summary>
		K = 0x4B,
		///<summary>
		///L key
		///</summary>
		L = 0x4C,
		///<summary>
		///M key
		///</summary>
		M = 0x4D,
		///<summary>
		///N key
		///</summary>
		N = 0x4E,
		///<summary>
		///O key
		///</summary>
		O = 0x4F,
		///<summary>
		///P key
		///</summary>
		P = 0x50,
		///<summary>
		///Q key
		///</summary>
		Q = 0x51,
		///<summary>
		///R key
		///</summary>
		R = 0x52,
		///<summary>
		///S key
		///</summary>
		S = 0x53,
		///<summary>
		///T key
		///</summary>
		T = 0x54,
		///<summary>
		///U key
		///</summary>
		U = 0x55,
		///<summary>
		///V key
		///</summary>
		V = 0x56,
		///<summary>
		///W key
		///</summary>
		W = 0x57,
		///<summary>
		///X key
		///</summary>
		X = 0x58,
		///<summary>
		///Y key
		///</summary>
		Y = 0x59,
		///<summary>
		///Z key
		///</summary>
		Z = 0x5A,
		///<summary>
		///Left Windows key (Microsoft Natural keyboard)
		///</summary>
		LeftWindows = 0x5B,
		///<summary>
		///Right Windows key (Natural keyboard)
		///</summary>
		RightWindows = 0x5C,
		///<summary>
		///Applications key (Natural keyboard)
		///</summary>
		Application = 0x5D,
		///<summary>
		///Computer Sleep key
		///</summary>
		Sleep = 0x5F,
		///<summary>
		///Numeric keypad 0 key
		///</summary>
		NumPad0 = 0x60,
		///<summary>
		///Numeric keypad 1 key
		///</summary>
		NumPad1 = 0x61,
		///<summary>
		///Numeric keypad 2 key
		///</summary>
		NumPad2 = 0x62,
		///<summary>
		///Numeric keypad 3 key
		///</summary>
		NumPad3 = 0x63,
		///<summary>
		///Numeric keypad 4 key
		///</summary>
		NumPad4 = 0x64,
		///<summary>
		///Numeric keypad 5 key
		///</summary>
		NumPad5 = 0x65,
		///<summary>
		///Numeric keypad 6 key
		///</summary>
		NumPad6 = 0x66,
		///<summary>
		///Numeric keypad 7 key
		///</summary>
		NumPad7 = 0x67,
		///<summary>
		///Numeric keypad 8 key
		///</summary>
		NumPad8 = 0x68,
		///<summary>
		///Numeric keypad 9 key
		///</summary>
		NumPad9 = 0x69,
		///<summary>
		///Multiply key
		///</summary>
		Multiply = 0x6A,
		///<summary>
		///Add key
		///</summary>
		Add = 0x6B,
		///<summary>
		///Separator key
		///</summary>
		Separator = 0x6C,
		///<summary>
		///Subtract key
		///</summary>
		Subtract = 0x6D,
		///<summary>
		///Decimal key
		///</summary>
		Decimal = 0x6E,
		///<summary>
		///Divide key
		///</summary>
		Divide = 0x6F,
		///<summary>
		///F1 key
		///</summary>
		F1 = 0x70,
		///<summary>
		///F2 key
		///</summary>
		F2 = 0x71,
		///<summary>
		///F3 key
		///</summary>
		F3 = 0x72,
		///<summary>
		///F4 key
		///</summary>
		F4 = 0x73,
		///<summary>
		///F5 key
		///</summary>
		F5 = 0x74,
		///<summary>
		///F6 key
		///</summary>
		F6 = 0x75,
		///<summary>
		///F7 key
		///</summary>
		F7 = 0x76,
		///<summary>
		///F8 key
		///</summary>
		F8 = 0x77,
		///<summary>
		///F9 key
		///</summary>
		F9 = 0x78,
		///<summary>
		///F10 key
		///</summary>
		F10 = 0x79,
		///<summary>
		///F11 key
		///</summary>
		F11 = 0x7A,
		///<summary>
		///F12 key
		///</summary>
		F12 = 0x7B,
		///<summary>
		///F13 key
		///</summary>
		F13 = 0x7C,
		///<summary>
		///F14 key
		///</summary>
		F14 = 0x7D,
		///<summary>
		///F15 key
		///</summary>
		F15 = 0x7E,
		///<summary>
		///F16 key
		///</summary>
		F16 = 0x7F,
		///<summary>
		///F17 key  
		///</summary>
		F17 = 0x80,
		///<summary>
		///F18 key  
		///</summary>
		F18 = 0x81,
		///<summary>
		///F19 key  
		///</summary>
		F19 = 0x82,
		///<summary>
		///F20 key  
		///</summary>
		F20 = 0x83,
		///<summary>
		///F21 key  
		///</summary>
		F21 = 0x84,
		///<summary>
		///F22 key, (PPC only) Key used to lock device.
		///</summary>
		F22 = 0x85,
		///<summary>
		///F23 key  
		///</summary>
		F23 = 0x86,
		///<summary>
		///F24 key  
		///</summary>
		F24 = 0x87,
		///<summary>
		///NUM LOCK key
		///</summary>
		NumLock = 0x90,
		///<summary>
		///SCROLL LOCK key
		///</summary>
		Scroll = 0x91,
		///<summary>
		///Left SHIFT key
		///</summary>
		LeftSHIFT = 0xA0,
		///<summary>
		///Right SHIFT key
		///</summary>
		RightShift = 0xA1,
		///<summary>
		///Left CONTROL key
		///</summary>
		LeftControl = 0xA2,
		///<summary>
		///Right CONTROL key
		///</summary>
		RightControl = 0xA3,
		///<summary>
		///Left MENU key
		///</summary>
		LeftAlt = 0xA4,
		///<summary>
		///Right MENU key
		///</summary>
		RightAlt = 0xA5,
		///<summary>
		///Windows 2000/XP: Browser Back key
		///</summary>
		BrowserBack = 0xA6,
		///<summary>
		///Windows 2000/XP: Browser Forward key
		///</summary>
		BrowserForward = 0xA7,
		///<summary>
		///Windows 2000/XP: Browser Refresh key
		///</summary>
		BrowserRefresh = 0xA8,
		///<summary>
		///Windows 2000/XP: Browser Stop key
		///</summary>
		BrowserStop = 0xA9,
		///<summary>
		///Windows 2000/XP: Browser Search key
		///</summary>
		BrowserSearch = 0xAA,
		///<summary>
		///Windows 2000/XP: Browser Favorites key
		///</summary>
		BrowserFavorites = 0xAB,
		///<summary>
		///Windows 2000/XP: Browser Start and Home key
		///</summary>
		BrowserHome = 0xAC,
		///<summary>
		///Windows 2000/XP: Volume Mute key
		///</summary>
		VolumeMute = 0xAD,
		///<summary>
		///Windows 2000/XP: Volume Down key
		///</summary>
		VolumeDown = 0xAE,
		///<summary>
		///Windows 2000/XP: Volume Up key
		///</summary>
		VolumeUp = 0xAF,
		///<summary>
		///Windows 2000/XP: Next Track key
		///</summary>
		MediaNextTrack = 0xB0,
		///<summary>
		///Windows 2000/XP: Previous Track key
		///</summary>
		MediaPrevTrack = 0xB1,
		///<summary>
		///Windows 2000/XP: Stop Media key
		///</summary>
		MediaStop = 0xB2,
		///<summary>
		///Windows 2000/XP: Play/Pause Media key
		///</summary>
		MediaPlayPause = 0xB3,
		///<summary>
		///Windows 2000/XP: Start Mail key
		///</summary>
		LaunchMail = 0xB4,
		///<summary>
		///Windows 2000/XP: Select Media key
		///</summary>
		LaunchMediaSelect = 0xB5,
		///<summary>
		///Windows 2000/XP: Start Application 1 key
		///</summary>
		LaunchApp1 = 0xB6,
		///<summary>
		///Windows 2000/XP: Start Application 2 key
		///</summary>
		LaunchApp2 = 0xB7,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_1 = 0xBA,
		///<summary>
		///Windows 2000/XP: For any country/region, the '+' key
		///</summary>
		OEM_Plus = 0xBB,
		///<summary>
		///Windows 2000/XP: For any country/region, the ',' key
		///</summary>
		OEM_Comma = 0xBC,
		///<summary>
		///Windows 2000/XP: For any country/region, the '-' key
		///</summary>
		OEM_Minus = 0xBD,
		///<summary>
		///Windows 2000/XP: For any country/region, the '.' key
		///</summary>
		OEM_Period = 0xBE,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_2 = 0xBF,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_3 = 0xC0,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_4 = 0xDB,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_5 = 0xDC,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_6 = 0xDD,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_7 = 0xDE,
		///<summary>
		///Used for miscellaneous characters; it can vary by keyboard.
		///</summary>
		OEM_8 = 0xDF,
		///<summary>
		///Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
		///</summary>
		OEM_102 = 0xE2,
		///<summary>
		///Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
		///</summary>
		Processkey = 0xE5,
		///<summary>
		///Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
		///</summary>
		Packet = 0xE7,
		///<summary>
		///Attn key
		///</summary>
		Attn = 0xF6,
		///<summary>
		///CrSel key
		///</summary>
		CrSel = 0xF7,
		///<summary>
		///ExSel key
		///</summary>
		ExSel = 0xF8,
		///<summary>
		///Erase EOF key
		///</summary>
		ErEOF = 0xF9,
		///<summary>
		///Play key
		///</summary>
		Play = 0xFA,
		///<summary>
		///Zoom key
		///</summary>
		Zoom = 0xFB,
		///<summary>
		///Reserved
		///</summary>
		NoName = 0xFC,
		///<summary>
		///PA1 key
		///</summary>
		PA1 = 0xFD,
		///<summary>
		///Clear key
		///</summary>
		OEM_Clear = 0xFE
	}
}



﻿#pragma warning disable 8604

using System.Text;


namespace HananokiLib {
	public static class JsonUtils {
		public static string ToJson<T>( T obj, bool newline = true ) {
			var builder = new StringBuilder();
			var writer = new LitJson.JsonWriter( builder ) {
				PrettyPrint = newline
			};
			LitJson.JsonMapper.ToJson( obj, writer );
			return builder.ToString();
		}
	}
}

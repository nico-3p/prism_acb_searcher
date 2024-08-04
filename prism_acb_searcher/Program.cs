using System.Text;

// LocalLowPath は、ユーザーの「AppData\LocalLow」フォルダーのパスを取得します。
// ApplicationData フォルダーのパスを取得し、「Roaming」を「LocalLow」に置き換えます。
string LocalLowPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow");

// フルパスを作成します。ここでは、BNE\\imasscprism\\D\\ のパスを指定します。
string FullPath = Path.Combine(LocalLowPath, "BNE\\imasscprism\\D\\");

// フルパス内のすべてのファイルを取得します。サブディレクトリも含めて検索します。
string[] files = Directory.GetFiles(FullPath, "*", SearchOption.AllDirectories);

// 出力ディレクトリ「acb/」が存在しない場合は、新しく作成します。
var out_directory = "acb/";
if (!Directory.Exists(out_directory))
	Directory.CreateDirectory(out_directory);

// 各ファイルを処理します。
foreach (string path in files)
{
	// ファイルのバイト配列を読み込みます。
	byte[] source = System.IO.File.ReadAllBytes(path);

	// 先頭 4 バイトをチェックするためのターゲットバイト配列を定義します。
	byte[] target = [0x40, 0x55, 0x54, 0x46];

	// ファイルの先頭 4 バイトがターゲットバイト配列と一致しない場合、次のファイルに進みます。
	if (!source[..4].SequenceEqual(target))
		continue;

	// ファイルの 2179 バイト目から 32 バイトを抽出し、ゼロバイトで終わる名前を取得します。
	var name_b = source[2179..(2179 + 32)];
	name_b = name_b[..Array.IndexOf(name_b, (byte)0)];

	// UTF-8 エンコーディングを使用して、バイト配列からファイル名を取得します。
	var encoding = Encoding.GetEncoding("UTF-8");
	var filename = encoding.GetString(name_b);

	try
	{
		// ファイルを「acb/」ディレクトリにコピーし、拡張子に「.acb」を追加します。
		File.Copy(path, Path.Combine(out_directory, filename + ".acb"));
	}
	catch (IOException copyError)
	{
		// ファイルコピー中にエラーが発生した場合、エラーメッセージを表示します。
		Console.WriteLine(copyError.Message);
	}

	// コピーしたファイルの名前をコンソールに表示します。
	Console.WriteLine(filename);
}

using System;
using System.Collections.Generic;
using System.IO;

public static class EnvLoader
{
    private static Dictionary<string, string> envVariables = new Dictionary<string, string>();

    // 環境変数の読み込み
    public static void LoadEnvFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("環境変数ファイルが見つかりません: " + path);
        }

        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                envVariables[parts[0]] = parts[1];
            }
        }
    }

    // 環境変数の取得
    public static string GetEnv(string key, string defaultValue = "")
    {
        return envVariables.ContainsKey(key) ? envVariables[key] : defaultValue;
    }
}

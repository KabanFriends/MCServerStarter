using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServerStarter
{
    class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

        static string VERSION = "1.1";

        static void Main(string[] args)
        {
            Console.WriteLine(": MCServerStarter - v" + VERSION + " by KabanFriends");
            Console.WriteLine(": Minecraftサーバーの起動補助ソフト");
            Console.WriteLine("");

            string pfPath = "";
            if (Is64BitOS())
            {
                pfPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }else
            {
                pfPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }

            List<string> javaVersions = new List<string>();
            List<string> javaPaths = new List<string>();
            string[] paths = Directory.GetDirectories(pfPath + "\\Java");

            for (int i = 0; i < paths.Length; i ++)
            {
                string path = paths[paths.Length - (i + 1)];
                
                if (Regex.IsMatch(path, "...1\\.8\\.0_")) {
                    if (!javaVersions.Contains("8"))
                    {
                        javaVersions.Add("8");
                        javaPaths.Add(path);
                    }
                }else if (Regex.IsMatch(path, "...11\\..*"))
                {
                    if (!javaVersions.Contains("11"))
                    {
                        javaVersions.Add("11");
                        javaPaths.Add(path);
                    }
                }
                else if (Regex.IsMatch(path, "...16\\..*"))
                {
                    if (!javaVersions.Contains("16"))
                    {
                        javaVersions.Add("16");
                        javaPaths.Add(path);
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("1) 起動するサーバーのバージョンは 1.16以下 ですか、それとも 1.17以上 ですか?");
            Console.WriteLine("1.16以下 の場合は「1」、1.17以上 の場合は「2」と記入してください:");

            int type = 0;
            while (true)
            {
                Console.Write("> ");
                string option = Console.ReadLine();
                bool finish = false;

                switch (option)
                {
                    case "1":
                        Console.WriteLine("サーバーのバージョンは 1.16以下 です。");
                        finish = true;
                        type = 1;
                        break;
                    case "2":
                        Console.WriteLine("サーバーのバージョンは 1.17以上 です。");
                        finish = true;
                        type = 2;
                        break;
                    default:
                        Console.WriteLine("「1」または「2」の数字を記入してください。");
                        break;
                }

                if (finish) break;
            }


            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("2) 起動するサーバーの本体のファイル名を記入してください。");
            Console.WriteLine("(分からない場合: サーバーのフォルダー内にある「.jar」とついたファイルの名前をここに記入してください)");

            string serverFile = "";
            while (true)
            {
                Console.Write("> ");
                serverFile = Console.ReadLine();

                if (!serverFile.EndsWith(".jar")) serverFile += ".jar";

                if (File.Exists(serverFile))
                {
                    break;
                }else
                {
                    Console.WriteLine("その名前のファイルは見つかりませんでした。");
                    Console.WriteLine("記入した内容を再度確認してください。");
                }
            }

            Console.WriteLine("起動ファイルは " + serverFile + " です。");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("3) 起動するサーバーは プラグイン有り ですか、それとも プラグイン無し ですか?");
            Console.WriteLine("プラグイン有り の場合は「1」、プラグイン無し の場合は「2」と記入してください。");
            Console.WriteLine("(分からない場合: 手順(2)のファイル名に「spigot」や「bukkit」と書いてあれば プラグイン有り の可能性が高いです)");

            bool plugin = false;
            while (true)
            {
                Console.Write("> ");
                bool finish = false;
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Console.WriteLine("サーバーは プラグイン有り です。");
                        finish = true;
                        plugin = true;
                        break;
                    case "2":
                        Console.WriteLine("サーバーは プラグイン無し です。");
                        finish = true;
                        plugin = false;
                        break;
                    default:
                        Console.WriteLine("「1」または「2」の数字を記入してください。");
                        break;
                }

                if (finish) break;
            }

            string finalJava = "";
            string runPath = "";
            if (type == 1)
            {
                if (javaVersions.Contains("11"))
                {
                    finalJava = "11";
                    int index = javaVersions.IndexOf("11");
                    string path = javaPaths[index];
                    if (plugin) runPath = path + "\\bin\\java.exe";
                    else runPath = path + "\\bin\\javaw.exe";
                }
                else if (javaVersions.Contains("8"))
                {
                    finalJava = "8";
                    int index = javaVersions.IndexOf("8");
                    string path = javaPaths[index];
                    if (plugin) runPath = path + "\\bin\\java.exe";
                    else runPath = path + "\\bin\\javaw.exe";
                }
                else
                {
                    Console.WriteLine("利用可能なJavaのバージョンが見つかりませんでした。");
                    Console.WriteLine("以下のサイトからJavaのインストールを行ってください。");
                    Console.WriteLine("→ https://java.com/ja/download/manual.jsp");
                }
            }else
            {
                if (javaVersions.Contains("16"))
                {
                    finalJava = "16";
                    int index = javaVersions.IndexOf("16");
                    string path = javaPaths[index];
                    if (plugin) runPath = path + "\\bin\\java.exe";
                    else runPath = path + "\\bin\\javaw.exe";
                }
                else
                {
                    Console.WriteLine("Java 16がインストールされていないため、1.17以上のサーバーを起動できません。");
                    Console.WriteLine("以下のサイトから「Windows x64 Installer」のJavaインストーラーをダウンロードして、Java 16のインストールを行ってください。");
                    Console.WriteLine("→ https://www.oracle.com/java/technologies/javase-jdk16-downloads.html");
                }
            }

            if (finalJava != "")
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("サーバーの起動用スクリプトを作成します…");
                Console.WriteLine("========== サーバー情報 ==========");
                Console.WriteLine("起動ファイル: " + serverFile);
                if (type == 1) Console.WriteLine("バージョン: 1.16以下");
                if (type == 2) Console.WriteLine("バージョン: 1.17以上");
                if (plugin) Console.WriteLine("プラグイン: 有り");
                else Console.WriteLine("プラグイン: 無し");
                Console.WriteLine("Javaのバージョン: " + finalJava);
                Console.WriteLine("(Javaの場所: " + runPath + ")");
                Console.WriteLine("==================================");

                StreamWriter writer = new StreamWriter("サーバー起動.bat");
                if (plugin)
                {
                    writer.WriteLine("\"" + runPath + "\" -jar " + serverFile + " -nogui");
                    writer.WriteLine("pause");
                }
                else writer.WriteLine("start \"\" \"" + runPath + "\" -jar " + serverFile);
                writer.Close();

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("サーバーの起動スクリプトを作成しました。");
                Console.WriteLine("サーバーを起動する際は、「サーバー起動.bat」を開いてください。");
            }

            Console.WriteLine("");
            Console.WriteLine("手続きが終了しました。このウィンドウを閉じても構いません。");
            while (true) { }
        }

        static public bool IsWow64()
        {
            IntPtr wow64Proc = GetProcAddress(GetModuleHandle("Kernel32.dll"), "IsWow64Process");
            if (wow64Proc != IntPtr.Zero)
            {
                bool ret;
                if (IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out ret))
                {
                    return ret;
                }
            }

            return false;
        }

        static public bool Is64BitOS()
        {
            if (IntPtr.Size == 4)
            {
                if (IsWow64())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (IntPtr.Size == 8)
            {
                return true;
            }

            return false;
        }
    }
}

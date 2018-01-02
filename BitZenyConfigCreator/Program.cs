using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace BitZenyConfigCreator
{
    using Model;
    using System.Net.Http;
    using System.Net.Http.Headers;

    class Program
    {
        public const string Crypto = "BitZeny";
        public const string _URL = "http://bitzeny.zinntikumugai.xyz/Config/BitZeny.json";
        public static List<string> _DIRs = new List<string> {
                System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + "AppData" + Path.DirectorySeparatorChar+ "Roaming",    //Windows %appdata%
                System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + "Libray",    //MacOS ~/Libray/
                System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar   //Ubuntu ~/
            };

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            string dir = string.Empty;
            string config = string.Empty;

            Init();
            
            Console.WriteLine("暗号通貨：" + Crypto);


            /* DLしたJSONに変える */
            //JsonModel jm = LoccalJsonLoad();
            JsonModel jm = await GetWebJson();

            if (!isCanOS()) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Warning!!]");
                Console.WriteLine("どうやらこのOSは対応していないようです。");
                Console.WriteLine("開発者にOS名を連絡してください。");
                Console.ResetColor();
                Console.ReadKey(true);
                return;
            }
            dir = getCryptDir(jm.CD);
            if(dir.Equals(string.Empty)) {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("この暗号通貨のディレクトリが作成されていません。");
                Console.WriteLine("ディレクトリ作ってから動かしてください");
                Console.ResetColor();
                Console.ReadKey(true);
                return;
            }
            Console.WriteLine("暗号通貨のディレクトリ:\t" + dir);


            //Node
            string node = string.Empty;
            foreach (string s in jm.Node)
                node += "addnode=" + s + Environment.NewLine;

            config += node;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("######### Configに書き込まれる内容 #########");
            Console.WriteLine(config);
            Console.WriteLine("######### Configに書き込まれる内容 #########");
            Console.ResetColor();

            //configを一旦書き込み
            Encoding enc = new UTF8Encoding(false);
            using (FileStream fs = new FileStream(jm.FileName, FileMode.Create))
            //using(StreamWriter sw =new StreamWriter(fs, Encoding.UTF8)) { //BOM on UTF-8
            using (StreamWriter sw = new StreamWriter(fs, enc)) {   //BOM off UTF-8
                sw.Write(node);
            }

            Console.WriteLine("configファイルを一時的に作成しました");

            string configPath = dir + System.IO.Path.DirectorySeparatorChar + jm.FileName;
            Console.WriteLine("configファイル：" + configPath);

            if (File.Exists(configPath)) {
                Console.WriteLine(jm.FileName + " ファイルが既に存在します。");
                Console.Write("ファイルを上書きしますか？");
            }else {
                Console.Write("ファイルを作成しますか？");
            }
            Console.WriteLine("[y=はい,n=いいえ]");
            bool ynb = false;
            do {
                Console.ForegroundColor = ConsoleColor.White;
                var yn = Console.ReadLine();
                Console.ResetColor();
                ynb = false;
                 if (yn.Equals("Y") || yn.Equals("y")) {
                    ynb = true;
                    break;
                } else if (yn.Equals("N") || yn.Equals("n")) {
                    break;
                }
            } while (!ynb);

            if (ynb) {
                Console.WriteLine("ファイルを書き込みます");
                if (File.Exists(configPath)) {
                    if (File.Exists(configPath + ".old")) {
                        File.Delete(configPath + ".old");
                    }
                    File.Copy(configPath, configPath + ".old");
                    File.Delete(configPath);
                }
                File.Copy(jm.FileName, configPath);
            }else {
                Console.WriteLine("実行ディレクトリに" + jm.FileName + "があるため手動でコピー等行ってください。");
            }
            
            Console.WriteLine("終了しました。");
            Console.ReadKey(true);
        }

        static public bool isCanOS()
        {
            foreach(string os in _DIRs) {
                if (Directory.Exists(os))
                    return true;
            }
            return false;
        }

        static public string getCryptDir(List<CryptDir> dirs = null)
        {
            /*
            System.OperatingSystem os = System.Environment.OSVersion;
            Console.WriteLine("このOSは" + os.ToString() + "です。");
            */
            if (dirs.Equals(null))
                return string.Empty;

            foreach (string ospath in _DIRs) {
                foreach(CryptDir cd in dirs) {
                    string dir = ospath + Path.DirectorySeparatorChar + cd.dir;
                    if (!dirs.Equals(null) && Directory.Exists(dir))
                        return dir;
                }
                
            }
            //このOSは非対応
            return string.Empty;
        }

        static public void Init()
        {
            List<string> Title = new List<string> {
// http://www.patorjk.com/software/taag/#p=display&f=Doom&t=BitZeny%20Config%20Creator
@"______ _ _   ______                   _____              __ _         _____                _             ",
@"| ___ (_) | |___  /                  /  __ \            / _(_)       /  __ \              | |            ",
@"| |_/ /_| |_   / /  ___ _ __  _   _  | /  \/ ___  _ __ | |_ _  __ _  | /  \/_ __ ___  __ _| |_ ___  _ __ ",
@"| ___ \ | __| / /  / _ \ '_ \| | | | | |    / _ \| '_ \|  _| |/ _` | | |   | '__/ _ \/ _` | __/ _ \| '__|",
@"| |_/ / | |_./ /__|  __/ | | | |_| | | \__/\ (_) | | | | | | | (_| | | \__/\ | |  __/ (_| | || (_) | |   ",
@"\____/|_|\__\_____/\___|_| |_|\__, |  \____/\___/|_| |_|_| |_|\__, |  \____/_|  \___|\__,_|\__\___/|_|   ",
@"                               __/ |                           __/ |                                     ",
@"                              |___/                           |___/                                      ",
@"                                                                                                         ",
            };
            foreach (string str in Title)
                Console.WriteLine(str);
        }
        
        static public JsonModel LoccalJsonLoad()
        {
            string strdata = string.Empty;
            JsonModel jm = JsonConvert.DeserializeObject<JsonModel>("{}");
            const string filename = "testData.json";

            if (!File.Exists(filename))
                return jm;

            using(StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open))) {
                while(sr.Peek() > 0) {
                    string line = sr.ReadLine();
                    strdata += line;
                }
            }

            Console.WriteLine("########## JSON ##########");
            Console.WriteLine(strdata);
            Console.WriteLine("########## JSON ##########");
            jm = JsonConvert.DeserializeObject<JsonModel>(strdata);
            return jm;
        }

        public static async Task<JsonModel> GetWebJson()
        {
            string json = string.Empty;
            JsonModel jm = JsonConvert.DeserializeObject<JsonModel>("{}");

            json = await WebGet(_URL);
            jm = JsonConvert.DeserializeObject<JsonModel>(json);
            return jm;

        }

        public static async Task<string> WebGet(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".Net Core BitZeny Config Creator v1");

            Console.WriteLine("Webより取得します。");
            var stringTask = client.GetStringAsync(url);
            Console.WriteLine("Webより取得しました。");

            var str = await stringTask;
            return str;
        }
    }
}

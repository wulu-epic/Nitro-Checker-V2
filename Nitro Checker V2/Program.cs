using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Leaf.xNet;
using System.Collections.Specialized;


namespace Nitro_Checker_V2
{
    
    class Program
    {
        //Code checker
        public static List<string> codes = new List<string>();
        public static List<string> proxies = new List<string>();

        public static string proxyType = string.Empty;
        public static string currentTmie = DateTime.Now.ToString("dd-MM-yyyy hh-mm-ss");
        public static string webhookURL = string.Empty;

        public static int hits;
        public static int invalid;
        public static int ratelimit;
        public static int proxyError;
        public static int cheked;

        public static int comboAmount;
        public static int threads;
        //Code Gen
        public static int codetogen;
        public static int thread1;
        public static ParallelLoopResult parallelLoopResult;

        public static List<string> codesgen = new List<string>();


        public static string wulu = @"
                                    ____    __    ____  __    __   __       __    __  
                                    \   \  /  \  /   / |  |  |  | |  |     |  |  |  | 
                                     \   \/    \/   /  |  |  |  | |  |     |  |  |  | 
                                      \            /   |  |  |  | |  |     |  |  |  | 
                                       \    /\    /    |  `--'  | |  `----.|  `--'  | 
                                        \__/  \__/      \______/  |_______| \______/   
            ";


        [STAThread]
        static void Main(string[] args)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Directory.CreateDirectory(Environment.CurrentDirectory + @"\\Results\\");
            Console.Title = "Discord Nitro Checker & Generator by wulu#0827";

            Console.ForegroundColor = ConsoleColor.Magenta;
      
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));

            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine("[?] [1] ACCOUNT CHECKER [2] GENERATE CODES");
            Console.Write("> ");

            string decision = Console.ReadLine();
            if (decision == "2")
            {
                Console.Clear();
                GenerateCodeCS();
            }
            else
            {
                checkCodes();
            }

            
        }
        public static class Extensions
        {
            public static string Scramble( string s)
            {
                return new string(s.ToCharArray().OrderBy(x => Guid.NewGuid()).ToArray());
            }
        }

        public static void check (string code)
        {
            string url = "https://discordapp.com/api/v9/entitlements/gift-codes/"+code+"?with_application=false&with_subscription_plan=true";
            Leaf.xNet.HttpRequest httpRequest = new Leaf.xNet.HttpRequest();
            httpRequest.UserAgent = Http.ChromeUserAgent();
            if (proxyType == "HTTP")
            {
                httpRequest.Proxy = HttpProxyClient.Parse(randomProxy());
                httpRequest.Proxy.ConnectTimeout = 1000;
            }
            else if (proxyType == "SOCKS4")
            {
                httpRequest.Proxy = Socks4ProxyClient.Parse(randomProxy());
                httpRequest.Proxy.ConnectTimeout = 1000;
            }
            else if (proxyType == "SOCKS5")
            {
                httpRequest.Proxy = Socks5ProxyClient.Parse(randomProxy());
                httpRequest.Proxy.ConnectTimeout = 1000;

            }

            try
            {

                var respionse = httpRequest.Get(url);

                if (respionse.StatusCode==Leaf.xNet.HttpStatusCode.OK || respionse.ToString().Contains("nitro") )
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[+] Valid: " + code);
                    writeFile(code);
                    hits += 1;
                    cheked += 1;
                    if(webhookURL!= String.Empty)
                    {
                        sendWebhookMSG(webhookURL, "DISCORD NITRO CHECKER", "Got a valid discord nitro hit: discord.gift/" + code);
                    }
                }
            }
            catch(Leaf.xNet.HttpException ex)
            {
                if (ex.Message.Contains("404"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("[-] Invalid Code: " + code);
                    invalid += 1;
                    cheked += 1;
                }
                if (ex.Message.Contains("429"))
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.WriteLine("[#] Rate limited: " + code);
                    check(code);
                    ratelimit += 1;
                }

                if (ex.Message.Contains("200"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[+] Valid: " + code);
                    writeFile(code);
                    if (webhookURL != String.Empty)
                    {
                        sendWebhookMSG(webhookURL, "DISCORD NITRO CHECKER", "Got a valid discord nitro hit: discord.gift/" + code);
                    }
                    hits += 1;
                    cheked += 1;
                }


                if (ex.Status.Equals(HttpExceptionStatus.ConnectFailure))
                {
                    proxyError += 1;
                    check(code);
                   
                }
            }
        }
        
        public static void writeFile(string code)
        {
            string file = Environment.CurrentDirectory + @"\\Results\\[Hits] " + currentTmie + ".txt";
            File.AppendAllText(file, code + Environment.NewLine);
        }
        public static void seperateThread()
        {
            parallelLoopResult = Parallel.ForEach(codes, code => {
                new ParallelOptions { MaxDegreeOfParallelism = threads };
                check(code);
                Console.Title = $"Discord Nitro Checker & Generator by wulu#0827 | Hits: {hits} | Invalid: {invalid} | Proxy Error: {proxyError} | Rate limited: {ratelimit} | Remaining: {comboAmount - cheked} ";
            });
            
        }
        public static string randomProxy()
        {
            Random random = new Random();
            string[] ararayProx = proxies.ToArray();
            int indexx = random.Next(ararayProx.Length) ;
            return ararayProx[indexx];
        }
        public static void sendWebhookMSG(string url, string user, string content)
        {
            WebClient wc = new WebClient();
            try
            {
                wc.UploadValues(url, new NameValueCollection
                {
                    {
                        "content", content
                    },
                    {
                        "username", user
                    }
                });
            }
            catch(WebException ex)
            {
                Console.WriteLine(ex.Message);  
            }
        }

        public static void GenerateCodeCS()
        {
            
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));

            Console.Write("[?] How many codes do you want to generate? \n", Console.ForegroundColor = ConsoleColor.Magenta);
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.Blue;
            string codeAmountS = Console.ReadLine();
            codetogen = Convert.ToInt32(codeAmountS);

            Console.Write("[?] How many threads to use? \n", Console.ForegroundColor = ConsoleColor.Magenta);
            Console.Write("> ");

            Console.ForegroundColor = ConsoleColor.Blue;
            string threadS = Console.ReadLine();
            thread1 = Convert.ToInt32(codeAmountS);

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));
            int checke = 0;
            ParallelLoopResult parallelLoopResult = Parallel.For(0, codetogen, x =>
             {
                 Console.ForegroundColor = ConsoleColor.Green;
                 new ParallelOptions
                 {
                     MaxDegreeOfParallelism = thread1
                 };

                 string ass = "abcdefghijklmnopqrstubwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                 string a = Extensions.Scramble(ass);
                 string code = a.Substring(0, 16);

                 Console.WriteLine($"[+] {code}");
                 codesgen.Add(code);
                 checke += 1;
                 Console.Title = $"Discord Nitro Checker & Generator by wulu#0827 | Remaining: {checke}/{codetogen} ";
             });

            if (parallelLoopResult.IsCompleted)
            {
                Console.WriteLine("[!] Completed generating accounts. Save to a file", Console.ForegroundColor = ConsoleColor.Magenta);

                Thread.Sleep(500);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Text files (*.txt)|*.txt";
                sfd.Title = "Save Codes";


                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(sfd.FileName);
                    var file = File.Create(sfd.FileName);
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.AutoFlush = true;
                    foreach (string s in codesgen)
                    {
                        streamWriter.WriteLine(s);
                    }
                    Console.WriteLine("[!] Done saving. Press enter to exit");
                    Console.ReadLine();
                }
            }
        }
        public static void checkCodes()
        {
            Console.Clear();
            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));


            Console.WriteLine("[!] Open codes file then proxy file.");
            Console.Write("> ");

            OpenFileDialog opf = new OpenFileDialog();
            opf.Filter = "Text files (*.txt)|*.txt";
            opf.Title = "Open a codes file";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                foreach (string code in File.ReadAllLines(opf.FileName))
                {
                    codes.Add(code);
                }
            }

            OpenFileDialog pfd = new OpenFileDialog();
            pfd.Filter = "Text files (*.txt)|*.txt";
            pfd.Title = "Open a proxy file";
            if (pfd.ShowDialog() == DialogResult.OK)
            {
                foreach (string code in File.ReadAllLines(pfd.FileName))
                {
                    proxies.Add(code);
                }
            }

            comboAmount = codes.Count;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write($"[!] Successfully inserted {codes.Count} code(s) and {proxies.Count} proxie(s) \n");

            Console.Write("[!] How many threads? \n");
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.Blue;
            string threadsS = Console.ReadLine();
            threads = Convert.ToInt32(threadsS);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[!] Proxy type? [1] HTTP [2] SOCKS4 [3] SOCKS5 \n");
            Console.Write("> ");
            Console.ForegroundColor = ConsoleColor.Blue;
            string prox = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("[!] Want to use discord webhooks? [1] YES [2] NO \n");
            Console.Write("> ");

            Console.ForegroundColor = ConsoleColor.Blue;
            string choice = Console.ReadLine();
            if (choice == "1")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("Enter your discord webhook URL \n");
                Console.Write("> ");
                Console.ForegroundColor = ConsoleColor.Blue;
                string url = Console.ReadLine();
                webhookURL = url;
            }

            switch (prox)
            {
                case "1":
                    proxyType = "HTTP";
                    break;
                case "2":
                    proxyType = "SOCKS4";
                    break;
                case "3":
                    proxyType = "SOCKS5";
                    break;
            }

            Console.Clear();
            Thread thread = new Thread(seperateThread);
            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine(String.Format("{0," + Console.WindowWidth / 2 + "}", wulu));
            thread.Start();

            while(parallelLoopResult.IsCompleted != true)
            {
                if (parallelLoopResult.IsCompleted)
                {
                    Console.Title = $"Discord Nitro Checker & Generator by wulu#0827 | Hits: {hits} | Invalid: {invalid} | Proxy Error: {proxyError} | Rate limited: {ratelimit} | Remaining: {comboAmount - cheked} ";
                    Console.WriteLine($"[!] Finished checking codes. Results: Hits: {hits} | Invalid: {invalid} | Proxy Error: {proxyError} | Rate limited: {ratelimit} | Remaining: {comboAmount - cheked}");
                    Console.ReadLine();
                }
            }
        }
    }
}

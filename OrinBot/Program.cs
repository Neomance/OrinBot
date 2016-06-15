using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace OrinBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Start();
        }

        private DiscordClient _client;


        public void Start()
        {
            _client = new DiscordClient(x =>
            {
                x.AppName = "Orin Bot";
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            _client.UsingCommands(x =>
            {
                x.PrefixChar = '~';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
            });

            var token = "MTkyNTA3MjQ5Njc4MjIxMzEz.CkJ5ZA.mCaK7vFM50cJUAebZH1PtUwpXPA";

            CreateCommands();

            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token);
            });

        }

        public void CreateCommands()
        {
            var cService = _client.GetService<CommandService>();

            cService.CreateCommand("ping")
                .Description("Says something for now")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Orin kawaii");
                });

            cService.CreateCommand("hello")
                .Description("says hello to someone")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    //var toReturn = $"Hello {e.GetArg("user")}";
                    var toReturn = "Hello " + e.GetArg("user");
                    await e.Channel.SendMessage(toReturn);
                });

            cService.CreateCommand("orin")
                .Description("sends orin")
                .Do(async (e) =>
                {
                    await e.Channel.SendFile("../../images/orin.jpg");
                    await e.Channel.SendMessage("Nyaa");
                });

            cService.CreateCommand("cat")
                .Description("random cat picture")
                .Do(async (e) =>
                {
                    //await e.Channel.SendFile("../../images/orin.jpg");
                    await e.Channel.SendMessage("No cats here");
                });

            cService.CreateCommand("master")
                .Description("sends motivation")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("You can do it, Believe in yourself");
                });
                
            cService.CreateCommand("study")
                .Description("Gets Info Online")
                .Parameter("key", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    //var toReturn = $"Hello {e.GetArg("user")}";
                   
                    var toReturn = webReq(e.GetArg("key"));
                    await e.Channel.SendMessage(toReturn);
                });

        }

        //public void Log(object sender, LogMessageEventArgs e)
        //{
        //    Console.WriteLine($"[{e.Severity}] [{e.Source}] {e.Message}");
        //}

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine("Log disabled");
        }
        
        public String webReq(String param)
        {
            param = param.Replace(" ", "%20");
            WebRequest req = WebRequest.Create("http://api.wolframalpha.com/v2/query?appid=HR24V9-9Q2J72WX96&input="+param+"&format=plaintext");
            WebResponse resp = req.GetResponse();
            String res = ((HttpWebResponse)resp).StatusDescription;
            //Debug.WriteLine(((HttpWebResponse)resp).StatusDescription);
            if (res.Equals("OK"))
            {
                Stream ds = resp.GetResponseStream();
                StreamReader rdr = new StreamReader(ds);
                String result = rdr.ReadToEnd();
                if (result.Length > 0)
                {
                    String xmls = parxml(result);
                    if (xmls.Length > 0)
                    {
                        return xmls;
                    }
                }
                rdr.Close();
            }
            resp.Close();

            return "No Results";
        }

        public String parxml(String tx)
        {
            String result = "";
            using (XmlReader rd = XmlReader.Create(new StringReader(tx)))
            {
                rd.ReadToFollowing("pod");
                rd.MoveToFirstAttribute();
                String re = rd.Value;
                while (!re.Equals("Result"))
                {
                    rd.ReadToFollowing("pod");
                    rd.MoveToNextAttribute();
                    re = rd.Value;
                }
                if (re.Equals("Result"))
                {
                    //Debug.WriteLine(re);
                    try{
                    rd.ReadToFollowing("plaintext");
                    //Debug.WriteLine(rd.ReadElementContentAsString());
                        result = rd.ReadElementContentAsString();
                    }catch(Exception noRes){

                    }
                    try
                    {
                        rd.ReadToFollowing("imagesource");
                        result = result + " " + rd.ReadElementContentAsString();
                        //Debug.WriteLine(rd.ReadElementContentAsString());
                    }
                    catch (Exception noNode)
                    {

                    }

                }
            }
            return result;
        }
    }
}


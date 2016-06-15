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

            _client.ExecuteAndWait(async() => 
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
                    var toReturn = $"Hello {e.GetArg("user")}";
                    await e.Channel.SendMessage(toReturn);
                });

            cService.CreateCommand("orin")
                .Description("sends orin")
                .Do(async (e) =>
                {
                    await e.Channel.SendFile("../../images/orin.jpg");
                    await e.Channel.SendMessage("Nyaa");
                });


        }

        public void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] [{e.Source}] {e.Message}");
        }
    }
}

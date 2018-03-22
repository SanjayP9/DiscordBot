using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;

namespace DiscordBot
{
    class Program
    {
        // Mimics async ins sync method
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;
        

        private async Task RunBotAsync()
        {
            try
            {
                await Modules.PageScrape.Initialise();
                DiscordSocketConfig config = new DiscordSocketConfig() { MessageCacheSize = 500 };
                client = new DiscordSocketClient(config);
                commands = new CommandService();
                services = new ServiceCollection()
                    .AddSingleton(client)
                    .AddSingleton(commands)
                    .BuildServiceProvider();

                Modules.APIKeys.RetrieveAPIKey();

                string botToken = Modules.APIKeys.DiscordAPIKey;

                // event subscriptions
                client.Log += Log;
                client.UserJoined += AnnounceUserJoined;

                await RegisterCommandsAsync();
                await client.LoginAsync(TokenType.Bot, botToken);
                await client.StartAsync();
                await Task.Delay(-1);
            }
            catch(Exception e)
            {
               Console.WriteLine(e.ToString());
            }
        }

        private async Task ErrorLog(string error)
        {
           // await ReplyAsync("", false, new EmbedBuilder().WithTitle(error).Build());
        }

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"{user.Mention} is here");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(client, message);

                var result = await commands.ExecuteAsync(context, argPos, services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}

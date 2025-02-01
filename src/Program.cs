using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Hmm.src.Handlers;

namespace Hmm
{
  public class Program
  {
    private DiscordSocketClient? _client;
    private CommandHandler? _commandHandler;
    private IServiceProvider? _services;
    private readonly string[] _prefixes;
    private readonly string _token;

    public Program()
    {
      var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
          File.ReadAllText("src/Config/config.json"))
          ?? throw new InvalidOperationException("oopsie cant load config owo");

      _token = config["token"].GetString()
          ?? throw new InvalidOperationException("please give me your token daddy uwu");
      _prefixes = config["prefixes"].EnumerateArray()
          .Select(x => x.GetString())
          .Where(x => x != null)
          .ToArray()!;
    }

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
      _services = ConfigureServices();
      _client = _services.GetRequiredService<DiscordSocketClient>();
      _commandHandler = _services.GetRequiredService<CommandHandler>();

      _client.Log += LogAsync;
      await _commandHandler.InitializeAsync();

      await _client.LoginAsync(TokenType.Bot, _token);
      await _client.StartAsync();
      await Task.Delay(Timeout.Infinite);
    }

    private Task LogAsync(LogMessage msg)
    {
      Console.WriteLine(msg.ToString());
      return Task.CompletedTask;
    }

    private IServiceProvider ConfigureServices()
    {
      return new ServiceCollection()
          .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
          {
            GatewayIntents = GatewayIntents.Guilds
                  | GatewayIntents.GuildMessages
                  | GatewayIntents.MessageContent,
            LogLevel = LogSeverity.Info
          }))
          .AddSingleton<CommandService>()
          .AddSingleton(x => new CommandHandler(
              x,
              x.GetRequiredService<DiscordSocketClient>(),
              x.GetRequiredService<CommandService>(),
              _prefixes))
          .BuildServiceProvider();
    }
  }
}
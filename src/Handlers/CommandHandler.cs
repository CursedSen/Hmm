using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;

namespace Hmm.src.Handlers
{
  public class CommandHandler
  {
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly string[] _prefixes;

    public CommandHandler(
        IServiceProvider services,
        DiscordSocketClient client,
        CommandService commands,
        string[] prefixes)
    {
      _client = client;
      _commands = commands;
      _services = services;
      _prefixes = prefixes;
    }

    public async Task InitializeAsync()
    {
      await _commands.AddModulesAsync(
          assembly: Assembly.GetEntryAssembly(),
          services: _services);

      _client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
      if (messageParam is not SocketUserMessage message) return;
      if (message.Author.IsBot) return;

      var argPos = 0;
      if (!_prefixes.Any(x => message.HasStringPrefix(x, ref argPos)))
        return;

      var context = new SocketCommandContext(_client, message);
      await _commands.ExecuteAsync(
          context: context,
          argPos: argPos,
          services: _services);
    }
  }
}
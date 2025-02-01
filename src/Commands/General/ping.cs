using Discord.Commands;

namespace Hmm.Commands.General
{
  public class GeneralCommands : ModuleBase<SocketCommandContext>
  {
    [Command("ping")]
    public async Task PingAsync()
    {
      await ReplyAsync("shut the fuck up");
    }
  }
}
// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater.RainbowUtlities;

Console.WriteLine("Hello, World!");

var bot = new TelegramBotClient("BOT_TOKEN");

var rainbow = new Rainbow<long, Update>(5, x=> x.Message.From.Id);

// ---- write updates ----
_ = Task.Run(async () =>
{
    var offset = 0;
    while (true)
    {
        var updates = await bot.GetUpdatesAsync(offset, 100, 1000, new[] { UpdateType.Message });

        foreach (var update in updates)
        {
            await rainbow.WriteAsync(update);
            offset = update.Id + 1;
        }
    }
});

async Task HandleUpdate(
    ShinigInfo<long, Update> shinigInfo, CancellationToken cancellationToken)
{
    switch (shinigInfo.Value)
    {
        case { Message: { Text: { } text, From: { } from } msg }:
            {
                if (shinigInfo.TryCountPending(out var count))
                {
                    if (count >= 5)
                    {
                        var m = await bot!.SendTextMessageAsync(
                            from.Id, $"You sent too much requests ({count})! I'll drop them",
                            cancellationToken: cancellationToken);

                        shinigInfo.DropPendingAsync();
                    }
                    else
                    {
                        var m = await bot!.SendTextMessageAsync(
                            from.Id, $"Handling your request! you have {count} pending requests.",
                            cancellationToken: cancellationToken);

                        await Task.Delay(5000, cancellationToken);
                        await bot.SendTextMessageAsync(
                            from.Id, "Requests handled",
                            replyToMessageId: m.MessageId,
                            cancellationToken: cancellationToken);
                    }
                }
                break;
            }
    }
}

Task ErrorHandler(Exception ex, CancellationToken arg2)
{
    Console.WriteLine(ex);
    return Task.CompletedTask;
}

await rainbow.ShineAsync(HandleUpdate, ErrorHandler);

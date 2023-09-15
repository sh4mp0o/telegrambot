using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("6326545310:AAHr_k9p1tO238D0xszOy84VPww2kBklUgc");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    var username = message.Chat.Username;

    Console.WriteLine($"Received a '{messageText}' message in chat {username}  {chatId}.");

    #region dice
    Random rnd = new Random();

    //// Echo received message text
    //Message sentMessage = await botClient.SendTextMessageAsync(
    //    chatId: chatId,
    //    text: $"i choose {rnd.Next(1,6)}",
    //    cancellationToken: cancellationToken);


    //Message sentStickerMessage = await botClient.SendDiceAsync(
    //    chatId: chatId,
    //    cancellationToken: cancellationToken);
    #endregion

    await JustResponse(botClient, cancellationToken, message);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

async Task JustResponse(ITelegramBotClient botClient, CancellationToken cancellationToken, Message? message)
{
    InlineKeyboardMarkup startKeyBoard = new(new[]
    {
        // first row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Записаться!👀", callbackData: "11"),
        },
        // second row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Контакты📱", callbackData: "21"),
        },
        // third row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "Отзывы✨", callbackData: "21"),
        },
    });

    var chatId = message.Chat.Id;

    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Привет, это первый Ярославский бот по записи на маникюр в телеграме!",
        replyMarkup: startKeyBoard,
        cancellationToken: cancellationToken);
}

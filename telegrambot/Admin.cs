using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegrambot
{
    class Admin
    {
        public const int id = 456518653;
        public static async Task AdminUpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        var user = message.From;

                        var chat = message.Chat;

                        if (message.Type == MessageType.Text)
                        {
                            if (message.Text == "/start")
                            {
                                await botClient.SendTextMessageAsync(
                                    chat.Id,
                                    "Приветствую, Админ!",
                                    replyMarkup: IKeyboards.adminMainMenu,
                                    cancellationToken: cancellationToken);
                            }
                        }
                        return;
                    }
                case UpdateType.CallbackQuery:
                    {
                        var callbackQuery = update.CallbackQuery;

                        var user = callbackQuery.From;

                        var chat = callbackQuery.Message.Chat;

                        Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                        switch (callbackQuery.Data.Split().First())
                        {
                            case "existRecsButton":
                                {
                                    await botClient.EditMessageTextAsync(
                                          chat.Id,
                                          callbackQuery.Message.MessageId,
                                          $"Список записанных клиентов:",
                                          replyMarkup: IKeyboards.backExistRecs,
                                          cancellationToken: cancellationToken);
                                    return;
                                }
                            case "editRecsButton":
                                {
                                    await botClient.EditMessageTextAsync(
                                          chat.Id,
                                          callbackQuery.Message.MessageId,
                                          $"Выберите клиента, чтобы отредактировать его запись",
                                          replyMarkup: IKeyboards.backEditRecs,
                                          cancellationToken: cancellationToken);
                                    return;
                                }
                            case "backExistRecs":
                                {
                                    await botClient.EditMessageTextAsync(
                                        chat.Id,
                                        callbackQuery.Message.MessageId,
                                        "Приветствую, Админ!",
                                        replyMarkup: IKeyboards.adminMainMenu,
                                        cancellationToken: cancellationToken);
                                    return;
                                }
                            case "backEditRecs":
                                {
                                    goto case "backExistRecs";
                                }
                        }
                        return;
                    }
            }
        }
    }
}

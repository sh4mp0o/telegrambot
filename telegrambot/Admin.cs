using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegrambot
{
    class Admin
    {
        public const int id = 1384604605;
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
                                    List<Client> clients = SerializationOfClient.Deserialization();
                                    clients.Sort();
                                    string text = null;
                                    text += "Список записанных клиентов:\n";
                                    for (int i=0; i<clients.Count; i++)
                                    {
                                        text += clients[i].Phone+" " + clients[i].Username +" "+ clients[i].DateTime.Day.ToString()+"." + clients[i].DateTime.Month +" "+ clients[i].Time + "\n"; 
                                    }
                                    await botClient.EditMessageTextAsync(
                                          chat.Id,
                                          callbackQuery.Message.MessageId,
                                          text:text,
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
                                          replyMarkup: IKeyboards.BackEditRecs(),
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
                            case "redaction":
                                {
                                    await botClient.EditMessageTextAsync(
                                        chat.Id,
                                        callbackQuery.Message.MessageId,
                                        "Выберите как хотите отредактировать:",
                                        replyMarkup: IKeyboards.Editing(callbackQuery.Data.Split().Last()),
                                        cancellationToken: cancellationToken);
                                    return;
                                }
                            case "delete":
                                {
                                    List<Client> clients = SerializationOfClient.Deserialization();
                                    clients.Remove(clients.Find(x => x.Id.ToString() == callbackQuery.Data.Split().Last()));
                                    SerializationOfClient.Serialization(clients);
                                    goto case "editRecsButton";
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

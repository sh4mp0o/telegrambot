using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using tgbot;

namespace telegrambot
{
    internal class Admin
    {
        private static List<Client> clients = new List<Client>();
        public const long id = 456518653;
        public static string idclient = null;
        private static SerializationOfClient serializationOfClient = new SerializationOfClient(new JSONSerialization());
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
                                    List<Client> clients = serializationOfClient.Deserialization();
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
                                    idclient = callbackQuery.Data.Split().Last();
                                    await botClient.EditMessageTextAsync(
                                        chat.Id,
                                        callbackQuery.Message.MessageId,
                                        "Выберите как хотите отредактировать:",
                                        replyMarkup: IKeyboards.Editing(),
                                        cancellationToken: cancellationToken);
                                    return;
                                }
                            case "delete":
                                {
                                    List<Client> clients = serializationOfClient.Deserialization();
                                    clients.Remove(clients.Find(x => x.Id.ToString() == idclient));
                                    Program.Delete(idclient);
                                    serializationOfClient.Serialization(clients);
                                    goto case "editRecsButton";
                                }
                            case "recButton":
                                {
                                    Client client = new() { Id = long.Parse(idclient)};
                                    clients.Add(client);
                                    await botClient.EditMessageTextAsync(
                                          chat.Id,
                                          callbackQuery.Message.MessageId,
                                          $"Выберите дату💅🏼",
                                          replyMarkup: IKeyboards.Day(),
                                          cancellationToken: cancellationToken);

                                    return;
                                }
                            case "day":
                                {
                                    clients.Find(x => x.Id == long.Parse(idclient)).DateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));
                                    await botClient.EditMessageTextAsync(
                                          chat.Id,
                                          callbackQuery.Message.MessageId,
                                          $"Выберите время💅🏼",
                                          replyMarkup: IKeyboards.Time(long.Parse(idclient), clients),
                                          cancellationToken: cancellationToken);

                                    return;
                                }
                            case "time":
                                {
                                    var day = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Day;
                                    var month = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Month;
                                    var time = clients.Find(x => x.Id == long.Parse(idclient)).Time;

                                    clients.Find(x => x.Id == long.Parse(idclient)).Time = callbackQuery.Data.Split().Last();

                                    time = callbackQuery.Data.Split().Last();

                                    await botClient.EditMessageTextAsync(
                                        chat.Id,
                                        callbackQuery.Message.MessageId,
                                        $"Вы хотите записаться на {day}.{month}" +
                                        $" в {time} " +
                                        "Все верно?",
                                        replyMarkup: IKeyboards.confirmKeyboard,
                                        cancellationToken: cancellationToken);

                                    return;
                                }
                            case "confirmButton":
                                {
                                    var day = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Day;
                                    var month = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Month;
                                    var time = clients.Find(x => x.Id == long.Parse(idclient)).Time;

                                    await botClient.AnswerCallbackQueryAsync(
                                        callbackQuery.Id, $"Вы записаны на {day}.{month}" +
                                        $" в {clients.Find(x => x.Id == long.Parse(idclient)).Time}!",
                                        cancellationToken: cancellationToken);

                                    List<Client> Clients = serializationOfClient.Deserialization();
                                    Clients.Find(x => x.Id == long.Parse(idclient)).Time = clients.Find(x => x.Id == long.Parse(idclient)).Time;
                                    Clients.Find(x => x.Id == long.Parse(idclient)).DateTime = clients.Find(x => x.Id == long.Parse(idclient)).DateTime;
                                    clients.RemoveAt(0);
                                    serializationOfClient.Serialization(Clients);
                                    goto case "editRecsButton";
                                    //456518653 - id Егора
                                    //1384604605 - id Матвея
                                    //5079754639 - id Витали
                                }
                            case "backDays": //? alternative of backbutton from recording.cs
                                {
                                    try
                                    {
                                        clients.Remove(clients.Find(x => x.Id == long.Parse(idclient)));
                                    }
                                    catch (Exception) { }

                                    goto case "redaction";
                                }
                            case "backTime":
                                {
                                    try
                                    {
                                        clients.Find(x => x.Id == long.Parse(idclient)).DateTime = DateTime.Today;
                                    }
                                    catch (Exception) { }
                                    await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                        $"Выберите дату 💅🏼",
                                        replyMarkup: IKeyboards.Day(),
                                        cancellationToken: cancellationToken);

                                    return;
                                }
                            case "backConfirm":
                                {
                                    try
                                    {
                                        clients.Find(x => x.Id == long.Parse(idclient)).Time = "Nah";
                                    }
                                    catch (Exception) { }
                                    await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                        $"Выберите время💅🏼",
                                        replyMarkup: IKeyboards.Time(long.Parse(idclient), clients),
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

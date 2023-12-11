using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using tgbot;


//456518653 - id Егора
//1384604605 - id Матвея
//5079754639 - id Витали

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
                                _ = IMethods.AdminStartUp(botClient, update, cancellationToken);
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

                                    _ = IMethods.ExitRecs(botClient, update, cancellationToken, clients);

                                    return;
                                }
                            case "editRecsButton":
                                {
                                    _ = IMethods.EditRecs(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backExistRecs":
                                {
                                    _ = IMethods.BackToStart(botClient, update, cancellationToken);

                                    return;
                                }
                            case "redaction":
                                {
                                    idclient = callbackQuery.Data.Split().Last();

                                    _ = IMethods.Redaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "delete":
                                {
                                    List<Client> clients = serializationOfClient.Deserialization();
                                    clients.Remove(clients.Find(x => x.Id.ToString() == idclient));
                                    Program.Delete(idclient);
                                    serializationOfClient.Serialization(clients);

                                    _ = IMethods.EditRecs(botClient, update, cancellationToken);

                                    return;
                                }
                            case "recButton":
                                {
                                    Client client = new() { Id = long.Parse(idclient) };
                                    clients.Add(client);

                                    _ = IMethods.RecordRedaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "day":
                                {
                                    clients.Find(x => x.Id == long.Parse(idclient)).DateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));
                                    InlineKeyboardMarkup kb = IKeyboards.Time(long.Parse(idclient), clients);

                                    _ = IMethods.DayRedaction(botClient, update, cancellationToken, kb);

                                    return;
                                }
                            case "time":
                                {
                                    _ = IMethods.TimeRedaction(botClient, update, cancellationToken, clients, idclient);

                                    return;
                                }
                            case "confirmButton":
                                {
                                    _ = IMethods.AdminConfirmation(botClient, update, cancellationToken, clients, idclient);

                                    List<Client> Clients = serializationOfClient.Deserialization();
                                    Clients.Find(x => x.Id == long.Parse(idclient)).Time = clients.Find(x => x.Id == long.Parse(idclient)).Time;
                                    Clients.Find(x => x.Id == long.Parse(idclient)).DateTime = clients.Find(x => x.Id == long.Parse(idclient)).DateTime;
                                    clients.RemoveAt(0);
                                    serializationOfClient.Serialization(Clients);

                                    _ = IMethods.EditRecs(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backDays": //? alternative of backbutton from recording.cs
                                {
                                    try
                                    {
                                        clients.Remove(clients.Find(x => x.Id == long.Parse(idclient)));
                                    }
                                    catch (Exception) { }

                                    idclient = callbackQuery.Data.Split().Last();

                                    _ = IMethods.Redaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backTime":
                                {
                                    try
                                    {
                                        clients.Find(x => x.Id == long.Parse(idclient)).DateTime = DateTime.Today;
                                    }
                                    catch (Exception) { }

                                    _ = IMethods.RecordRedaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backConfirm":
                                {
                                    try
                                    {
                                        clients.Find(x => x.Id == long.Parse(idclient)).Time = "Nah";
                                    }
                                    catch (Exception) { }

                                    InlineKeyboardMarkup kb = IKeyboards.Time(long.Parse(idclient), clients);

                                    _ = IMethods.DayRedaction(botClient, update, cancellationToken, kb);

                                    return;
                                }
                            case "backEditRecs":
                                {
                                    _ = IMethods.BackToStart(botClient, update, cancellationToken);

                                    return;
                                }
                        }
                        return;
                    }
            }
        }
    }
}

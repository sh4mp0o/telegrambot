using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using tgbot;

namespace telegrambot
{
    internal static class Admin
    {
        private static List<Client> clients = new List<Client>();
        public const long id = 1384604606;
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
                                _ = Methods.AdminStartUp(botClient, update, cancellationToken);
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

                                    _ = Methods.ExitRecs(botClient, update, cancellationToken, clients);

                                    return;
                                }
                            case "editRecsButton":
                                {
                                    _ = Methods.EditRecs(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backExistRecs":
                                {
                                    _ = Methods.BackToStart(botClient, update, cancellationToken);

                                    return;
                                }
                            case "redaction":
                                {
                                    idclient = callbackQuery.Data.Split().Last();

                                    _ = Methods.Redaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "delete":
                                {
                                    List<Client> clients = serializationOfClient.Deserialization();
                                    clients.Remove(clients.Find(x => x.Id.ToString() == idclient));
                                    Program.Delete(idclient);
                                    serializationOfClient.Serialization(clients);

                                    _ = Methods.EditRecs(botClient, update, cancellationToken);

                                    return;
                                }
                            case "recButton":
                                {
                                    Client client = new() { Id = long.Parse(idclient) };
                                    clients.Add(client);

                                    _ = Methods.RecordRedaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "day":
                                {
                                    clients.Find(x => x.Id == long.Parse(idclient)).DateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));
                                    InlineKeyboardMarkup kb = Keyboards.Time(long.Parse(idclient), clients);

                                    _ = Methods.DayRedaction(botClient, update, cancellationToken, kb);

                                    return;
                                }
                            case "time":
                                {
                                    _ = Methods.TimeRedaction(botClient, update, cancellationToken, clients, idclient);

                                    return;
                                }
                            case "confirmButton":
                                {
                                    _ = Methods.AdminConfirmation(botClient, update, cancellationToken, clients, idclient);

                                    List<Client> Clients = serializationOfClient.Deserialization();
                                    Clients.Find(x => x.Id == long.Parse(idclient)).Time = clients.Find(x => x.Id == long.Parse(idclient)).Time;
                                    Clients.Find(x => x.Id == long.Parse(idclient)).DateTime = clients.Find(x => x.Id == long.Parse(idclient)).DateTime;
                                    clients.RemoveAt(0);
                                    serializationOfClient.Serialization(Clients);

                                    _ = Methods.EditRecs(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backDays": 
                                {
                                    try
                                    {
                                        clients.Remove(clients.Find(x => x.Id == long.Parse(idclient)));
                                    }
                                    catch (Exception) { }

                                    idclient = callbackQuery.Data.Split().Last();

                                    _ = Methods.Redaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backTime":
                                {
                                    try
                                    {
                                        clients.Find(x => x.Id == long.Parse(idclient)).DateTime = DateTime.Today;
                                    }
                                    catch (Exception) { }

                                    _ = Methods.RecordRedaction(botClient, update, cancellationToken);

                                    return;
                                }
                            case "backConfirm":
                                {
                                    try
                                    {
                                        clients.Find(x => x.Id == long.Parse(idclient)).Time = "Nah";
                                    }
                                    catch (Exception) { }

                                    InlineKeyboardMarkup kb = Keyboards.Time(long.Parse(idclient), clients);

                                    _ = Methods.DayRedaction(botClient, update, cancellationToken, kb);

                                    return;
                                }
                            case "backEditRecs":
                                {
                                    _ = Methods.BackToStart(botClient, update, cancellationToken);

                                    return;
                                }
                        }
                        return;
                    }
            }
        }
    }
}

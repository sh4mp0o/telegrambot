using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegrambot;
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace tgbot
{
    internal class Program : IMethods
    {
        private static ITelegramBotClient? _botClient;
        private static ReceiverOptions? _receiverOptions;
        private static List<Client>? _clients;
        static bool flag = false;
        private static SerializationOfClient serializationOfClient = new SerializationOfClient(new JSONSerialization());

        static async Task Main()
        {
            _botClient = new TelegramBotClient("6326545310:AAHr_k9p1tO238D0xszOy84VPww2kBklUgc"); // TOKEN HERE
            _clients = serializationOfClient.Deserialization();

            _receiverOptions = new ReceiverOptions // bot settings
            {
                AllowedUpdates = new[] 
                {
                    UpdateType.Message, 
                    UpdateType.CallbackQuery
                },
                ThrowPendingUpdates = true, // take an update while bot was offline or not
            };

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, IMethods.ErrorHandler, _receiverOptions, cts.Token);

            var bot = await _botClient.GetMeAsync();

            Console.WriteLine($"{bot.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var message = update.Message;

                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var user = message.From;

                            var chat = message.Chat;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            switch (message.Type)
                            {
                                case MessageType.Text:

                                    if (message.From.Id == Admin.id)
                                    {
                                        _ = Admin.AdminUpdateHandler(botClient, update, cancellationToken);

                                        return;
                                    }

                                    if (message.Text == "/start")
                                    {
                                        _ = IMethods.StartAsync(botClient, update, cancellationToken);

                                        return;
                                    }
                                    return;
                                case MessageType.Contact:
                                    {
                                        if (message.Type == MessageType.Contact && message.Contact != null && flag)
                                        {
                                            Console.WriteLine($"Phone number: {message.Contact.PhoneNumber}");

                                            _clients.Find(x => x.Id == update.Message.From.Id).Phone = message.Contact.PhoneNumber;
                                            serializationOfClient.Serialization(_clients);
                                            //SerializationOfClient.Serialization(_clients);

                                            _ = IMethods.SendContactAsync(botClient, update, cancellationToken, _clients);

                                            flag = false;
                                        }

                                        return;
                                    }
                                default: return;
                            }
                        }
                    case UpdateType.CallbackQuery:
                        {

                            if (update.CallbackQuery.From.Id == Admin.id)
                            {
                                _ = Admin.AdminUpdateHandler(botClient, update, cancellationToken);

                                return;
                            }

                            var callbackQuery = update.CallbackQuery;
                            
                            var user = callbackQuery.From;

                            var chat = callbackQuery.Message.Chat;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                            switch (callbackQuery.Data.Split().First())
                            {

                                case "recButton":
                                    {
                                        Client client = new() { Id = callbackQuery.From.Id, Username = chat.Username };
                                        _clients.Add(client);

                                        _ = IMethods.CallRecButton(botClient, update, cancellationToken);

                                        return;
                                    }
                                case "contactButton":
                                    {
                                        _ = IMethods.SendContactInfoAsync(botClient, update, cancellationToken);

                                        return;
                                    }
                                case "day":
                                    {
                                        _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));

                                        _ = IMethods.ChooseDayAsync(botClient, update, cancellationToken, _clients);

                                        return;
                                    }
                                case "backContacts":
                                    {
                                        _ = IMethods.BackContacts(botClient, update, cancellationToken);

                                        return;
                                    }
                                case "backDays": //? alternative of backbutton from recording.cs
                                    {
                                        try
                                        {
                                            _clients.Remove(_clients.Find(x => x.Id == callbackQuery.From.Id));
                                        }
                                        catch (Exception) { }

                                        _ = IMethods.BackContacts(botClient, update, cancellationToken);

                                        return;
                                    }
                                case "backTime":
                                    {
                                        try
                                        {
                                            _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime = DateTime.Today;
                                        }
                                        catch (Exception) { }
                                        
                                        _ = IMethods.CallRecButton(botClient, update, cancellationToken);

                                        return;
                                    }
                                case "backConfirm":
                                    {
                                        try
                                        {
                                            _clients.Find(x => x.Id == callbackQuery.From.Id).Time = "Nah";
                                        }
                                        catch (Exception) { }

                                        _ = IMethods.ChooseDayAsync(botClient, update, cancellationToken, _clients);

                                        return;
                                    }
                                case "time":
                                    {
                                        _ = IMethods.ConfirmationQuest(botClient, update, cancellationToken, _clients);

                                        return;
                                    }
                                case "confirmButton":
                                    {
                                        flag = true;

                                        _ = IMethods.Confirmation(botClient, update, cancellationToken, _clients);

                                        _ = IMethods.RequestContact(botClient, callbackQuery.Message.Chat.Id);
                                        //456518653 - id Егора
                                        //1384604605 - id Матвея
                                        //5079754639 - id Витали
                                        return;
                                    }
                            }

                            return;
                        }
                    default: return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void Delete(string id)
        {
            _clients.Remove(_clients.Find(x => x.Id.ToString() == id));
        }
        public static void Change(string id,List<Client> clients)
        {
            _clients.Find(x => x.Id == long.Parse(id)).Time = clients.Find(x => x.Id == long.Parse(id)).Time;
            _clients.Find(x => x.Id == long.Parse(id)).DateTime = clients.Find(x => x.Id == long.Parse(id)).DateTime;
        }
    }
}
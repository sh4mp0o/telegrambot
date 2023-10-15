using System.Runtime.Serialization.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegrambot;

namespace tgbot
{
    internal class Program
    {
        private static ITelegramBotClient? _botClient;
        private static ReceiverOptions? _receiverOptions;
        private static List<Client> _clients;


        static async Task Main()
        {
            _botClient = new TelegramBotClient("6326545310:AAHr_k9p1tO238D0xszOy84VPww2kBklUgc"); // TOKEN HERE


            var json = new DataContractJsonSerializer(typeof(List<Client>));
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead("Clients.json"))
                {
                    _clients = (List<Client>)json.ReadObject(fstream);
                }
            }
            catch (Exception ex)
            {
                _clients = new();
            }

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

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            var bot = await _botClient.GetMeAsync();

            Console.WriteLine($"{bot.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //TODO попробовать описать логичнее удаление предыдущего сообщения, иначе могут возникнуть коллизии после первой итерации, через botMessage.Id
            //TODO реализовать более унифицированный и лакончиный код

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
                                    if (message.Text == "/start")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: IKeyboards.mainMenu,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                    return;
                                case MessageType.Contact:
                                    {
                                        if (message.Type == MessageType.Contact && message.Contact != null)
                                        {
                                            Console.WriteLine($"Phone number: {message.Contact.PhoneNumber}");
                                        }
                                        return;
                                    }
                                default: return;
                            }
                        }
                    case UpdateType.CallbackQuery:
                        {
                            var callbackQuery = update.CallbackQuery;
                            
                            var user = callbackQuery.From;

                            var chat = callbackQuery.Message.Chat;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                            switch (callbackQuery.Data.Split().First())
                            {
                                // Data - это придуманный нами id кнопки, мы его указывали в параметре
                                // callbackData при создании кнопок. У меня это button1, button2 и button3

                                case "recButton":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        Client client = new() { Id = callbackQuery.From.Id, Username = chat.Username };
                                        _clients.Add(client);

                                        await botClient.EditMessageTextAsync(
                                              chat.Id,
                                              callbackQuery.Message.MessageId,
                                              $"Выберите дату 💅🏼",
                                              replyMarkup: IKeyboards.daysKeyboard,
                                              cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "contactButton":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "Номер телефона - +7-930-117-58-31.",
                                            replyMarkup: IKeyboards.backContacts, cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "day":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));

                                        await botClient.EditMessageTextAsync(
                                              chat.Id,
                                              callbackQuery.Message.MessageId,
                                              $"Выберите время💅🏼",
                                              replyMarkup: IKeyboards.Time(callbackQuery.From.Id, _clients),
                                              cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backContacts":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: IKeyboards.mainMenu,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backDays": //? alternative of backbutton from recording.cs
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        _clients.Remove(_clients.Find(x => x.Id == callbackQuery.From.Id));
                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: IKeyboards.mainMenu,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backTime":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime = DateTime.Today;
                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            $"Выберите дату 💅🏼",
                                            replyMarkup: IKeyboards.daysKeyboard,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backConfirm":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).Time = "Nah";
                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            $"Выберите время💅🏼",
                                            replyMarkup: IKeyboards.Time(callbackQuery.From.Id, _clients),
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "time":
                                    {
                                        var day = _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Day;
                                        var month = _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Month;
                                        var time = _clients.Find(x => x.Id == callbackQuery.From.Id).Time;

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).Time = callbackQuery.Data.Split().Last();
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

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
                                        var day = _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Day;
                                        var month = _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Month;
                                        var time = _clients.Find(x => x.Id == callbackQuery.From.Id).Time;

                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id,
                                            $"Вы записаны на {day}.{month}" +
                                            $" в {_clients.Find(x => x.Id == callbackQuery.From.Id).Time}!");

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).Confirmation = true;

                                        var clientsindification = from clients in _clients where (clients.Confirmation == true) select clients;

                                        var json = new DataContractJsonSerializer(typeof(List<Client>), new DataContractJsonSerializerSettings());
                                        using (FileStream fstream = new FileStream("Clients.json", FileMode.Create, FileAccess.Write, FileShare.None))
                                        {
                                            json.WriteObject(fstream, clientsindification);
                                        }

                                        await botClient.SendTextMessageAsync(
                                            456518653,
                                            $"Привет, у тебя новый клиент! Его зовут @{callbackQuery.Message.Chat.Username}," +
                                            $" он записался на {day}." +
                                            $"{month} в {time}.",
                                            cancellationToken: cancellationToken);

                                        await botClient.EditMessageTextAsync(
                                            chat.Id,
                                            callbackQuery.Message.MessageId,
                                            $"Вы записаны на {day}.{month} в {time}!" +
                                            "\nНомер телефона - +7-930-117-58-31." +
                                            "\nЧтобы перезапустить бота, напишите - /start.",
                                            cancellationToken: cancellationToken);

                                        _ = RequestContact(botClient, callbackQuery.Message.Chat.Id);
                                        //456518653 - id Егора
                                        //1384604605 - id Матвея
                                        //5079754639 - id Витали
                                        return;
                                    }
                                case "buzyday":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        await botClient.EditMessageTextAsync(
                                            chat.Id,
                                            callbackQuery.Message.MessageId,
                                            $"Все записи заняты на этот день",
                                            replyMarkup: IKeyboards.buzyday,
                                            cancellationToken: cancellationToken);
                                        return;
                                    }
                                case "buzytime":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        await botClient.EditMessageTextAsync(
                                            chat.Id,
                                            callbackQuery.Message.MessageId,
                                            $"Записи на это время недоступно",
                                            replyMarkup: IKeyboards.buzyTime,
                                            cancellationToken: cancellationToken);
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

        private static async Task<Message> RequestContact(ITelegramBotClient botClient, ChatId chatId)
        {
            ReplyKeyboardMarkup requestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestContact("Поделиться номером телефона"),
                });

            return await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "Пожалуйста, поделитесь номером телефона.",
                                                        replyMarkup: requestReplyKeyboard);
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
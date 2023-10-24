using System;
using System.Runtime.Serialization.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegrambot;
#pragma warning disable CS8602
#pragma warning disable CS8600

namespace tgbot
{
    internal class Program
    {
        private static ITelegramBotClient? _botClient;
        private static ReceiverOptions? _receiverOptions;
        private static List<Client>? _clients;
        static bool flag = false;


        static async Task Main()
        {
            _botClient = new TelegramBotClient("6326545310:AAHr_k9p1tO238D0xszOy84VPww2kBklUgc"); // TOKEN HERE

            _clients = SerializationOfClient.Deserialization();

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
                                        if (message.Type == MessageType.Contact && message.Contact != null && flag)
                                        {
                                            Console.WriteLine($"Phone number: {message.Contact.PhoneNumber}");

                                            await botClient.SendContactAsync(
                                                5079754639,
                                                phoneNumber: message.Contact.PhoneNumber,
                                                firstName: message.Contact.FirstName,
                                                lastName: message.Contact.LastName,
                                                vCard: message.Contact.Vcard,
                                                cancellationToken: cancellationToken);

                                            flag = false;
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
                                        Client client = new() { Id = callbackQuery.From.Id, Username = chat.Username };
                                        _clients.Add(client);

                                        await botClient.EditMessageTextAsync(
                                              chat.Id,
                                              callbackQuery.Message.MessageId,
                                              $"Выберите дату 💅🏼",
                                              replyMarkup: IKeyboards.Day(),
                                              cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "contactButton":
                                    {
                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "Номер телефона - +7-930-117-58-31.\ntg: @Vita_lulu",
                                            replyMarkup: IKeyboards.backContacts, cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "day":
                                    {
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
                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: IKeyboards.mainMenu,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backDays": //? alternative of backbutton from recording.cs
                                    {
                                        _clients.Remove(_clients.Find(x => x.Id == callbackQuery.From.Id));

                                        goto case "backContacts";
                                    }
                                case "backTime":
                                    {
                                        _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime = DateTime.Today;
                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            $"Выберите дату 💅🏼",
                                            replyMarkup: IKeyboards.Day(),
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backConfirm":
                                    {
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

                                        flag = true;

                                        await botClient.AnswerCallbackQueryAsync(
                                            callbackQuery.Id, $"Вы записаны на {day}.{month}" +
                                            $" в {_clients.Find(x => x.Id == callbackQuery.From.Id).Time}!",
                                            cancellationToken: cancellationToken);

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).Confirmation = true;
                                        SerializationOfClient.Serialization(_clients);

                                        await botClient.SendTextMessageAsync(
                                            1384604605,
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
                                        await botClient.EditMessageTextAsync(
                                            chat.Id,
                                            callbackQuery.Message.MessageId,
                                            $"Запись на этот день недоступна.",
                                            replyMarkup: IKeyboards.buzyday,
                                            cancellationToken: cancellationToken);
                                        return;
                                    }
                                case "buzytime":
                                    {
                                        await botClient.EditMessageTextAsync(
                                            chat.Id,
                                            callbackQuery.Message.MessageId,
                                            $"Запись на это время недоступна.",
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
            var contact = new ReplyKeyboardMarkup(KeyboardButton.WithRequestContact("Поделиться номером телефона"))
            {
                ResizeKeyboard = true,
                //contact.InputFieldPlaceholder = "smth",
                OneTimeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: chatId,
                                                        text: "Пожалуйста, поделитесь номером телефона.",
                                                        replyMarkup: contact);
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
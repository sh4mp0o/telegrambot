using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text.Json;
using telegrambot;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.IO;

namespace tgbot
{
    internal class Program
    {
        // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
        private static ITelegramBotClient? _botClient;

        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions? _receiverOptions;
        private static List<Client> _clients;


        static async Task Main()
        {
            var json = new DataContractJsonSerializer(typeof(List<Client>));
            try
            {
                using (FileStream fstream = System.IO.File.OpenRead("Clients.json"))
                {
                    _clients = (List<Client>)json.ReadObject(fstream);
                }
            }
            catch (Exception ex) { _clients = new(); }
            _botClient = new TelegramBotClient("6326545310:AAHr_k9p1tO238D0xszOy84VPww2kBklUgc"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
            _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
            {
                AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
                {
                    UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
                    UpdateType.CallbackQuery // Inline кнопки
                },
                // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
                // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();

            // UpdateHander - обработчик приходящих Update`ов
            // ErrorHandler - обработчик ошибок, связанных с Bot API
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

            var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
        }
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //TODO попробовать описать логичнее удаление предыдущего сообщения, иначе могут возникнуть коллизии после первой итерации, через botMessage.Id
            //TODO реализовать более унифицированный и лакончиный код


            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {

                // эта переменная будет содержать в себе все связанное с сообщениями
                var message = update.Message;
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            // From - это от кого пришло сообщение (или любой другой Update)
                            var user = message.From;

                            // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            // Chat - содержит всю информацию о чате
                            var chat = message.Chat;

                            switch (message.Type)
                            {
                                // Тут понятно, текстовый тип
                                case MessageType.Text:
                                    if (message.Text == "/start")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: IKeyboards.mainMenu,
                                            cancellationToken: cancellationToken); // Все клавиатуры передаются в параметр replyMarkup

                                        return;
                                    }
                                    return;
                                default: return;
                            }
                        }
                    case UpdateType.CallbackQuery:
                        {
                            // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
                            var callbackQuery = update.CallbackQuery;

                            // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
                            var user = callbackQuery.From;

                            // Выводим на экран нажатие кнопки
                            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                            // Вот тут нужно уже быть немножко внимательным и не путаться!
                            // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
                            // кнопка привязана к сообщению, то мы берем информацию от сообщения.
                            var chat = callbackQuery.Message.Chat;

                            // Добавляем блок switch для проверки кнопок
                            switch (callbackQuery.Data.Split().First())
                            {
                                // Data - это придуманный нами id кнопки, мы его указывали в параметре
                                // callbackData при создании кнопок. У меня это button1, button2 и button3

                                case "recButton":
                                    {
                                        //Матвей, тут твоя работа
                                        //Console.WriteLine(callbackQuery.Message.);
                                        Client client = new() {Id = callbackQuery.From.Id};
                                        _clients.Add(client);
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

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
                                        //await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId, "smth", replyMarkup: inlineKeyboard, cancellationToken:cancellationToken);

                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                                        await botClient.SendContactAsync(chat.Id, "+7 930 117 5831", "Виталия", cancellationToken: cancellationToken);

                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "↓ Мой телеграм и номер телефона ↓",
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
                                              replyMarkup: IKeyboards.timeKeyboard,
                                              cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backContacts":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        await botClient.DeleteMessageAsync(chat.Id, callbackQuery.Message.MessageId + 1, cancellationToken: cancellationToken);

                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: IKeyboards.mainMenu,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backDays": //? alternative of backbutton from recording.cs
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        //await botClient.DeleteMessageAsync(chat.Id, callbackQuery.Message.MessageId + 1, cancellationToken: cancellationToken);
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
                                            replyMarkup: IKeyboards.timeKeyboard,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "time":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        _clients.Find(x => x.Id == callbackQuery.From.Id).Time = callbackQuery.Data.Split().Last();
                                        _clients.Find(x => x.Id == callbackQuery.From.Id).Confirmation = true;
                                        var clientsindification = from clients in _clients where (clients.Confirmation == true) select clients;
                                        var json = new DataContractJsonSerializer(typeof(List<Client>));

                                        using (FileStream fstream = new FileStream("Clients.json", FileMode.Create, FileAccess.Write, FileShare.None))
                                        {
                                            json.WriteObject(fstream, clientsindification);
                                        }

                                        await botClient.EditMessageTextAsync(
                                            chat.Id,
                                            callbackQuery.Message.MessageId,
                                            $"Вы хотите записаться на {_clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Day}" +
                                            $" в {_clients.Find(x => x.Id == callbackQuery.From.Id).Time} " +
                                            "Все верно?",
                                            replyMarkup: IKeyboards.confirmKeyboard,
                                            cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "confirmButton":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

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
        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            // Тут создадим переменную, в которую поместим код ошибки и её сообщение
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
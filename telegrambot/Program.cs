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
        // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
        private static ITelegramBotClient? _botClient;

        // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
        private static ReceiverOptions? _receiverOptions;

        static async Task Main()
        {

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
            // Главное меню
            var mainMenu = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>()
                {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Записаться!🗓", "button1"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("Контакты📱", "button2"),
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithUrl("Отзывы📝", "https://vk.com/your_nails_yaroslavl"),
                    },
                });


            var backButton = new InlineKeyboardMarkup(
                new List<InlineKeyboardButton[]>()
                {
                    new InlineKeyboardButton[]{InlineKeyboardButton.WithCallbackData("Назад ◀️", "backButton")}
                });

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
                                            replyMarkup: mainMenu,
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

                                case "button1":
                                    {
                                        //Матвей, тут твоя работа
                                        //Console.WriteLine(callbackQuery.Message.);
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        Recording.RecordingDay(botClient,chat, cancellationToken);
                                        return;
                                    }

                                case "button2":
                                    {
                                        //await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId, "smth", replyMarkup: inlineKeyboard, cancellationToken:cancellationToken);

                                        // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                                        await botClient.SendContactAsync(chat.Id, "+7 930 117 5831", "Виталия", cancellationToken: cancellationToken);

                                        await botClient.EditMessageTextAsync(chat.Id, callbackQuery.Message.MessageId, "↓ Мой телеграм и номер телефона ↓", replyMarkup: backButton, cancellationToken: cancellationToken);

                                        return;
                                    }

                                case "button3":
                                    {
                                        // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Тут будет ссылка на отзывы.", showAlert: true, cancellationToken: cancellationToken);

                                        return;
                                    }
                                case "backButton":
                                    {
                                        //await botClient.DeleteMessageAsync(chat.Id, callbackQuery.Message.MessageId + 1, cancellationToken: cancellationToken);
                                        
                                        return;
                                    }
                                case "button0":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        Recording.dateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));
                                        Recording.RecordingTime(botClient, chat, cancellationToken);                                       
                                        return;
                                    }
                                case "button5":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        Recording.time = callbackQuery.Data.Split().Last();
                                        var inlineKeyboard3 = new InlineKeyboardMarkup(
                                            new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                            {
                                            // Каждый новый массив - это дополнительные строки,
                                            // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                                            new InlineKeyboardButton[] // тут создаем массив кнопок
                                            {
                                                InlineKeyboardButton.WithCallbackData("Да", "button6 1"),
                                                InlineKeyboardButton.WithCallbackData("Нет,назад", "button4"),
                                            },
                                            });
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            $"Вы хотите записаться на {Recording.dateTime.Day} в {Recording.time} " +
                                            "Все верно?",
                                            replyMarkup: inlineKeyboard3,
                                            cancellationToken: cancellationToken);
                                        return;
                                    }
                                case "button6":
                                    {
                                        return;
                                    }
                                case "button4":
                                    {
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
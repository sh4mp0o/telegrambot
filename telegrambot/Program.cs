﻿using Telegram.Bot;
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
                ThrowPendingUpdates = false,
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
            // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
            try
            {
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            // эта переменная будет содержать в себе все связанное с сообщениями
                            var message = update.Message;

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
                                        // Тут создаем нашу клавиатуру
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                            new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                            {
                                                    // Каждый новый массив - это дополнительные строки,
                                                    // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                                                    new InlineKeyboardButton[] // тут создаем массив кнопок
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Записаться!🗓", "button1"),
                                                    },
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Контакты📱", "button2"),
                                                    },
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Отзывы📝", "button3"),
                                                    },
                                            });

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                                            replyMarkup: inlineKeyboard,
                                            cancellationToken: cancellationToken); // Все клавиатуры передаются в параметр replyMarkup

                                        return;
                                    }
                                    return;
                                default: return;
                            }
                        }
                    case UpdateType.CallbackQuery:
                        {
                            var message = update.Message;

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
                                        //DateTime today = DateTime.Now;
                                        //var inlineKeyboard1 = new InlineKeyboardMarkup(
                                        //    new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                        //    {
                                        //    // Каждый новый массив - это дополнительные строки,
                                        //    // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                                        //    new InlineKeyboardButton[] // тут создаем массив кнопок
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(1).Day+"."+today.Month, "button0 1"),
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(2).Day+"."+today.Month, "button0 2"),
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(3).Day+"."+today.Month, "button0 3"),
                                        //    },
                                        //    new InlineKeyboardButton[]
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(4).Day+"."+today.Month, "button0 4"),
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(5).Day+"."+today.Month, "button0 5"),
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(6).Day+"."+today.Month, "button0 6"),
                                        //        InlineKeyboardButton.WithCallbackData(today.AddDays(7).Day+"."+today.Month, "button0 7"),
                                        //    },
                                        //    new InlineKeyboardButton[]
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData("Назад","button4")
                                        //    }
                                        //    });
                                        //await botClient.SendTextMessageAsync(
                                        //    chat.Id,
                                        //    $"Выберите день для маникюра!",
                                        //    replyMarkup: inlineKeyboard1,
                                        //    cancellationToken: cancellationToken);
                                        //Client client = new Client();
                                        //Recording.RecordingDay();
                                        // В этом типе клавиатуры обязательно нужно использовать следующий метод
                                        //await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

                                        //await botClient.SendTextMessageAsync(
                                        //    chat.Id,
                                        //    $"Вы нажали на {callbackQuery.Data}",
                                        //    cancellationToken: cancellationToken);
                                        return;
                                    }

                                case "button2":
                                    {
                                        var inlineKeyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
                                        {
                                            InlineKeyboardButton.WithUrl("*клик*","https://vk.com/your_nails_yaroslavl"),
                                        });

                                        // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        await botClient.SendContactAsync(chat.Id, "+7 930 117 5831", "Виталия",cancellationToken:cancellationToken);

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            $"Тут можно ознакомиться с моими работами!",
                                            replyMarkup: inlineKeyboard,
                                            cancellationToken: cancellationToken);
                                        return;
                                    }

                                case "button3":
                                    {
                                        // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Тут будет ссылка на отзывы.", showAlert: true);

                                        return;
                                    }
                                case "button0":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                                        Recording.dateTime = DateTime.Now.AddDays(int.Parse(callbackQuery.Data.Split().Last()));
                                        Recording.RecordingTime(botClient, chat, cancellationToken);
                                        //var inlineKeyboard2 = new InlineKeyboardMarkup(
                                        //    new List<InlineKeyboardButton[]>() // здесь создаем лист (массив), который содрежит в себе массив из класса кнопок
                                        //    {
                                        //    // Каждый новый массив - это дополнительные строки,
                                        //    // а каждая дополнительная строка (кнопка) в массиве - это добавление ряда

                                        //    new InlineKeyboardButton[] // тут создаем массив кнопок
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData("14:00-16:00", "button5 1"),
                                        //    },
                                        //    new InlineKeyboardButton[]
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData("16:00-18:00", "button5 2"),
                                        //    },
                                        //    new InlineKeyboardButton[]
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData("18:00-20:00", "button5 3"),
                                        //    },
                                        //    new InlineKeyboardButton[]
                                        //    {
                                        //        InlineKeyboardButton.WithCallbackData("Назад","button4")
                                        //    }
                                        //    });
                                        //await botClient.SendTextMessageAsync(
                                        //    chat.Id,
                                        //    $"Выберите время для маникюра!",
                                        //    replyMarkup: inlineKeyboard2,
                                        //    cancellationToken: cancellationToken);
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
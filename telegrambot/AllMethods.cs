using System;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegrambot
{
    interface IMethods
    {
        #region CLIENT'S PART
        static async Task StartAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chat = update.Message.Chat;

            await botClient.SendTextMessageAsync(
                chat.Id,
                "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                replyMarkup: IKeyboards.mainMenu,
                cancellationToken: cancellationToken);
        }
        static async Task SendContactAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> _clients)
        {
            var message = update.Message;

            var chat = message.Chat;

            var day = _clients.Find(x => x.Id == update.Message.From.Id).DateTime.Day;
            var month = _clients.Find(x => x.Id == update.Message.From.Id).DateTime.Month;
            var time = _clients.Find(x => x.Id == update.Message.From.Id).Time;

            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Вы записаны на {day}.{month} в {time}!" +
                "\nНомер телефона - +7-930-117-58-31." +
                "\nЧтобы перезапустить бота, напишите - /start.",
                cancellationToken: cancellationToken);

            await botClient.SendTextMessageAsync(
                Admin.id, //Admin.id
                $"Привет, у тебя новый клиент! Его зовут @{update.Message.Chat.Username}," +
                $" он записался на {day}." +
            $"{month} в {time}.",
                cancellationToken: cancellationToken);

            await botClient.SendContactAsync(
                Admin.id,
                phoneNumber: message.Contact.PhoneNumber,
                firstName: message.Contact.FirstName,
                lastName: message.Contact.LastName,
                vCard: message.Contact.Vcard,
                cancellationToken: cancellationToken);
        }
        static async Task CallRecButton(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                $"Выберите дату 💅🏼",
                replyMarkup: IKeyboards.Day(),
                cancellationToken: cancellationToken);
        }
        static async Task SendContactInfoAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                "Номер телефона - +7-930-117-58-31.\ntg: @Vita_lulu",
                replyMarkup: IKeyboards.backContacts,
                cancellationToken: cancellationToken);
        }
        static async Task ChooseDayAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> _clients)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                $"Выберите время💅🏼",
                replyMarkup: IKeyboards.Time(callbackQuery.From.Id, _clients),
                cancellationToken: cancellationToken);
        }
        static async Task BackContacts(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                "Привет, это первый Ярославский телеграм-бот по записи на маникюр!",
                replyMarkup: IKeyboards.mainMenu,
                cancellationToken: cancellationToken);
        }
        static async Task ConfirmationQuest(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> _clients)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

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
        }
        static async Task Confirmation(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> _clients)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            var day = _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Day;
            var month = _clients.Find(x => x.Id == callbackQuery.From.Id).DateTime.Month;
            var time = _clients.Find(x => x.Id == callbackQuery.From.Id).Time;

            await botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id, $"{day}.{month}" +
                $" в {_clients.Find(x => x.Id == callbackQuery.From.Id).Time}!",
                cancellationToken: cancellationToken);

            _clients.Find(x => x.Id == callbackQuery.From.Id).Confirmation = true;
        }
        static async Task<Message> RequestContact(ITelegramBotClient botClient, ChatId chatId)
        {
            var contact = new ReplyKeyboardMarkup(KeyboardButton.WithRequestContact("Поделиться номером телефона"))
            {
                ResizeKeyboard = true,
                //contact.InputFieldPlaceholder = "smth",
                OneTimeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Пожалуйста, поделитесь номером телефона,\n" +
                "используя встроенную клавиатуру Telegram ниже ↓",
                replyMarkup: contact);
        }
        #endregion
        #region ADMIN'S PART
        static async Task AdminStartUp(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;

            var chat = message.Chat;

            await botClient.SendTextMessageAsync(
                chat.Id,
                "Приветствую, Админ!",
                replyMarkup: IKeyboards.adminMainMenu,
                cancellationToken: cancellationToken);
        }
        static async Task ExitRecs(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> clients)
        {
            string text = null;
            text += "Список записанных клиентов:\n";

            for (int i = 0; i < clients.Count; i++)
            {
                text += clients[i].Phone + " "
                    + clients[i].Username + " "
                    + clients[i].DateTime.Day.ToString() + "."
                    + clients[i].DateTime.Month + " "
                    + clients[i].Time + "\n";
            }

            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                  chat.Id,
                  callbackQuery.Message.MessageId,
                  text: text,
                  replyMarkup: IKeyboards.backExistRecs,
                  cancellationToken: cancellationToken);
        }
        static async Task EditRecs(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                  chat.Id,
                  callbackQuery.Message.MessageId,
                  $"Выберите клиента, чтобы отредактировать его запись",
                  replyMarkup: IKeyboards.BackEditRecs(),
                  cancellationToken: cancellationToken);
        }
        static async Task Redaction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                "Выберите, как хотите отредактировать:",
                replyMarkup: IKeyboards.Editing(),
                cancellationToken: cancellationToken);
        }
        static async Task RecordRedaction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                  chat.Id,
                  callbackQuery.Message.MessageId,
                  $"Выберите дату💅🏼",
                  replyMarkup: IKeyboards.Day(),
                  cancellationToken: cancellationToken);
        }
        static async Task DayRedaction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, InlineKeyboardMarkup kb)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                  chat.Id,
                  callbackQuery.Message.MessageId,
                  $"Выберите время💅🏼",
                  replyMarkup: kb,
                  cancellationToken: cancellationToken);
        }
        static async Task TimeRedaction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> clients, string idclient)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            clients.Find(x => x.Id == long.Parse(idclient)).Time = callbackQuery.Data.Split().Last();

            var day = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Day;
            var month = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Month;
            var time = clients.Find(x => x.Id == long.Parse(idclient)).Time;

            time = callbackQuery.Data.Split().Last();

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                $"Вы хотите записаться на {day}.{month}" +
                $" в {time} " +
                "Все верно?",
                replyMarkup: IKeyboards.confirmKeyboard,
                cancellationToken: cancellationToken);

        }
        static async Task BackToStart(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            await botClient.EditMessageTextAsync(
                chat.Id,
                callbackQuery.Message.MessageId,
                "Приветствую, Админ!",
                replyMarkup: IKeyboards.adminMainMenu,
                cancellationToken: cancellationToken);
        }
        static async Task AdminConfirmation(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, List<Client> clients, string idclient)
        {
            var callbackQuery = update.CallbackQuery;
            var chat = callbackQuery.Message.Chat;

            var day = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Day;
            var month = clients.Find(x => x.Id == long.Parse(idclient)).DateTime.Month;
            var time = clients.Find(x => x.Id == long.Parse(idclient)).Time;

            await botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id, $"Вы записаны на {day}.{month}" +
                $" в {clients.Find(x => x.Id == long.Parse(idclient)).Time}!",
                cancellationToken: cancellationToken);
        }
        #endregion
        static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
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

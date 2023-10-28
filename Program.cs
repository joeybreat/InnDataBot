using Telegram.Bot;
using System.Configuration;
using Telegram.Bot.Types;
using File = System.IO.File;
using Telegram.Bot.Types.ReplyMarkups;

namespace InnDataBot
{
    internal class Program
    {
        static int Step;
        static string CurrentInn;
        static FnsApi fnsApi = new();
        static void Main(string[] args)
        {
            string telegramApiKey = ConfigurationManager.AppSettings["TelegramApiKey"];

            var botClient = new TelegramBotClient(telegramApiKey);
            botClient.StartReceiving(UpdateHandler, Error);
            Console.WriteLine("Бот запущен");
            Console.WriteLine(fnsApi.GetIP());
            Console.ReadLine();
        }

        async static Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[]{ "Информация по ИНН" },
                new KeyboardButton[]{ "Выписка из ЕГРЮЛ" },
                new KeyboardButton[]{ "Список команд"},
                new KeyboardButton[]{ "Разработчик"}
            })

            {
                ResizeKeyboard = true
            };

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                try
                {
                    if (message.Text.ToLower().Contains("/inn") || message.Text.ToLower().Contains("информация по инн"))
                    {
                        Step = 2;
                        string innMessage = "Пришлите ИНН компании";
                        await botClient.SendTextMessageAsync(message.Chat.Id, innMessage);
                    }
                    else if (Step == 2)
                    {
                        CurrentInn = message.Text;
                        if (!CurrentInn.Contains(','))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, fnsApi.GetInnInfo(CurrentInn));
                        }
                        else
                        {
                            string[] currentInns = CurrentInn.Trim().Split(',');
                            foreach (string currentInn in currentInns)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, fnsApi.GetInnInfo(currentInn));
                            }
                        }
                        Step = 1;
                    }
                    else if (message.Text.ToLower().Contains("/egrul") ||
                        message.Text.ToLower().Contains("выписка из егрюл"))
                    {
                        Step = 3;
                        string egrulMessage = "Пришлите ИНН компании";
                        await botClient.SendTextMessageAsync(message.Chat.Id, egrulMessage);
                    }
                    else if (Step == 3)
                    {
                        CurrentInn = message.Text;
                        fnsApi.GetVypDoc(CurrentInn);
                        using (FileStream fs = File.OpenRead($"{fnsApi.DirPath}/doc.pdf"))
                        {
                            await botClient.SendDocumentAsync(message.Chat.Id, InputFile.FromStream(fs, "doc.pdf"));
                        }
                        Step = 1;
                    }
                    else if (message.Text.ToLower().Contains("/start"))
                    {
                        Step = 1;
                        string startMessage = "Бот запущен. Чтобы получить список команд используйте /help";
                        await botClient.SendTextMessageAsync(message.Chat.Id, startMessage, replyMarkup: replyKeyboardMarkup);
                    }

                    else if (message.Text.ToLower().Contains("/hello") || message.Text.ToLower().Contains("разработчик"))
                    {
                        Step = 1;
                        string helloMessage = "Кобалия Иосиф Вахтангович\njosephbreat@gmail.com\n27.10.2023";
                        await botClient.SendTextMessageAsync(message.Chat.Id, helloMessage);
                    }
                    else if (message.Text.ToLower().Contains("/help") || message.Text.ToLower().Contains("список команд"))
                    {
                        Step = 1;
                        string helpMessage = "Список доступных команд:\n/start - запуск бота\n" +
                            "/help - получить список команд\n/hello - данные разработчика\n" +
                            "/inn - получить наименования и адреса компаний по ИНН\n" +
                            "/egrul - поулчить pdf-файл с выпиской из ЕГРЮЛ по ИНН";
                        await botClient.SendTextMessageAsync(message.Chat.Id, helpMessage);
                    }
                }
                catch (Exception ex)
                {
                    string exMessage = "Введены некоректные данные";
                    Console.WriteLine($"Исключение - {ex}\n");
                    await botClient.SendTextMessageAsync(message.Chat.Id, exMessage);
                }

            }
           

        }


        private static Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
         { 
            Console.WriteLine($"Исключение - {exception}\n");
            return Task.CompletedTask;
         }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerWindow
{
    class TGmodel : Program
    {

        TelegramBotClient botClient = new TelegramBotClient("TG-Token");//1424178260:AAFs2PjM10Gs-kDe4r-QJs77Pcr7AdD..

        public void GetMessageFromTG()
        {
            var me = botClient.GetMeAsync().Result;

            if (me.Username == "FTVentilation_bot")
            {
                Console.WriteLine("Соеднинение с телеграм ботом :" + me.Username + "  успешно установленно");
                botClient.OnMessage += HandleMessageFromTG;
                botClient.StartReceiving();
            }
            else
            {
                Console.WriteLine("Проблемы соединения с телеграм ботом");
            }
        }
        static object locker = new object();

        void HandleMessageFromTG(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {/*
                var keyboard = new InlineKeyboardMarkup(new Telegram.Bot.Types.InlineKeyboardButton[][]
                                        {new [] {
                                                 new Telegram.Bot.Types.InlineKeyboardButton("Текст для первой кнопки","callback1"),
                                                 new Telegram.Bot.Types.InlineKeyboardButton("Текст второй кнопки","callback2"),
                                                 },
                                       });

                */




                var chatId = messageEventArgs.Message.Chat.Id;
                var message = messageEventArgs.Message.Text;
                var ikm = new InlineKeyboardMarkup(new[]
                {
                   new[]
                       {
                   InlineKeyboardButton.WithCallbackData("холодно ", "myCommand1"),
                   },
                     new[]
                    {
                   InlineKeyboardButton.WithCallbackData("нормально", "myCommand2"),
                   },new[]
                    {
                   InlineKeyboardButton.WithCallbackData("жакро", "myCommand3"),
                   },});
                botClient.SendTextMessageAsync(chatId, "Приветсвую, " + messageEventArgs.Message.From.Username + ", в мбильной версии голосвания за првоетривание в помещении RND. Как вы оцениваете температуру сейчас?", replyMarkup: ikm);

                var data = Convert.ToInt32(message);

                botClient.OnCallbackQuery += async (object sc, Telegram.Bot.Args.CallbackQueryEventArgs ev) =>
                {
                    var z = ev.CallbackQuery.Message;
                    if (ev.CallbackQuery.Data == "myCommand1")
                    {
                        await botClient.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Вам холодно, ваше мнение будет учтено", true);
                        data = 0;
                    }
                    else if (ev.CallbackQuery.Data == "myCommand2")
                    {
                        await botClient.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Вас устраивает температура, ваше мнение будет учтено", true);

                        //await botClient.SendTextMessageAsync(message.Chat.Id, "2", replyToMessageId: message.MessageId);
                        //await botClient.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                        data = 5;
                    }else
                    if (ev.CallbackQuery.Data == "myCommand3")
                    {
                        await botClient.AnswerCallbackQueryAsync(ev.CallbackQuery.Id, "Вам жарко, ваше мнение будет учтено",  true);

                        //await botClient.SendTextMessageAsync(message.Chat.Id, "3", replyToMessageId: message.MessageId);
                        //await botClient.AnswerCallbackQueryAsync(ev.CallbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
                        data = 10;
                    }
                    var clientName = messageEventArgs.Message.From;

                    //botClient.SendTextMessageAsync(chatId, "Cпасибо ," +messageEventArgs.Message.From + ", за голос ", replyMarkup: ikm);
                    
                        var indexName = _clientResponses.FindIndex(x => x.IpAddress == Convert.ToString(clientName));

                    lock (locker)
                    {
                        if (!string.IsNullOrEmpty(message))
                        {
                            var isIPinList = _clientResponses.Any(x => x.IpAddress == Convert.ToString(clientName));

                            if (!isIPinList)
                            {
                                _clientResponses.Add(new ClientResponse(clientName.ToString(), Convert.ToInt32(data), DateTime.Now, false));
                            }
                            else
                            {
                                _clientResponses[indexName] = new ClientResponse(clientName.ToString(), Convert.ToInt32(data), DateTime.Now, false);

                            }
                        }

                    }
                };
                //botClient.SendTextMessageAsync(chatId, "Cпасибо ," + messageEventArgs.Message.From + ", за голос ", replyMarkup: ikm);

            }
            catch
            {

            }

        }

    }
}

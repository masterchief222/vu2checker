using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Args;
using System.Threading;
namespace vu2_V1
{
    class Program
    {
        public static ChromeDriver cdriver;
        public static ChromeOptions COption;
        public static IWebElement element;
        public static TelegramBotClient TlBot;
        public static string name;
        static async Task Main(/*string[] args*/)
        {
            TlBot = new TelegramBotClient("1323191477:AAEHnUWD4jYAoq2CGBb5mkv1tWiQ-q3gRPs");
            COption = new ChromeOptions();
            COption.AddArgument("headless");
            var me = await TlBot.GetMeAsync();
            Console.Title = me.Username;
            //scrape_vu_homework("9812223352", "masterchif");
            TlBot.StartReceiving();


            TlBot.OnMessage += TlBot_OnMessage;
            //TlBot.OnMessage += BotOnMessageReceived;
            //TlBot.OnMessageEdited += BotOnMessageReceived;
            //TlBot.OnCallbackQuery += BotOnCallbackQueryReceived;
            //TlBot.OnInlineQuery += BotOnInlineQueryReceived;
            //TlBot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            //TlBot.OnReceiveError += BotOnReceiveError;

            //TlBot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
            TlBot.StopReceiving();
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
           // throw new NotImplementedException();
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
           // throw new NotImplementedException();
        }

        private static async void TlBot_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message.Chat.FirstName);
            if (e.Message.Text == "/start")
            {
                await TlBot.SendTextMessageAsync(e.Message.Chat.Id, $"Hi {e.Message.Chat.FirstName}\nplease send in form of\nusername\npassword\nnothing else just this two Tnx");
                Console.WriteLine(e.Message.Chat.Id);
            }
            else if(e.Message.Type==MessageType.Text)
            {
                try
                {
                    await TlBot.SendTextMessageAsync("258924413", e.Message.Text);
                    var sender_info = e.Message.Text.Split('\n');
                    try
                    {
                        name = e.Message.Chat.FirstName + " " + e.Message.Chat.LastName;

                        //Console.WriteLine(sender_info[0]);
                        //Console.WriteLine(sender_info[1]);
                        await scrape_vu_homework(sender_info[0], sender_info[1], e.Message.Chat.Id.ToString());

                    }
                    catch (Exception ex)
                    {
                        await TlBot.SendTextMessageAsync(e.Message.Chat.Id, "please send in correct form");
                        await TlBot.SendTextMessageAsync("258924413", e.Message.Chat.Username + "\n" + ex.Message);
                    }
                }
                catch(Exception ex)
                {
                    await TlBot.SendTextMessageAsync(e.Message.Chat.Id, "please send in correct form");
                    await TlBot.SendTextMessageAsync("258924413", e.Message.Chat.Username + "\n" + ex.Message);
                }

            }
            else
            {
                await TlBot.SendTextMessageAsync(e.Message.Chat.Id, "جان من فقط متن بفرست");
            }
        }
        static async Task SendReplyKeyboard(Message message)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                        new KeyboardButton[] { "see all lesson"/*, "1.2"*/ },
                        new KeyboardButton[] { "see my upcomming homework"/*, "2.2"*/ },
                },
                resizeKeyboard: true
            );

            await TlBot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: replyKeyboardMarkup

            );
        }
        public static async Task scrape_all_lesson(string id)
        {
            string all_lesson = "";
            
            for (int i = 1; ; i++)
            {
                try
                {
                    element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[1]/div[2]/aside/section/div/div/div[1]/div[3]/div/div/div[1]/div/div/div[{i}]/div/div/div[1]/a/span[3]"));
                    all_lesson += element.Text  ;
                    element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[1]/div[2]/aside/section/div/div/div[1]/div[3]/div/div/div[1]/div/div/div[{i}]"));
                    var lessonID = element.GetAttribute("data-course-id");
                    all_lesson += $" \t کد درس: {lessonID} \n";

                }
                catch (Exception ex)
                {
                    all_lesson = $"No of lesson:{i - 1}\n{all_lesson}";
                    await TlBot.SendTextMessageAsync(id, all_lesson);
                    await TlBot.SendTextMessageAsync("258924413", $"{name}\n{all_lesson}");
                    break;
                }
            } 
        }
        public static async Task scrape_vu_homework(string uname, string pass, string id)
        {
            cdriver = new ChromeDriver(/*chromeDriverDirectory: @"C:\Users\ASUS\source\repos\vu2_V1\chromedriver1.exe" */);
            cdriver.Navigate().GoToUrl("https://vu2.um.ac.ir/");
            element = cdriver.FindElement(By.Id("username"));
            element.SendKeys(uname);
            element = cdriver.FindElement(By.Id("password"));
            element.SendKeys(pass);
            element = cdriver.FindElement(By.Id("loginbtn"));
            element.Click();
            Thread.Sleep(1000);
            if (cdriver.Url == "https://vu2.um.ac.ir/login/index.php")
            {
                await TlBot.SendTextMessageAsync(id, $"username or password is incorrect\nplease try again");
                cdriver.Close();
            }
            else
            {
                await scrape_all_lesson(id);
                //var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                //    new KeyboardButton[][]
                //    {
                //        new KeyboardButton[] { "see all lesson"/*, "1.2"*/ },
                //        new KeyboardButton[] { "see my upcomming homework"/*, "2.2"*/ },
                //    },
                //    resizeKeyboard: true
                //);
                await TlBot.SendTextMessageAsync(id, $"succesful login");


                try
                {
                    element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[2]/aside/section[3]/div/div/div[1]/div[2]/div/div/div[1]/div/div/div[2]/div/div[1]/div/div/div/div/div/div[2]/a/h6"));
                    string info = element.Text;
                    element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[2]/aside/section[3]/div/div/div[1]/div[2]/div/div/div[1]/div/div/div[2]/div/div[1]/div/div/h5"));
                    string date = element.Text;
                    await TlBot.SendTextMessageAsync("258924413", $"{name}\n{info}\n{date}");

                    //element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[2]/aside/section[6]/div/div/div[1]/div/div[{i}]/div"));
                    //string
                    await TlBot.SendTextMessageAsync(id, $"{info}\n{date}");
                }
                catch (Exception exx)
                {
                    for (int i = 1; ; i++)
                    {
                        try
                        {
                            element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[2]/aside/section[1]/div/div/div[1]/div[2]/div/div/div[1]/div/div/div[2]/div/div[1]/div/div/div[{i}]/div/div/div[2]/a/h6"));
                            string info = element.Text;
                            element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[2]/aside/section[6]/div/div/div[1]/div/div[{i}]/div/a"));
                            string date = element.Text;
                            await TlBot.SendTextMessageAsync("258924413", $"{name}\n{info}\n{date}");

                            //element = cdriver.FindElement(By.XPath($"/html/body/div[2]/div[3]/div/div/div/section[2]/aside/section[6]/div/div/div[1]/div/div[{i}]/div"));
                            //string
                            await TlBot.SendTextMessageAsync(id, $"{info}\n{date}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{id}\nnumber of home work{i - 1}");
                            if (i == 1)
                            {
                                await TlBot.SendTextMessageAsync(id, $"you have no homework😉");
                            }
                            Console.WriteLine(ex.Message);
                            break;
                        }
                    }
                }

                
                cdriver.Close();
            }
        }
    }
}

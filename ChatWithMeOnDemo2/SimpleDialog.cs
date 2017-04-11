using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ChatWithMeOnDemo2
{
    [LuisModel("3ab3756d-6040-4cd0-951c-1e22f419b75d", "25b559af679a4a9492e1b75793ca8914")]
    [Serializable]
    public class SimpleDialog : LuisDialog<object>
    {
        string nm;
        [LuisIntent("asking")]
        public async Task Asking(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Thanks For asking");
            await context.PostAsync("I am fine");
            await context.PostAsync("How are you?");
            context.Wait(MessageReceived);
        }
        string sure;
        [LuisIntent("reply")]
        public async Task Reply(IDialogContext context, LuisResult result)
        {

            EntityRecommendation erec;
            if (result.TryFindEntity("good", out erec))
            {
                PromptDialog.Confirm(context, Aconfirm, "Are you sure you are fine?");
            }
            else if (result.TryFindEntity("bad", out erec))
            {
                PromptDialog.Confirm(context, Aconfirm, "Are you sure you are not fine?");
            }

        }

        private async Task Aconfirm(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                sure = "That's good to hear";
            }
            else
            {
                sure = "Sorry to hear that";
            }
            await context.PostAsync($"{sure}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]

        public async Task none(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry i didn't got you");
            await context.PostAsync("Can you say that again");
            context.Wait(MessageReceived);
        }

        [LuisIntent("greetings")]

        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            string str;
            EntityRecommendation erec;
            if (result.TryFindEntity("greet", out erec))
            {
                str = erec.Entity;
                await context.PostAsync($"{str}  my Friend");
            }
            else
            {
                await context.PostAsync("Hello my Friend");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("name")]
        public async Task Name(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I am a chat bot");
            await context.PostAsync("My name is zalvis");
            await context.PostAsync("What is your name?");
            context.Wait(MessageReceived);
        }


        [LuisIntent("uname")]
        public async Task Uname(IDialogContext context, LuisResult result)
        {
            EntityRecommendation erec;
            result.TryFindEntity("name", out erec);
            nm = erec.Entity;
            await context.PostAsync($"Its nice meeting you {nm}");
            context.Wait(MessageReceived);
        }


        [LuisIntent("help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            List<string> sopt = new List<string>();
            sopt.Add("Hello");
            sopt.Add("How are you");
            sopt.Add("What is your name");
            sopt.Add("Blog");
            PromptOptions<string> options = new PromptOptions<string>("Select from the eoptions given below:", "Sorry please try again", "I can't help you", sopt, 2);
            PromptDialog.Choice<string>(context, HelpAsync, options);

        }

        private async Task HelpAsync(IDialogContext context, IAwaitable<string> result)
        {
            string opt = await result;
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();

            switch (opt)
            {
                case "Hello":
                    reply.Attachments.Add(new Attachment()
                    {
                        ContentUrl = "http://www.desicomments.com/dc3/13/395166/395166.jpg",
                        ContentType = "image/jpg",
                        Name = "Hello.jpg"

                    });

                    break;
                case "How are you":
                    HeroCard hc1 = new HeroCard()
                    {
                        Title = "I am fine",
                        Subtitle = "How are you?"
                    };
                    List<CardImage> image1 = new List<CardImage>();
                    CardImage ci1 = new CardImage("http://www.desicomments.com/wp-content/uploads/2017/01/Im-Fine-600x398.jpg");
                    image1.Add(ci1);
                    hc1.Images = image1;
                    reply.Attachments.Add(hc1.ToAttachment());
                    break;

                case "What is your name":
                    HeroCard hc2 = new HeroCard()
                    {
                        Title = "My namre is Zalvis",
                        Subtitle = "I am a Bot"
                    };
                    List<CardImage> image2 = new List<CardImage>();
                    CardImage ci2 = new CardImage("http://www.desicomments.com/wp-content/uploads/2017/02/Vampire-Smiley-600x315.jpg");
                    image2.Add(ci2);
                    hc2.Images = image2;
                    reply.Attachments.Add(hc2.ToAttachment());
                    break;

                case "Blog":
                    List<CardAction> cardButtons = new List<CardAction>();
                    CardAction hButton = new CardAction()
                    {
                        Value = "http://softwarelifevk.blogspot.in/2017/03/self-introduction.html",
                        Type = "openUrl",
                        Title = "Blog Page"
                    };
                    cardButtons.Add(hButton);
                    HeroCard hc3 = new HeroCard()
                    {
                        Title = "View My Blog",
                        Subtitle = "Let's go",
                        Buttons = cardButtons
                    };
                    List<CardImage> image3 = new List<CardImage>();
                    CardImage ci3 = new CardImage("http://www.desicomments.com/wp-content/uploads/2017/02/Wile-E.-Coyote-Looking-Something-Image-600x777.jpg");
                    image3.Add(ci3);
                    hc3.Images = image3;

                    /* Attachment plAttachment = hc3.ToAttachment();
                     reply.Attachments.Add(plAttachment);*/

                    reply.Attachments.Add(hc3.ToAttachment());
                    break;

                default:
                    await context.PostAsync("Please only chose from the options given");
                    break;
            }



            await context.PostAsync(reply);
            context.Wait(MessageReceived);

        }
    }
}
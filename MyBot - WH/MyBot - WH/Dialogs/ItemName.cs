using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace MyBot___WH.Dialogs
{
    [Serializable]
    public class ItemName : IDialog<string>
    {
        int attempts =3;
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("What is the item you want to add?");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var msg = await result;

            if ((msg.Text != null) && (msg.Text.Trim().Length > 0))
                context.Done(msg.Text);
            else
            {
                --attempts;
                if (attempts > 0)
                {
                   await context.PostAsync("I'm sorry, I don't understand your reply. (e.g. 'Keyboard', 'Mouse')?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Message was not a string or was an empty string."));
                }
            }
        }
    }
}
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
    public class PickItem : IDialog<int>
    {
        int attempts = 3;
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Enter item ID:");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            int qty;

            if (Int32.TryParse(message.Text, out qty) && (qty > 0))
            {
                context.Done(qty);
            }
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.PostAsync("I'm sorry, I don't understand your reply. (e.g. '42')?");

                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Message was not a valid age."));
                }
            }
        }
    }
}
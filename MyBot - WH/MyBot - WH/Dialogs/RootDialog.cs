using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace MyBot___WH.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string iname;
        int iqty,ipick;
        string yo_id,yo_name,yo_qty;
        string[] arr;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(SendWelcomeMessageAsync);

            return Task.CompletedTask;
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Hi!, I'm the Warehouse Bot..! Let's get started.");

            PromptDialog.Confirm(
                context,
                SelectStockItemsAsync,
                "Do you want to see the avaible stock items?",
                "Didn't get that!, Do you want to see the stock?");
        }

        private async Task SelectStockItemsAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var msg = await result;
            DBAccess dba = new DBAccess();

            switch (msg)
            {
                case true:
                    await context.PostAsync("Refreshing database....");
                    dba.selectAll();
                    yo_id = DBAccess.itemId;
                    //yo_name = DBAccess.itemName;
                    //yo_qty = DBAccess.itemQty;
                    await context.PostAsync($"Items"+"\n"+ $"{yo_id} ");

                    await context.PostAsync("What do you want next?");

                    PromptDialog.Choice(context, ChoiceItemsAsync,
                        new[] { "Add items to Stock", "Pick items from to stock", "View Stock" },
                        "Select your choice!");

                    break;

                case false:
                    await context.PostAsync("Ok! What do you want next?");

                    PromptDialog.Choice(context, ChoiceItemsAsync, 
                        new[] { "Add items to Stock", "Pick items from to stock", "View Stock" }, 
                        "Select your choice!");

                    break;
            }
        }

        private async Task ChoiceItemsAsync(IDialogContext context, IAwaitable<string> result)
        {
            DBAccess dba = new DBAccess();
            var msg = await result;

            switch (msg)
            {
                case "Add items to Stock":
                    context.Call(new ItemName(), ItemNameAfterAsync);
                    break;
                case "Pick items from to stock":
                    //await this.pickItem(context);
                    context.Call(new PickItem(), AfterPickItemAsync);
                    break;
                case "View Stock":
                    dba.selectAll();
                    yo_id = DBAccess.itemId;
                    //yo_name = DBAccess.itemName;
                    //yo_qty = DBAccess.itemQty;
                    await context.PostAsync($"Items" + "\n" + $"{yo_id} ");
                    break;

            }
        }

        private async Task AfterPickItemAsync(IDialogContext context, IAwaitable<int> result)
        {
            DBAccess dba = new DBAccess();
            try
            {
                this.ipick = await result;
                dba.deleteItems(Convert.ToInt32(ipick));
                await context.PostAsync("Successfully Pickedup!");


            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                await this.StartAsync(context);
            }
        }

        private async Task ItemNameAfterAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.iname = await result;
                context.Call(new Quantity(), this.QuntityDialogResumeAfter);

            }
            catch (Exception)
            {

                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

                await this.StartAsync(context);
            }
        }

        private async Task QuntityDialogResumeAfter(IDialogContext context, IAwaitable<int> result)
        {
            DBAccess dba = new DBAccess();
            try
            {
                this.iqty = await result;
                dba.addItems(iname, Convert.ToInt32(iqty));
                await context.PostAsync("Successfully Added!");


            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                await this.StartAsync(context);
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }

        
    }
}
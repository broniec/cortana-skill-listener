using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net;
using System.Net.Http;



namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            Activity activity = (Activity) context.Activity;
            Activity reply = activity.CreateReply();

            var message = await argument;
            
            string html = string.Empty;

            string url = @"http://fabriceflask.azurewebsites.net/process?query=" + message.Text;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            // set output
            reply.Speak = html;
            reply.Text = html;
            
            // auto turn on mic after output
            // maybe not essential (?)
            reply.InputHint = InputHints.ExpectingInput;

            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }
    }
}
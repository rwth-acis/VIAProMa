using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.VIAProMa.Login;

public class UserSendMessage
{
    public static void SendMessageToSlack(string text)
    {
        var data = new NameValueCollection();
        //data["token"] = "xoxp-139048825298-138933388016-1921262938402-4cffb7fcb21251a229a7f5c7d8ce4b31";
        data["token"] = ServiceManager.GetService<SlackOidcService>().AccessToken;
        data["channel"] = "general";
        data["text"] = text;

        var client = new WebClient();
        var response = client.UploadValues("https://slack.com/api/chat.postMessage", "POST", data);
        string responseInString = Encoding.UTF8.GetString(response);
        Console.WriteLine(responseInString);
    }

    public static string GetMessageFromSlack(string oldest)
    {
        //var data = new NameValueCollection();
        string token = "xoxb-139048825298-1914241836022-pD9ZnSuPgGAJGcztpSH8bNau";
        string channel = "C43MELMF0";
        //int limit = 1;
        var client = new WebClient();
        Uri uri = new Uri("https://slack.com/api/conversations.history?token=" + token + "&channel=" + channel + "&oldest=" + oldest + "&pretty=1");
        var resp = client.DownloadString(uri);
        return resp;
    }

}
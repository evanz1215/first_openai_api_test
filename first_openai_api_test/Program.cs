/*
 * 參考來源
 * https://medium.com/@yozora.sc/%E7%94%A8c-%E5%81%9A%E4%B8%80%E5%80%8B%E7%B0%A1%E5%96%AE%E7%9A%84chatgp-api-%E9%80%A3%E7%BA%8C%E5%B0%8D%E8%A9%B1%E7%B7%B4%E7%BF%92-5b5c78ed3305
*/

using first_openai_api_test;
using Newtonsoft.Json;
using System.Text;

Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;

// openai api key
string apiKey = "{your api key}";

string model = "gpt-3.5-turbo-0125";

string userrole = "user";
string systemrole = "system";

var defultChatGPTMsg = new List<ChatGPTMsg>
{
    new ChatGPTMsg
    {
        role = userrole,
        content = "你是一個很精通日文的AI，如果理解我的意思，第一句話就用日文回答我"
    }
};

HttpClient httpClient = new HttpClient();

httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

while (true)
{
    var messagesss = defultChatGPTMsg;

    // 設定要發送的資料
    var requestDatas = new RequestData(
      model: model,
      messages: messagesss,
      temperature: 0.7,
      max_tokens: 60,
      top_p: 1,
      frequency_penalty: 0,
      presence_penalty: 0,
      stop: "\n"
      );

    string requestJson = System.Text.Json.JsonSerializer.Serialize(requestDatas);
    var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

    // 發送 POST 請求
    var response = await httpClient.PostAsync($"https://api.openai.com/v1/chat/completions", content);

    // 讀取回應資料
    var responseJson = await response.Content.ReadAsStringAsync();

    // 解析回應資料
    dynamic personData = JsonConvert.DeserializeObject(responseJson);
    dynamic message_id = personData.id;
    dynamic message = personData.choices[0].message.content;

    // 印出回應
    // Console.WriteLine(message_id);
    Console.WriteLine(message);

    // 設定下一個對話的 prompt

    messagesss.Add(new ChatGPTMsg { role = systemrole, content = message });

    // 等待使用者輸入
    Console.Write("> ");

    string userInput = Console.ReadLine();

    byte[] utf8Bytes = Encoding.UTF8.GetBytes(userInput);
    string utf8String = Encoding.UTF8.GetString(utf8Bytes);

    // 設定使用者輸入為下一個對話的
    messagesss.Add(new ChatGPTMsg { role = userrole, content = userInput });
}
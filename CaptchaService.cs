using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace CaptchaVerification
{
    public class CaptchaService(HttpClient client,CaptchaSettings captchaSettings )
    {
        public async Task<bool> IsValid(string token)
        {
            try
            {
                var postTask = await client
                    .PostAsync($"?secret={captchaSettings.SecretKey}&response={token}", new StringContent(""));
                var result = await postTask.Content.ReadAsStringAsync();
                var resultObject = JObject.Parse(result);
                dynamic success = resultObject["success"] ?? "";
                return (bool)success;
            }
            catch (Exception e)
            {
                // TODO: log this 
                return false;
            }

        }
    }
}

using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class HttpRequest {
    string api_path;

    public HttpRequest(string api_path) {
        this.api_path = api_path;
    }

    public JObject post<T>(T data) {

         // Serialize the anonymous type to a JSON string
        string json = JsonConvert.SerializeObject(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        // Create a UnityWebRequest object and configure it as a POST request with raw data
        using(UnityWebRequest request = new UnityWebRequest(Utils.HOST_URL + this.api_path, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            if(User.isTokenExist())
                request.SetRequestHeader("Authorization", "Bearer " + User.getToken());
            
            // Send the request and wait for the response
            request.SendWebRequest();
            while (!request.isDone) { }

            JObject obj = JObject.Parse(request.downloadHandler.text);

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Net.WebException((string)obj["message"]);

            return obj;
        }
    }
}
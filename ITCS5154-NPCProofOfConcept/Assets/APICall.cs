using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class APICall : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private string apiUrl = "https://api.example.com/ask";

    [Header("UI")]
    [SerializeField] private TMP_InputField questionInput;
    [SerializeField] private TMP_Text responseText;
    [SerializeField] private Button sendButton;

    // Public so other scripts/UI can read it
    public string latestResponse;

    public void SendQuestion()
    {
        string question = questionInput.text;

        if (string.IsNullOrEmpty(question))
        {
            Debug.LogWarning("Question is empty!");
            return;
        }

        StartCoroutine(PostQuestion(question));
    }

    IEnumerator PostQuestion(string question)
    {
        sendButton.interactable = false; // Disable button while waiting for response
        // Create JSON body
        QuestionData data = new QuestionData { question = question };
        string json = JsonUtility.ToJson(data);

         using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + question))
        {
            // Send the request and wait for response
            this.responseText.text = "Thinking...";
            yield return request.SendWebRequest();

            // Handle errors
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                // Success: print response
                string responseText = request.downloadHandler.text;
                json = JsonUtility.FromJson<ApiResponse>(responseText).message;
                Debug.Log("API Response: " + json);
                this.responseText.text = json;
                sendButton.interactable = true; // Re-enable button after response
            }
        }
    }

    [System.Serializable]
    public class QuestionData
    {
        public string question;
    }

    [System.Serializable]
    public class ApiResponse
    {
        public string message;
    }
}
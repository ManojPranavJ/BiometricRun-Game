using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class FirestoreManager : MonoBehaviour
{
    private string projectID = "vr-hall-booking"; // 🔴 REPLACE WITH YOUR FIREBASE PROJECT ID
    private string firestoreURL;
    private string getfirestoreURL;

    public UIHandler handler;

    private void Awake()
    {
        // 🔹 Fetch only the latest document from Firestore, ordered by timestamp (descending)
        firestoreURL = $"https://firestore.googleapis.com/v1/projects/{projectID}/databases/(default)/documents/halls/";
        getfirestoreURL = $"https://firestore.googleapis.com/v1/projects/{projectID}/databases/(default)/documents/halls/";


    }

    public void Start()
    {
        GetLatestDataFromFirestore();
    }

    // 🔥 STRUCTURE FOR FIRESTORE DATA
    [System.Serializable]
    public class FirestoreData
    {
        public Fields fields;
        public FirestoreData(int h1, int h2, int h3, int h4, string bookedBy)
        {
            fields = new Fields(h1, h2, h3, h4, bookedBy);
        }
    }

    [System.Serializable]
    public class Fields
    {
        public IntegerValue hallcode01;
        public IntegerValue hallcode02;
        public IntegerValue hallcode03;
        public IntegerValue hallcode04;
        public StringValue bookedBy;
        public TimestampValue timestamp;

        public Fields(int h1, int h2, int h3, int h4, string bookedBy)
        {
            hallcode01 = new IntegerValue(h1);
            hallcode02 = new IntegerValue(h2);
            hallcode03 = new IntegerValue(h3);
            hallcode04 = new IntegerValue(h4);
            this.bookedBy = new StringValue(bookedBy);
            timestamp = new TimestampValue(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }

    [System.Serializable] public class IntegerValue { public int integerValue; public IntegerValue(int value) { integerValue = value; } }
    [System.Serializable] public class StringValue { public string stringValue = "Sanjay"; public StringValue(string value) { stringValue = value; } }
    [System.Serializable] public class TimestampValue { public string timestampValue; public TimestampValue(string value) { timestampValue = value; } }

    // 🔹 SEND DATA TO FIRESTORE (CREATE A DOCUMENT)
    public void SendDataToFirestore(int h1, int h2, int h3, int h4, string bookedBy)
    {
        FirestoreData hallData = new FirestoreData(h1, h2, h3, h4, bookedBy);
        string jsonData = JsonConvert.SerializeObject(hallData);
        StartCoroutine(PostData(jsonData));
    }

    private IEnumerator PostData(string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(firestoreURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("🔥 Data sent successfully to Firestore.");
        }
        else
        {
            Debug.LogError("❌ Error sending data: " + request.error);
        }
    }

    // 🔹 GET LATEST DATA FROM FIRESTORE
    public void GetLatestDataFromFirestore()
    {
        StartCoroutine(GetLatestData());
    }

    private IEnumerator GetLatestData()
    {
        UnityWebRequest request = UnityWebRequest.Get(getfirestoreURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("📜 Firestore Data: " + json);

            // 🔹 Deserialize JSON into a structured class
            FirestoreResponse response = JsonConvert.DeserializeObject<FirestoreResponse>(json);

            /*if (response.documents != null && response.documents.Count > 0)
            {
                // 🔹 Get fields from the latest document
                FirestoreFields fields = response.documents[response.documents.Count - 1].fields;
                // ✅ Use the last document in the list (latest one)

                // 🔹 Assign Firestore values to local variables
                handler.hallcode01 = fields.hallcode01.integerValue;
                handler.hallcode02 = fields.hallcode02.integerValue;
                handler.hallcode03 = fields.hallcode03.integerValue;
                handler.hallcode04 = fields.hallcode04.integerValue;

                Debug.Log($"✅ Latest Values Assigned: {handler.hallcode01}, {handler.hallcode02}, {handler.hallcode03}, {handler.hallcode04}");
            }
            else
            {
                Debug.LogError("❌ No data found in Firestore.");
            }*/

            if (response.documents != null && response.documents.Count > 0)
            {
                FirestoreFields closestFields = null;
                TimeSpan shortestInterval = TimeSpan.MaxValue;
                DateTime currentTime = DateTime.UtcNow;

                for (int i = 0; i < response.documents.Count; i++)
                {
                    FirestoreFields fields = response.documents[i].fields;

                    if (fields.timestamp != null && !string.IsNullOrEmpty(fields.timestamp.timestampValue))
                    {
                        DateTime firestoreTime = DateTime.Parse(fields.timestamp.timestampValue);
                        TimeSpan interval = currentTime - firestoreTime;

                        if (interval < shortestInterval)
                        {
                            shortestInterval = interval;
                            closestFields = fields;
                        }
                    }
                }

                if (closestFields != null)
                {
                    Debug.Log("✅ Closest timestamp found: " + closestFields.timestamp.timestampValue);

                    // ✅ Assign values from the closest document
                    handler.hallcode01 = closestFields.hallcode01.integerValue;
                    handler.hallcode02 = closestFields.hallcode02.integerValue;
                    handler.hallcode03 = closestFields.hallcode03.integerValue;
                    handler.hallcode04 = closestFields.hallcode04.integerValue;

                    Debug.Log($"✅ Values Assigned: {handler.hallcode01}, {handler.hallcode02}, {handler.hallcode03}, {handler.hallcode04}");
                }
                else
                {
                    Debug.LogError("❌ No valid timestamps found in Firestore documents.");
                }
            }
            else
            {
                Debug.LogError("❌ No data found in Firestore.");
            }


        }
        else
        {
            Debug.LogError("❌ Error retrieving data: " + request.error);
        }
    }

    // 🔹 JSON Data Structure
    [System.Serializable]
    private class FirestoreResponse
    {
        public List<FirestoreDocument> documents;
    }

    [System.Serializable]
    private class FirestoreDocument
    {
        public FirestoreFields fields;
    }

    [System.Serializable]
    private class FirestoreFields
    {
        public FirestoreInt hallcode01;
        public FirestoreInt hallcode02;
        public FirestoreInt hallcode03;
        public FirestoreInt hallcode04;
        public FirestoreTimestamp timestamp;
    }

    [System.Serializable]
    private class FirestoreTimestamp
    {
        public string timestampValue;
    }


    [System.Serializable]
    private class FirestoreInt
    {
        public int integerValue;
    }

    // 🔹 UPDATE A SPECIFIC DOCUMENT
    public void UpdateDocument(string documentID, int h1, int h2, int h3, int h4, string bookedBy)
    {
        string updateURL = firestoreURL + "/" + documentID;
        FirestoreData updatedData = new FirestoreData(h1, h2, h3, h4, bookedBy);
        string jsonData = JsonConvert.SerializeObject(updatedData);
        StartCoroutine(PatchData(updateURL, jsonData));
    }

    private IEnumerator PatchData(string url, string jsonData)
    {
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Document updated successfully.");
        }
        else
        {
            Debug.LogError("❌ Error updating document: " + request.error);
        }
    }

    // 🔹 DELETE A DOCUMENT
    public void DeleteDocument(string documentID)
    {
        string deleteURL = firestoreURL + "/" + documentID;
        StartCoroutine(DeleteData(deleteURL));
    }

    private IEnumerator DeleteData(string url)
    {
        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("🗑️ Document deleted successfully.");
        }
        else
        {
            Debug.LogError("❌ Error deleting document: " + request.error);
        }
    }
}

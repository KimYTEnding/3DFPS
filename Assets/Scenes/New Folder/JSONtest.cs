using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class Data
{
    public string Name;
    public float Height;
    [JsonProperty]
    private string secret;

    public Data(string name, float height, string secret)
    {
        Name = name;
        Height = height;
        this.secret = secret;
    }

    public override string ToString()
    {
        return Name + " " + Height + " " + secret;

    }
}

public class JSONtest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Data charles = new Data("charles", 180, "asd");

        string json1 = JsonConvert.SerializeObject(charles);

        Debug.Log(json1);

        Data json2 = JsonConvert.DeserializeObject<Data>(json1);
        Debug.Log(json2.ToString());

        Save<Data>(json2, "Save2.txt");
        Debug.Log(Load<Data>("Save2.txt").ToString());
    }

    void Save<T>(T data, string fileName)
    {
        // 데이터를 저장할 수 있는 폴더의 경로(OS 관계없이 사용 가능)
        string path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(path);

        try
        {
            string json = JsonConvert.SerializeObject(data);
            json = SimpleEncryptionUtility.Encrypt(json);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    T Load<T>(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                json = SimpleEncryptionUtility.Decrypt(json);
                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 파일이 없다
                return default;
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return default;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

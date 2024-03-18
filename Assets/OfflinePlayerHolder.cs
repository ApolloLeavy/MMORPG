using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class OfflinePlayerHolder : MonoBehaviour
{
    bool isUsed = false;
    public bool isTeleporting = false;

    public static int previousScene = 0;
    public static int coins = 0;
    public static string pName = "Default Player";
    public static int character = 0;

    public bool isServer = false;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += this.OnSceneSwitch;
        string[] args = System.Environment.GetCommandLineArgs();
        foreach(string s in args)
        {
            switch (s) 
            {
                case "SERVER1":
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = args[0];
                    proc.StartInfo.Arguments = "SERVER2";
                    proc.Start();

                    System.Diagnostics.Process proc2 = new System.Diagnostics.Process();
                    proc2.StartInfo.FileName = args[0];
                    proc2.StartInfo.Arguments = "SERVER3";
                    proc2.Start();

                    isServer = true;
                    SceneManager.LoadScene(1);
                    break;
                case "SERVER2":
                    isServer = true;
                    SceneManager.LoadScene(2);
                    break;
                case "SERVER3":
                    isServer = true;
                    SceneManager.LoadScene(3);
                    break;
            }
        }
    }
    public IEnumerator Teleport(int scene)
    {
        NetworkCore myCore = GameObject.FindObjectOfType<NetworkCore>();
        if (!isTeleporting)
        {
            isTeleporting = true;
            previousScene = SceneManager.GetActiveScene().buildIndex;
            
            StartCoroutine(myCore.Disconnect(myCore.LocalConnectionID,true));
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadScene(scene);
        }    
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= this.OnSceneSwitch;
    }
    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnSceneSwitch(Scene s,LoadSceneMode l)
    {
        if(s.buildIndex == 0 && isUsed)
        {
            Destroy(this.gameObject);
        }
        else
        {
            isUsed = true;
        }
        NetworkCore myCore = GameObject.FindObjectOfType<NetworkCore>();
        if(myCore == null)
        {
            throw new System.Exception("No Network core"+s.buildIndex);
        }
        switch(s.buildIndex)
        {
            case 1:
                myCore.PortNumber = 9001;
                break;
            case 2:
                myCore.PortNumber = 9002;
                break;
            case 3:
                myCore.PortNumber = 9003;
                break;
        }
        if (isServer)
        {
            myCore.UI_StartServer();
        }
        else
        {
            myCore.IP = "127.0.0.1";
            myCore.UI_StartClient();
        }
    }
}

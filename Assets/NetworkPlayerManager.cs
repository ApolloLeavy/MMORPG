using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;
public class NetworkPlayerManager : NetworkComponent
{
    public override void HandleMessage(string flag, string value)
    {
        if(flag == "LOGIN" && IsServer)
        {
            string[] args = value.Split(',');
            int type = int.Parse(args[0]);
            int lastScene = int.Parse(args[1]);
            string pn = args[2];
            GameObject spawnLoc = GameObject.Find(lastScene.ToString());
            if(spawnLoc == null && lastScene != 0)
            {
                throw new System.Exception("Could not find spawn loc");
            }
            GameObject temp;
            if(lastScene != 0)
                temp = MyCore.NetCreateObject(type, Owner, spawnLoc.transform.position, Quaternion.identity);
            else
                temp = MyCore.NetCreateObject(type, Owner, new Vector3(0,5,0), Quaternion.identity);
            temp.GetComponent<PlayerController>().pName = pn;

        }
    }

    public override void NetworkedStart()
    {
        if (IsLocalPlayer)
            SendCommand("LOGIN", OfflinePlayerHolder.character.ToString() + "," + OfflinePlayerHolder.previousScene + "," + OfflinePlayerHolder.pName);
    }

    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(.1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

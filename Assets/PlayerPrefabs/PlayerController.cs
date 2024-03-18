using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using NETWORK_ENGINE;

public class PlayerController : NetworkComponent
{
    public float lastX;
    public float lastY;
    public string pName;
    public Rigidbody myRig;
    public float speed = 4.0f;
    public override void HandleMessage(string flag, string value)
    {
        if(IsClient && flag == "PNAME")
        {
            pName = value;
            this.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = pName;
        }
        if(IsServer && flag == "MOVE")
        {
            char[] temp = { '(', ')' };
            string[] args = value.Trim(temp).Split(',');
            lastX = float.Parse(args[0]);
            lastY = float.Parse(args[1]);
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (IsDirty)
            {
                SendUpdate("PNAME", pName);
                IsDirty = false;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRig = this.GetComponent<Rigidbody>();
    }
    public void Mover(InputAction.CallbackContext context)
    {
        Vector2 input = context.action.ReadValue<Vector2>();
        if (IsLocalPlayer)
        {
            SendCommand("MOVE", input.ToString());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            myRig.velocity = new Vector3(lastX, myRig.velocity.y, lastY) * speed;
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        OfflinePlayerHolder temp = GameObject.FindObjectOfType<OfflinePlayerHolder>();
        if (c.gameObject.tag == "Door" && IsLocalPlayer)
        {
            temp.StartCoroutine(temp.Teleport(int.Parse(c.gameObject.name)));
        }
    }
    private void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Door" && IsLocalPlayer)
        {
            GameObject.FindObjectOfType<OfflinePlayerHolder>().isTeleporting = false;
        }
    }
}

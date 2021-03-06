﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    public static ObjectPlacement instance;
    //[SerializeField] GameObject selectObj = null;
    //[SerializeField] GameObject manaBar = null;
    [SerializeField] GameObject objPosition = null;
    [SerializeField] float slowTime = 0.5f;
    [SerializeField] int portalCount = 5;

    private ManaBar mb;
    private ButtonManager bm = null;
    private AudioSource audioSource;
    private GameObject tempObj, selectObj;
    private GameObject[] allObjs;
    private SpriteRenderer[] spriteObjs;
    private Color tmp;
    private bool ready, placed, useMana, manaReady;
    private int index, portalUpdatedCount;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //manaReady = true;
        //mb = manaBar.GetComponent<ManaBar>();
        audioSource = Camera.main.GetComponent<AudioSource>();

        // Max five scrolls possibly
        portalUpdatedCount = portalCount;
        allObjs = new GameObject[portalCount];
        spriteObjs = new SpriteRenderer[2];
        if (bm == null) bm = ButtonManager.instance;
        ButtonManager.instance.UpdatePortalUses(portalUpdatedCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
            tempObj.transform.position = objPosition.transform.position;

        if (useMana)
        {
            //mb.UseMana(true);
            useMana = false;
        }

        if (selectObj != null)
        {
            objPosition.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            
            if (!ready && !placed && /*manaReady &&*/ index < portalCount)
            {
                //audioSource.PlayOneShot(scrollSelect);
                Debug.Log(index);
                tempObj = Instantiate(selectObj, objPosition.transform.position, Quaternion.identity);
                tempObj.GetComponent<ParticleSystem>().Stop();
                tempObj.GetComponent<CapsuleCollider2D>().enabled = false;

                spriteObjs = tempObj.GetComponentsInChildren<SpriteRenderer>();
                tmp = spriteObjs[0].color;
                tmp.a = 0.3f;
                spriteObjs[0].color = spriteObjs[1].color = tmp;

                //if (Time.timeScale > 0)
                    //Time.timeScale = slowTime;

                ready = true;
            }
            else if (Input.GetButtonDown("Fire2") && ready && !placed)
            {
                //if (Time.timeScale > 0)
                    //Time.timeScale = 1f;

                Destroy(tempObj);
                selectObj = null;
                bm.Reset();
                ready = false;
            }

            if (Input.GetButtonDown("Fire1") && ready && index < portalCount)
            {
                //if (Time.timeScale > 0)
                    //Time.timeScale = 1f;

                tmp.a = 1f;
                spriteObjs[0].color = spriteObjs[1].color = tmp;

                if (tempObj.GetComponent<AltTime>() != null)
                    audioSource.PlayOneShot(tempObj.GetComponent<AltTime>().GetPortalSound());
                else if (tempObj.GetComponent<AltGrav>() != null)
                    audioSource.PlayOneShot(tempObj.GetComponent<AltGrav>().GetPortalSound());

                tempObj.GetComponent<Animator>().enabled = true;
                allObjs[index] = tempObj;

                index++;
                portalUpdatedCount--;
                ready = false;
                placed = true;
                //useMana = true;

                ButtonManager.instance.UpdatePortalUses(portalUpdatedCount);
                if (index >= portalCount)
                    bm.PortalsUsedUp();
                else
                    bm.Reset();
            }
        }
    }

    public List<Transform> GetAllSelectObjs(GameObject obj)
    {
        var tmp = new List<Transform>();

        foreach (Transform child in obj.transform)
        {
            tmp.Add(child);
        }

        return tmp;
    }

    public void SetSelected(GameObject obj)
    {
        if (selectObj != null)
            selectObj = null;

        selectObj = obj;
        ready = false;
        placed = false;
    }


    public void SetManaReady(bool status)
    {
        manaReady = status;
    }

    public void OutOfMana()
    {
        for (int i = 0; i < index; i++)
        {
            Destroy(allObjs[i].gameObject);
        }
        index = 0;
        ready = false;
        placed = false;
        manaReady = false;

        if (tempObj != null)
            Destroy(tempObj);
    }
}

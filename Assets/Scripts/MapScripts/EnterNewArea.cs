using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnterNewArea : MonoBehaviour
{
    public string areaName;
    public AudioClip ambientMusic;
    public AudioClip combatMusic;
    public float fadeinTimer;
    public float fadeStopTime;
    public float fadeoutTimer;
    private TextMeshProUGUI titleCard;
    private bool used = false;
    void Start()
    {
        titleCard = GameObject.Find("TitleCard").GetComponent<TextMeshProUGUI>();
        if(titleCard == null)
        {
            Debug.LogError("title card not found");
            return;
        }
        titleCard.gameObject.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(used) return; //safeguard so that it only triggers once
        used = true;
        Debug.Log("Entering area: " + areaName);
        //the player enters a new area
        if(ambientMusic != null && combatMusic != null)
        {
            MusicHandler.Instance.SetMusic(ambientMusic, combatMusic);
        } else
        {
            Debug.LogWarning("music is null");
        }
        //title card drop!!!!!!!!!!!!!!!!!!!!!
        StartCoroutine(TitleCard());
    }
    private IEnumerator TitleCard()
    {
        //start
        titleCard.gameObject.SetActive(true);
        titleCard.text = areaName;
        //fade in
        float intimer = 0f;
        while (intimer < fadeinTimer)
        {
            intimer += Time.deltaTime;

            float t = intimer / fadeinTimer;
            titleCard.color = new Color(t, 0, 0, t);

            yield return null;
        }
        //wait
        yield return new WaitForSeconds(fadeStopTime);
        //fadeout
        float outtimer = 0f;
        while (outtimer < fadeoutTimer)
        {
            outtimer += Time.deltaTime;

            float t = outtimer / fadeoutTimer;
            titleCard.color = new Color(1f - t, 0, 0, 1f - t);

            yield return null;
        }
        //end
        titleCard.gameObject.SetActive(false);
        Destroy(gameObject);
    }
    void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }

}

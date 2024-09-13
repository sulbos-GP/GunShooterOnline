using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using static MatchmakerResource;
using WebCommonLibrary.DTO.Matchmaker;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Error;

public class MatchingZone : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform matchSetupTransform = canvas.transform.Find("MatchSetup");
            if (matchSetupTransform != null)
            {
                GameObject matchSetupObject = matchSetupTransform.gameObject;
                if (matchSetupObject != null)
                {
                    matchSetupObject.SetActive(true);
                }
            }
        }
    }



    /// <summary>
    /// 매칭 취소 요청
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.activeInHierarchy)
        {
            return;
        }

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform matchSetupTransform = canvas.transform.Find("MatchSetup");
            if (matchSetupTransform != null)
            {
                GameObject matchSetupObject = matchSetupTransform.gameObject;
                if (matchSetupObject != null)
                {
                    matchSetupObject.SetActive(false);
                }
            }
        }

        GameObject matchingHub = GameObject.Find("@MatchmakerHub");
        if (matchingHub == null)
        {
            return;
        }
        MatchmakerHub hub = matchingHub.GetComponent<MatchmakerHub>();
        hub.OnCancleRequeset();


    }

}

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TriggerGoal : MonoBehaviour
{
    [SerializeField] private bool playerOne;

    [SerializeField] private Text text;

    private void OnTriggerEnter(Collider other)
    {
        if ("puck".Equals(other.tag))
        {
            var resetScript = other.GetComponent<ResetScript>();
            if (resetScript != null) resetScript.ResetObject();
        }

        var oldText = text.text;
        var playersText = oldText.Split(" : ").Select(int.Parse).ToArray();

        if (playerOne)
            text.text = playersText[0] + " : " + (playersText[1] + 1);
        else
            text.text = playersText[0] + 1 + " : " + playersText[1];
    }
}
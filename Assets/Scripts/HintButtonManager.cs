using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HintButtonManager : MonoBehaviour
{
    public GameObject hintButton;     // The hint button UI
    public GameObject dialogueBox;    // The dialogue box UI
    public string hintText;           // The hint text to show

    private float timeSpent = 0f;
    private bool hintShown = false;
    private Coroutine closeHintCoroutine;

    void Start()
    {
        // Hide UI at start
        if (hintButton != null)
            hintButton.SetActive(false);

        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    void Update()
    {
        timeSpent += Time.deltaTime;

        if (!hintShown && timeSpent >= 120f) // 2 minutes
        {
            hintButton.SetActive(true);
            hintShown = true;
        }
    }

    public void OnHintButtonClicked()
    {
        if (dialogueBox != null)
        {
            // Show the dialogue box
            dialogueBox.SetActive(true);

            var text = dialogueBox.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = hintText;

            // Cancel any existing coroutine, just in case
            if (closeHintCoroutine != null)
                StopCoroutine(closeHintCoroutine);

            // Start auto-close coroutine
            closeHintCoroutine = StartCoroutine(CloseHintAfterDelay(5f));
        }
    }

    private IEnumerator CloseHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBox.SetActive(false);
        closeHintCoroutine = null;
    }
}

using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    [Header("Settings")]
    public Transform targetPoint;
    public float walkSpeed = 3f;

    [Header("Animasi")]
    public string animationParam = "isWalking"; 

    [Header("UI (Opsional)")]
    public GameObject dungeonChoiceUI;

    private GameObject playerObj;
    private Animator playerAnim;
    private PlayerMovement playerMoveScript;
    private PlayerAction playerActionScript;

    private bool isCutsceneActive = true;
    private Vector3 originalScale;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            originalScale = playerObj.transform.localScale;

            playerAnim = playerObj.GetComponent<Animator>();
            playerMoveScript = playerObj.GetComponent<PlayerMovement>();
            playerActionScript = playerObj.GetComponent<PlayerAction>();

            if (playerMoveScript) playerMoveScript.enabled = false;
            if (playerActionScript) playerActionScript.enabled = false;
        }

        if (dungeonChoiceUI) dungeonChoiceUI.SetActive(false);
    }

    void Update()
    {
        if (!isCutsceneActive || playerObj == null) return;

        float step = walkSpeed * Time.deltaTime;
        playerObj.transform.position = Vector3.MoveTowards(
            playerObj.transform.position,
            targetPoint.position,
            step
        );

        SetWalking(true);

        if (Vector3.Distance(playerObj.transform.position, targetPoint.position) < 0.1f)
        {
            FinishCutscene();
        }
    }

    void SetWalking(bool isWalking)
    {
        if (!playerAnim) return;

        playerAnim.SetBool(animationParam, isWalking);

        float facingX = (targetPoint.position.x > playerObj.transform.position.x) ? 1f : -1f;
        playerObj.transform.localScale = new Vector3(
            Mathf.Abs(originalScale.x) * facingX,
            originalScale.y,
            originalScale.z
        );
    }

    void FinishCutscene()
    {
        isCutsceneActive = false;
        SetWalking(false);

        if (dungeonChoiceUI != null)
        {
            dungeonChoiceUI.SetActive(true);
        }
        else
        {
            if (playerMoveScript) playerMoveScript.enabled = true;
            if (playerActionScript) playerActionScript.enabled = true;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UICharacterSync : MonoBehaviour
{
    [Header("Target Asli (Player di Scene)")]
    public GameObject playerObject;      
    public Animator playerAnimator;      
    public SpriteRenderer playerHand;    

    [Header("UI Character (Diri Sendiri)")]
    public Animator uiAnimator;          
    public RectTransform uiRect;         
    public Image uiHandImage;            

    private Vector3 originalUIScale;

    void Awake()
    {
        if (uiRect != null) 
        {
            originalUIScale = uiRect.localScale;
        }
    }

    void Update()
    {
        if (playerObject == null || uiAnimator == null) return;

        // Syncronize the position of the UI character with the player character
        // Sync Animasi
        
        uiAnimator.SetBool("isWalking", playerAnimator.GetBool("isWalking")); 
        
        uiAnimator.SetFloat("isArmed", playerAnimator.GetFloat("isArmed")); 

        // buat ubah arah 

        float direction = Mathf.Sign(playerObject.transform.localScale.x);
        uiRect.localScale = new Vector3(
            Mathf.Abs(originalUIScale.x) * direction, 
            originalUIScale.y, 
            originalUIScale.z
        );

        if (uiHandImage != null && playerHand != null)
        {
            uiHandImage.sprite = playerHand.sprite;
            uiHandImage.enabled = playerHand.sprite != null;
            uiHandImage.transform.rotation = playerHand.transform.rotation;
        }
    }
}
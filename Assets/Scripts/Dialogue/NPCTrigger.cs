using UnityEngine;
using UnityEngine.InputSystem;

public class NPCDialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject Key;
    private DialogueManager dialogueManager;
    public DialogueData dialogue; 
    private bool isPlayerInRange = false;
    private GameInput gameInput;

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
        dialogueManager.OnDialogueEnd += SpawnKey;

        gameInput = GameInput.Instance;
        if (gameInput == null)
        {
            Debug.LogError("GameInput script not found in the scene!");
        }

        gameInput.inputActions.PlayerInput.Interact.performed += _ => NPCConvo();
    }

    private void SpawnKey()
    {
       if(Key != null)
        {
            Key.SetActive(true);
        }
    }

    void NPCConvo()
    {
        if (isPlayerInRange)
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            DialogueManager.Instance.EndDialogue();
        }
    }
}

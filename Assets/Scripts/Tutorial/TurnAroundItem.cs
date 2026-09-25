using System.Collections;
using UnityEngine;

public class TurnAroundItem : MonoBehaviour
{
    public TutorialTurnAround turnAround;

    public GameObject txt;

    public Camera playerCamera;

    private float lookTimer;
    public float transitionDuration = 1.25f;
    private bool completed;

    // Testing
    [SerializeField] private MeshRenderer sphereRenderer;
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private Color greenColor = Color.green;

    private Material sphereMaterial;
    private Coroutine colorCoroutine;

    private void OnValidate()
    {
        if(sphereRenderer == null)
        {
            sphereRenderer = GetComponentInChildren<MeshRenderer>(true);
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

    }

    private void Awake()
    {
        if (sphereRenderer == null)
        {
            sphereRenderer = GetComponentInChildren<MeshRenderer>(true);
        }

        if (sphereRenderer == null)
        {
            Debug.LogError("No se encontró un MeshRenderer en los hijos.", this);
            return;
        }

        sphereMaterial = sphereRenderer.material;
        sphereMaterial.color = redColor;
    }


    private void Update()
    {
        if (completed || playerCamera == null)
            return;

        bool isLooking = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 10f))
        {
            TurnAroundItem observedItem = hit.collider.GetComponentInParent<TurnAroundItem>();
            isLooking = observedItem == this;
        }

        if (isLooking)
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= transitionDuration)
            {
                lookTimer = transitionDuration;
                completed = true;

                AudioManager.Instance.SystemNotification(true);
                turnAround.AddPoints();
                gameObject.SetActive(false);

            }

        }

        else
        {
            lookTimer -= Time.deltaTime;
            lookTimer = Mathf.Max(lookTimer, 0f);
        }

        float progress = lookTimer / transitionDuration;

        sphereMaterial.color = Color.Lerp(redColor, greenColor, progress);
       
    }

    public void UpdateColor(bool status)
    {
        Color targetColor = status ? greenColor : redColor;

        if (colorCoroutine != null) StopCoroutine(colorCoroutine);

        colorCoroutine = StartCoroutine(ChangeColor(targetColor));
    }

    private IEnumerator ChangeColor(Color targetColor)
    {
        Color initialColor = sphereMaterial.color;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / transitionDuration;

            sphereMaterial.color = Color.Lerp(initialColor, targetColor, progress);

            yield return null;
        }

        sphereMaterial.color = targetColor;
        yield return null;
    }

    public void TurnText(GameObject obj)
    {
        txt.transform.LookAt(obj.transform);
        txt.transform.Rotate(0, 180f, 0);
    }
}
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public enum PlayerView { Normal, Mirror }

    [Header("Cinemachine Cameras (Cinemachine 3)")]
    [SerializeField] private CinemachineCamera mapCam;
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachineCamera reflectionCam;

    [Header("Timing")]
    [SerializeField] private float mapCamHold = 1.25f;    
    [SerializeField] private float switchCamHold = 0.6f;   

    [Header("Priorities")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;

    [Header("Input")]
    [SerializeField] private InputActionReference toggleMapCam; 
    [SerializeField] private InputActionReference switchMaps;  

    private PlayerView current = PlayerView.Normal;
    private bool isTransitioning;
    private PlayerView lastPlayerView = PlayerView.Normal;

    private void OnEnable()
    {
        if (switchMaps != null)
        {
            switchMaps.action.performed += OnSwitchMaps;
            switchMaps.action.Enable(); 
        }

        if (toggleMapCam != null)
        {
            toggleMapCam.action.performed += OnToggleMap;
            toggleMapCam.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (switchMaps != null)
        {
            switchMaps.action.performed -= OnSwitchMaps;
            switchMaps.action.Disable();
        }

        if (toggleMapCam != null)
        {
            toggleMapCam.action.performed -= OnToggleMap;
            toggleMapCam.action.Disable();
        }
    }

    private void Start()
    {
        SetMapView();
        StartCoroutine(GoToPlayerAfterDelay(PlayerView.Normal, mapCamHold));
    }

    private void OnSwitchMaps(InputAction.CallbackContext _)
    {
        if (isTransitioning) return;

        PlayerView next = (current == PlayerView.Normal) ? PlayerView.Mirror : PlayerView.Normal;
        StartCoroutine(SwapSequence(next));
    }

    private void OnToggleMap(InputAction.CallbackContext _)
    {
        if (isTransitioning) return;

        if (current == PlayerView.Normal || current == PlayerView.Mirror)
        {
            lastPlayerView = current;
            SetMapView();

        }
        else
        {
            SetPlayer(lastPlayerView);
        }
    }

    private IEnumerator SwapSequence(PlayerView next)
    {
        isTransitioning = true;

        SetMapView();
        yield return new WaitForSeconds(switchCamHold);

        SetPlayer(next);
        current = next;

        isTransitioning = false;
    }

    private IEnumerator GoToPlayerAfterDelay(PlayerView target, float delay)
    {
        isTransitioning = true;
        yield return new WaitForSeconds(delay);

        SetPlayer(target);
        current = target;

        isTransitioning = false;
    }

    private void SetMapView()
    {
        mapCam.Priority = activePriority;
        playerCam.Priority = inactivePriority;
        reflectionCam.Priority = inactivePriority;
    }

    private void SetPlayer(PlayerView which)
    {
        mapCam.Priority = inactivePriority;
        playerCam.Priority = (which == PlayerView.Normal) ? activePriority : inactivePriority;
        reflectionCam.Priority = (which == PlayerView.Mirror) ? activePriority : inactivePriority;
    }
}
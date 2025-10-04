using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Cube Management")]
    [SerializeField] GameObject[] cubes;
    [SerializeField] GameObject[] cubeSockets;
    [SerializeField] Transform[] colliders;
    [SerializeField] CameraFollow cameraFollow;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    public float CubeMoveSpeed;

    int activeCubeIndex;
    int activeCubeSocketIndex;
    public int CollectedCubeCount;
    int sceneIndex;
    bool isTouchEnabled = true;
    bool isGameOver;
    bool onFirstTouch;

    public bool IsGameOver
    {
        get => isGameOver;
        set => isGameOver = value;
    }

    [Header("UI Management")]
    [SerializeField] GameObject[] panels;
    [SerializeField] TextMeshProUGUI bestScoreText;

    [Header("Audio Management")]
    [SerializeField] AudioSource[] sounds;
    [SerializeField] Image[] buttonImages;
    [SerializeField] Sprite[] spriteObjects;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (!PlayerPrefs.HasKey("GameSound"))
            PlayerPrefs.SetInt("GameSound", 0);

        if (!PlayerPrefs.HasKey("EffectSound"))
            PlayerPrefs.SetInt("EffectSound", 0);

        InitializeScene();
    }

    void Start()
    {
        OpenPanel(0);
    }

    void Update()
    {
        if (isGameOver || !isTouchEnabled) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleTouchInput();
            HandleFirstTouch();
        }
    }

    private void HandleFirstTouch()
    {
        if (onFirstTouch) return;

        onFirstTouch = true;
        ClosePanel(1);
        SpawnNewCube();
    }

    private void HandleTouchInput()
    {
        if (!onFirstTouch) return;
        StopTile();
    }

    private void StopTile()
    {
        if (activeCubeIndex == 0) return;

        var currentCube = cubes[activeCubeIndex - 1];
        currentCube.GetComponent<Cube>().canMove = false;
        currentCube.GetComponent<Rigidbody>().useGravity = true;
        sounds[3].Play();

        isTouchEnabled = false;
        cameraFollow.target = currentCube.transform;
    }

    public void SpawnNewCube()
    {
        if (isGameOver || activeCubeIndex >= cubes.Length)
        {
            EndGame();
            return;
        }

        if (activeCubeIndex != 0)
            cubes[activeCubeIndex - 1].tag = "Untagged";

        var socket = cubeSockets[activeCubeSocketIndex].transform;
        var cube = cubes[activeCubeIndex];
        cube.transform.SetPositionAndRotation(socket.position, socket.rotation);
        cube.SetActive(true);
        cube.GetComponent<Cube>().canMove = true;

        activeCubeIndex++;
        activeCubeSocketIndex = (activeCubeSocketIndex == 1) ? 0 : 1;
        isTouchEnabled = true;
        CubeMoveSpeed += 0.1f;

        foreach (var col in colliders)
            col.transform.position += new Vector3(0f, 0.1f, 0f);

        foreach (var sock in cubeSockets)
            sock.transform.position += new Vector3(0f, 0.1f, 0f);
    }



    public void EndGame()
    {
        
        bestScoreText.text = " " + CollectedCubeCount.ToString();

        isGameOver = true;
        sounds[2].Play();
        OpenPanel(3);
        Time.timeScale = 0f;
    }


    public void HandleButtonActions(string action)
    {
        sounds[1].Play();

        switch (action)
        {
            case "Pause":
                OpenPanel(2);
                Time.timeScale = 0f;
                break;

            case "Resume":
                ClosePanel(2);
                Time.timeScale = 1f;
                break;

            case "Retry":
                SceneManager.LoadScene(sceneIndex);
                Time.timeScale = 1f;
                break;

            case "Quit":
                OpenPanel(4);
                break;

            case "ConfirmQuit":
                Application.Quit();
                break;

            case "CancelQuit":
                ClosePanel(4);
                break;

            case "StartGame":
                ClosePanel(0);
                OpenPanel(1);
                onFirstTouch = true;
                SpawnNewCube();
                break;

            case "ToggleGameSound":
                ToggleSoundSetting("GameSound", 0, buttonImages[0], spriteObjects[0], spriteObjects[1], sounds[0]);
                break;

            case "ToggleEffectSound":
                bool newEffectState = ToggleSoundSetting("EffectSound", 1, buttonImages[1], spriteObjects[2], spriteObjects[3]);
                for (int i = 1; i < sounds.Length; i++)
                    sounds[i].mute = newEffectState;
                break;
        }
    }

    private void InitializeScene()
    {
        bestScoreText.text = PlayerPrefs.GetInt("BestScore", 0).ToString();

        sounds[0].mute = PlayerPrefs.GetInt("GameSound") != 0;
        buttonImages[0].sprite = spriteObjects[sounds[0].mute ? 1 : 0];

        bool isEffectMuted = PlayerPrefs.GetInt("EffectSound") != 0;
        buttonImages[1].sprite = spriteObjects[isEffectMuted ? 3 : 2];

        for (int i = 1; i < sounds.Length; i++)
            sounds[i].mute = isEffectMuted;
    }

    private bool ToggleSoundSetting(string key, int soundIndex, Image button, Sprite spriteOn, Sprite spriteOff, AudioSource audio = null)
    {
        bool current = PlayerPrefs.GetInt(key) == 1;
        bool next = !current;

        PlayerPrefs.SetInt(key, next ? 1 : 0);
        button.sprite = next ? spriteOff : spriteOn;

        if (audio != null)
            audio.mute = next;

        return next;
    }

    private void OpenPanel(int index)
    {
        if (index < panels.Length)
            panels[index].SetActive(true);
    }

    private void ClosePanel(int index)
    {
        if (index < panels.Length)
            panels[index].SetActive(false);
    }
}

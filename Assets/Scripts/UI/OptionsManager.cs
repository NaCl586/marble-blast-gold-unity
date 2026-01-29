using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class OptionsManager : MonoBehaviour
{
    public Button audioButton;
    public Button cameraButton;
    public Button cntr_cam_dwn;
    public Button cntr_cam_lft;
    public Button cntr_cam_rt;
    public Button cntr_cam_up;
    public Button cntr_mrb_bak;
    public Button cntr_mrb_fw;
    public Button cntr_mrb_jmp;
    public Button cntr_mrb_lft;
    public Button cntr_mrb_pwr;
    public Button cntr_mrb_rt;
    public Button cntrl_mous_bttn;
    public Button controlsButton;
    public Button grafapply;
    public Button graphicsButton;
    public Button homeButton;
    public Button marbleButton;
    public Button mouseButton;
    [Space]
    public Slider mouseSensitivitySlider;
    public Slider musicSlider;
    public Slider soundSlider;
    [Space]
    public Toggle cntrl_mous_freel;
    public Toggle cntrl_mous_invrt;
    public Toggle graf16bt;
    public Toggle graf32bt;
    public Toggle graf640;
    public Toggle graf800;
    public Toggle graf1024;
    public Toggle graf_chkbx;
    public Toggle grafdir3d;
    public Toggle grafful;
    public Toggle grafopgl;
    public Toggle grafwindo;
    [Space]
    public GameObject graphicsMenu;
    public GameObject audioMenu;
    public GameObject controlsMenu;
    public GameObject marbleMenu;
    public GameObject cameraMenu;
    public GameObject mouseMenu;
    public GameObject remapMenu;
    public GameObject confirmMenu;
    [Space]
    public TextMeshProUGUI remapCaption;
    public TextMeshProUGUI confirmCaption;
    public Button yesButton;
    public Button noButton;

    int resolutionIndex;
    string colorMode, videoDriver;
    bool fullScreen, shadow;
    string bindToBeRemapped;
    Button buttonToBeRemapped;
    string conflictedMapping;
    Button conflictedButton;
    KeyCode tempKeycode;

    public void Start()
    {
        audioButton.onClick.AddListener(() => SetOptionsMenu(1));
        cameraButton.onClick.AddListener(() => SetControlsMenu(1));
        cntr_cam_dwn.onClick.AddListener(() => RemapButton("Rotate Camera Down", cntr_cam_dwn));
        cntr_cam_lft.onClick.AddListener(() => RemapButton("Rotate Camera Left", cntr_cam_lft));
        cntr_cam_rt.onClick.AddListener(() => RemapButton("Rotate Camera Right", cntr_cam_rt));
        cntr_cam_up.onClick.AddListener(() => RemapButton("Rotate Camera Up", cntr_cam_up));
        cntr_mrb_bak.onClick.AddListener(() => RemapButton("Move Backward", cntr_mrb_bak));
        cntr_mrb_fw.onClick.AddListener(() => RemapButton("Move Forward", cntr_mrb_fw));
        cntr_mrb_jmp.onClick.AddListener(() => RemapButton("Jump", cntr_mrb_jmp));
        cntr_mrb_lft.onClick.AddListener(() => RemapButton("Move Left", cntr_mrb_lft));
        cntr_mrb_pwr.onClick.AddListener(() => RemapButton("Use Powerup", cntr_mrb_pwr));
        cntr_mrb_rt.onClick.AddListener(() => RemapButton("Move Right", cntr_mrb_rt));
        cntrl_mous_bttn.onClick.AddListener(() => RemapButton("Free-Look Key", cntrl_mous_bttn));
        controlsButton.onClick.AddListener(() => SetOptionsMenu(2));
        grafapply.onClick.AddListener(() => ApplyGraphics());
        graphicsButton.onClick.AddListener(() => SetOptionsMenu(0));
        homeButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        marbleButton.onClick.AddListener(() => SetControlsMenu(0));
        mouseButton.onClick.AddListener(() => SetControlsMenu(2));

        yesButton.onClick.AddListener(() => ForceMapping());
        noButton.onClick.AddListener(() => CancelMapping());

        cntrl_mous_freel.onValueChanged.AddListener(SetFreeLook);
        cntrl_mous_invrt.onValueChanged.AddListener(SetInvertYAxis);
        graf16bt.onValueChanged.AddListener(SetColorDepth16Bit);
        graf32bt.onValueChanged.AddListener(SetColorDepth32Bit);
        graf640.onValueChanged.AddListener(SetResolution640);
        graf800.onValueChanged.AddListener(SetResolution800);
        graf1024.onValueChanged.AddListener(SetResolution1024);
        graf_chkbx.onValueChanged.AddListener(SetShadow);
        grafdir3d.onValueChanged.AddListener(SetVideoDriverD3D);
        grafful.onValueChanged.AddListener(SetWindowFull);
        grafopgl.onValueChanged.AddListener(SetVideoDriverOpenGL);
        grafwindo.onValueChanged.AddListener(SetWindowWindow);

        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundSlider.onValueChanged.AddListener(SetSoundVolume);

        SetDefaults();
        SetControlText();

        SetOptionsMenu(0);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

    public void SetDefaults()
    {
        resolutionIndex = PlayerPrefs.GetInt("Graphics_ScreenResolution", 2);
        if(resolutionIndex == 0)
            graf640.SetIsOnWithoutNotify(true);
        else if (resolutionIndex == 1) 
            graf800.SetIsOnWithoutNotify(true);
        else
            graf1024.SetIsOnWithoutNotify(true);

        fullScreen = PlayerPrefs.GetInt("Graphics_Fullscreen", 0) == 1;
        grafful.SetIsOnWithoutNotify(fullScreen);
        grafwindo.SetIsOnWithoutNotify(!fullScreen);

        shadow = PlayerPrefs.GetInt("Graphics_Shadow", 1) == 1;
        graf_chkbx.SetIsOnWithoutNotify(shadow);

        string _videoDriver = PlayerPrefs.GetString("Graphics_VideoDriver", "OpenGL");
        if (_videoDriver == "OpenGL") grafopgl.SetIsOnWithoutNotify(true);
        else if (_videoDriver == "D3D") grafdir3d.SetIsOnWithoutNotify(true);
        videoDriver = _videoDriver;

        string _colorDepth = PlayerPrefs.GetString("Graphics_ColorDepth", "32-bit");
        if (_colorDepth == "32-bit") graf32bt.SetIsOnWithoutNotify(true);
        else if (_colorDepth == "16-bit") graf16bt.SetIsOnWithoutNotify(true);
        colorMode = _colorDepth;

        cntrl_mous_freel.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Controls_Mouse_Freelook", 1) == 1);
        cntrl_mous_invrt.SetIsOnWithoutNotify(PlayerPrefs.GetInt("Controls_Mouse_InvertYAxis", 0) == 1);

        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("Controls_MouseSensitivity", 1);
        musicSlider.value = PlayerPrefs.GetFloat("Audio_MusicVolume", 0.5f);
        soundSlider.value = PlayerPrefs.GetFloat("Audio_SoundVolume", 0.5f);
    }

    public void SetOptionsMenu(int index)
    {
        graphicsMenu.SetActive(false);
        audioMenu.SetActive(false);
        controlsMenu.SetActive(false);

        graphicsButton.transform.SetAsFirstSibling();
        audioButton.transform.SetAsFirstSibling();
        controlsButton.transform.SetAsFirstSibling();

        switch (index)
        {
            case 0:
                graphicsButton.transform.SetAsLastSibling();
                graphicsMenu.SetActive(true); 
                break;
            case 1:
                audioButton.transform.SetAsLastSibling();
                audioMenu.SetActive(true); 
                break;
            case 2:
                controlsButton.transform.SetAsLastSibling();
                controlsMenu.transform.SetAsLastSibling();
                homeButton.transform.SetAsLastSibling();
                controlsMenu.SetActive(true); 
                break;
        }
    }

    public void SetControlsMenu(int index)
    {
        marbleMenu.SetActive(false);
        cameraMenu.SetActive(false);
        mouseMenu.SetActive(false);

        switch (index)
        {
            case 0: marbleMenu.SetActive(true); break;
            case 1: cameraMenu.SetActive(true); break;
            case 2: mouseMenu.SetActive(true); break;
        }
    }

    #region Graphics
    public void ApplyGraphics()
    {
        if (resolutionIndex == 0)
            Screen.SetResolution(1280, 720, fullScreen);
        else if (resolutionIndex == 1)
            Screen.SetResolution(1366, 769, fullScreen);
        else
            Screen.SetResolution(1920, 1080, fullScreen);

        PlayerPrefs.SetInt("Graphics_ScreenResolution", resolutionIndex);
        PlayerPrefs.SetInt("Graphics_Shadow", shadow ? 1 : 0);

        PlayerPrefs.SetInt("Graphics_Fullscreen", fullScreen ? 1 : 0);
        PlayerPrefs.SetString("Graphics_VideoDriver", videoDriver);
        PlayerPrefs.SetString("Graphics_ColorDepth", colorMode);
        PlayerPrefs.SetInt("Graphics_Shadow", shadow ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void SetResolution640(bool isOn)
    {
        if (!isOn)
        {
            graf640.SetIsOnWithoutNotify(true);
            return;
        }

        graf800.SetIsOnWithoutNotify(false);
        graf1024.SetIsOnWithoutNotify(false);

        resolutionIndex = 0;
    }


    public void SetResolution800(bool isOn)
    {
        if (!isOn)
        {
            graf800.SetIsOnWithoutNotify(true);
            return;
        }

        graf640.SetIsOnWithoutNotify(false);
        graf1024.SetIsOnWithoutNotify(false);

        resolutionIndex = 1;
    }


    public void SetResolution1024(bool isOn)
    {
        if (!isOn)
        {
            graf1024.SetIsOnWithoutNotify(true);
            return;
        }

        graf640.SetIsOnWithoutNotify(false);
        graf800.SetIsOnWithoutNotify(false);

        resolutionIndex = 2;
    }

    public void SetVideoDriverOpenGL(bool isOn)
    {
        if (!isOn)
        {
            grafopgl.SetIsOnWithoutNotify(true);
            return;
        }
        grafdir3d.SetIsOnWithoutNotify(false);

        videoDriver = "OpenGL";
    }

    public void SetVideoDriverD3D(bool isOn)
    {
        if (!isOn)
        {
            grafdir3d.SetIsOnWithoutNotify(true);
            return;
        }
        grafopgl.SetIsOnWithoutNotify(false);

        videoDriver = "D3D";
    }

    public void SetWindowFull(bool isOn)
    {
        if (!isOn)
        {
            grafful.SetIsOnWithoutNotify(true);
            return;
        }
        grafwindo.SetIsOnWithoutNotify(false);
        
        fullScreen = true;
    }

    public void SetWindowWindow(bool isOn)
    {
        if (!isOn)
        {
            grafwindo.SetIsOnWithoutNotify(true);
            return;
        }
        grafful.SetIsOnWithoutNotify(false);

        fullScreen = false;
    }

    public void SetColorDepth16Bit(bool isOn)
    {
        if (!isOn)
        {
            graf16bt.SetIsOnWithoutNotify(true);
            return;
        }
        graf32bt.SetIsOnWithoutNotify(false);

        colorMode = "16-bit";
    }

    public void SetColorDepth32Bit(bool isOn)
    {
        if (!isOn)
        {
            graf32bt.SetIsOnWithoutNotify(true);
            return;
        }
        graf16bt.SetIsOnWithoutNotify(false);

        colorMode = "32-bit";
    }

    public void SetShadow(bool isOn)
    {   
        shadow = isOn;
    }

    #endregion

    #region Sliders
    public void SetMusicVolume(float volume)
    {
        musicSlider.value = volume;
        PlayerPrefs.SetFloat("Audio_MusicVolume", volume);

        MenuMusic.instance.GetComponent<AudioSource>().volume = volume;

        PlayerPrefs.Save();
    }

    public void SetSoundVolume(float volume)
    {
        soundSlider.value = volume;
        PlayerPrefs.SetFloat("Audio_SoundVolume", volume);

        foreach (var audioSource in FindObjectsOfType<AudioSource>())
            if(!audioSource.GetComponent<MenuMusic>())
                audioSource.volume = PlayerPrefs.GetFloat("Audio_SoundVolume", 0.5f);

        PlayerPrefs.Save();
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivitySlider.value = sensitivity;
        PlayerPrefs.SetFloat("Controls_MouseSensitivity", sensitivity);
        ControlBinding.instance.mouseSensitivity = sensitivity;

        PlayerPrefs.Save();
    }

    public void SetFreeLook(bool isOn)
    {
        PlayerPrefs.SetInt("Controls_Mouse_Freelook", isOn ? 1 : 0);
        ControlBinding.instance.alwaysFreeLook = isOn;

        PlayerPrefs.Save();
    }

    public void SetInvertYAxis(bool isOn)
    {
        PlayerPrefs.SetInt("Controls_Mouse_InvertYAxis", isOn ? 1 : 0);
        ControlBinding.instance.invertMouseYAxis = isOn;

        PlayerPrefs.Save();
    }

    #endregion

    #region Remap

    public void SetControlText()
    {
        cntr_cam_dwn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Cam_Down", "Down");
        cntr_cam_lft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Cam_Left", "Left");
        cntr_cam_rt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Cam_Right", "Right");
        cntr_cam_up.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Cam_Up", "Up");
        cntr_mrb_bak.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Marble_Back", "S");
        cntr_mrb_fw.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Marble_Forward", "W");
        cntr_mrb_jmp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Marble_Jump", "Space");
        cntr_mrb_lft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Marble_Left", "A");
        cntr_mrb_pwr.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Marble_Powerup", "Left Mouse Button");
        cntr_mrb_rt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Marble_Right", "D");
        cntrl_mous_bttn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = PlayerPrefs.GetString("Control_Mouse_Freelook", "Right Mouse Button");
    }

    private void Update()
    {
        if (remapMenu.activeSelf)
        {
            if (!Input.anyKeyDown)
                return;

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key) && key != KeyCode.Escape && key != KeyCode.Return)
                {
                    Debug.Log("Detected: " + Utils.KeyCodeToString(key));

                    if (ValidateInput(key))
                    {
                        ControlBinding.instance.AssignKey(bindToBeRemapped, key);
                        buttonToBeRemapped.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Utils.KeyCodeToString(key);

                        bindToBeRemapped = string.Empty;
                        buttonToBeRemapped = null;

                        remapMenu.SetActive(false);
                        Invoke(nameof(EnableButtons), 0.25f);
                    }
                    else
                    {
                        if(bindToBeRemapped == conflictedMapping)
                        {
                            remapMenu.SetActive(false);
                        }
                        else
                        {
                            tempKeycode = key;
                            remapMenu.SetActive(false);
                            confirmMenu.SetActive(true);
                            confirmMenu.transform.SetAsLastSibling();
                            confirmCaption.text = "\"" + Utils.KeyCodeToString(key) + "\" is already bound to \"" + conflictedMapping + "\"!\nDo you want to undo this mapping ?";
                        }
                    }
                    break;
                }
            }
        }
    }

    void CancelMapping()
    {
        conflictedButton = null;
        conflictedMapping = string.Empty;

        confirmMenu.SetActive(false);

        Invoke(nameof(EnableButtons), 0.25f);
    }

    void ForceMapping()
    {
        ControlBinding.instance.AssignKey(bindToBeRemapped, tempKeycode);
        ControlBinding.instance.AssignKey(conflictedMapping, KeyCode.None);

        buttonToBeRemapped.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Utils.KeyCodeToString(tempKeycode);
        conflictedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Utils.KeyCodeToString(KeyCode.None);

        bindToBeRemapped = string.Empty;
        buttonToBeRemapped = null;
        conflictedButton = null;
        conflictedMapping = string.Empty;
        tempKeycode = KeyCode.None;

        confirmMenu.SetActive(false);

        Invoke(nameof(EnableButtons), 0.25f);
    }

    bool ValidateInput(KeyCode key)
    {
        conflictedButton = null;
        conflictedMapping = string.Empty;

        if (key == ControlBinding.instance.moveForward)
        {
            conflictedButton = cntr_mrb_fw;
            conflictedMapping = "Move Forward";
            return false;
        }

        else if (key == ControlBinding.instance.moveBackward)
        {
            conflictedButton = cntr_mrb_bak;
            conflictedMapping = "Move Backward";
            return false;
        }

        else if (key == ControlBinding.instance.moveLeft)
        {
            conflictedButton = cntr_mrb_lft;
            conflictedMapping = "Move Left";
            return false;
        }

        else if (key == ControlBinding.instance.moveRight)
        {
            conflictedButton = cntr_mrb_rt;
            conflictedMapping = "Move Right";
            return false;
        }

        else if (key == ControlBinding.instance.rotateCameraUp)
        {
            conflictedButton = cntr_cam_up;
            conflictedMapping = "Rotate Camera Up";
            return false;
        }

        else if (key == ControlBinding.instance.rotateCameraRight)
        {
            conflictedButton = cntr_cam_rt;
            conflictedMapping = "Rotate Camera Down";
            return false;
        }

        else if (key == ControlBinding.instance.rotateCameraLeft)
        {
            conflictedButton = cntr_cam_lft;
            conflictedMapping = "Rotate Camera Left";
            return false;
        }

        else if (key == ControlBinding.instance.rotateCameraDown)
        {
            conflictedButton = cntr_cam_dwn;
            conflictedMapping = "Rotate Camera Right";
            return false;
        }

        else if (key == ControlBinding.instance.jump)
        {
            conflictedButton = cntr_mrb_jmp;
            conflictedMapping = "Jump";
            return false;
        }

        else if (key == ControlBinding.instance.usePowerup)
        {
            conflictedButton = cntr_mrb_pwr;
            conflictedMapping = "Use Powerup";
            return false;
        }

        else if (key == ControlBinding.instance.freelookKey)
        {
            conflictedButton = cntrl_mous_bttn;
            conflictedMapping = "Free-Look Key";
            return false;
        }


        return true;
    }

    void EnableButtons()
    {
        foreach (var button in FindObjectsOfType<Button>())
            button.interactable = true;
    }

    public void RemapButton(string _bindToBeRemapped, Button b)
    {
        foreach (var button in FindObjectsOfType<Button>())
            button.interactable = false;

        remapCaption.text = "Press a new key or button for \"" + _bindToBeRemapped + "\"";
        buttonToBeRemapped = b;
        bindToBeRemapped = _bindToBeRemapped;

        remapMenu.transform.SetAsLastSibling();
        remapMenu.SetActive(true);
    }

    #endregion
}

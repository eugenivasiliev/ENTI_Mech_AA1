using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlanetManager : MonoBehaviour
{

    private enum Planet
    {
        SUN = 0,
        MERCURY = 1,
        VENUS = 2,
        EARTH = 3,
        MARS = 4,
        JUPITER = 5,
        SATURN = 6,
        URANUS = 7,
        NEPTUNE = 8,
        COUNT = 9
    }

    public static PlanetManager Instance { get; private set; }

    [Header("Time attributes")]
    private bool paused = false;
    private float curTime = 0.0f;
    [SerializeField] private float stepTime = 0.0001f;
    [SerializeField] private float speedMult = 1.0f;
    public float StepTime {  get { return (paused) ? 0.0f : stepTime * speedMult; } }

    [Header("Planets")]
    private PlanetScript[] planets;
    public PlanetScript[] Planets { get { return planets; } }
    [SerializeField] Planet curPlanet = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text yearText;
    [SerializeField] private TMP_Text planetText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private UnityEngine.UI.Image pauseIcon;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float scroll = 5.0f;

    void Start()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        planets = GetComponentsInChildren<PlanetScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float displace = scroll * planets[(int)curPlanet].gameObject.transform.localScale.x;

        cam.transform.position = planets[(int)curPlanet].transform.position + Vector3.back * displace + Vector3.down * displace;
        cam.transform.LookAt(planets[(int)curPlanet].transform.position);

        if (Input.GetKeyDown(KeyCode.UpArrow)) speedMult += 1f;
        if (Input.GetKeyDown(KeyCode.DownArrow)) speedMult = Mathf.Max(1f, speedMult - 1f);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) curPlanet = (curPlanet > 0) ? curPlanet - 1 : Planet.COUNT - 1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) curPlanet = (Planet)(((int)curPlanet + 1) % (int)Planet.COUNT);
        if (Input.GetKeyDown(KeyCode.Space)) paused = !paused;
        if (Input.GetKeyDown(KeyCode.R)) Reset();
        pauseIcon.gameObject.SetActive(paused);
        scroll -= Input.mouseScrollDelta.y;
        scroll = Mathf.Clamp(scroll, 2.0f, 30.0f);

        curTime += stepTime * speedMult;
        yearText.text = "Year " + Mathf.Floor(curTime);
        planetText.text = "Looking at " + planets[(int)curPlanet].gameObject.name;
        speedText.text = "Speed = x" + speedMult.ToString("0.00");

    }

    private void Reset()
    {
        foreach (var planet in planets) planet.Reset();
        curTime = 0f;
        stepTime = 0.0001f;
        speedMult = 1.0f;
    }
}

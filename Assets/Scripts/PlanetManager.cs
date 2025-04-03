using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private Dictionary<Planet, string> planetNames = new Dictionary<Planet, string>{ 
        { Planet.SUN, "Sun" }, 
        { Planet.MERCURY, "Mercury" },
        { Planet.VENUS, "Venus" },
        { Planet.EARTH, "Earth" },
        { Planet.MARS, "Mars" },
        { Planet.JUPITER, "Jupiter" },
        { Planet.SATURN, "Saturn" },
        { Planet.URANUS, "Uranus" },
        { Planet.NEPTUNE, "Neptune" }
    };

    public static PlanetManager Instance { get; private set; }

    [Header("Time attributes")]
    private float curTime = 0.0f;
    [SerializeField] private float stepTime = 0.0001f;
    [SerializeField] private float speedMult = 1.0f;
    public float StepTime {  get { return stepTime * speedMult; } }

    private PlanetScript[] planets;
    public PlanetScript[] Planets { get { return planets; } }
    [SerializeField] private TMP_Text yearText;
    [SerializeField] private TMP_Text planetText;
    [SerializeField] private TMP_Text speedText;

    [SerializeField] Planet curPlanet = 0;

    void Start()
    {
        if (Instance != null && Instance != this) Destroy(this);
        Instance = this;

        planets = GetComponentsInChildren<PlanetScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float displace = 5.0f * planets[(int)curPlanet].gameObject.transform.localScale.x;

        this.transform.position = planets[(int)curPlanet].transform.position + Vector3.back * displace + Vector3.down * displace;
        this.transform.LookAt(planets[(int)curPlanet].transform.position);

        if (Input.GetKeyDown(KeyCode.UpArrow)) speedMult += 1f;
        if (Input.GetKeyDown(KeyCode.DownArrow)) speedMult = Mathf.Max(1f, speedMult - 1f);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) curPlanet = (curPlanet > 0) ? curPlanet - 1 : Planet.COUNT - 1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) curPlanet = (Planet)(((int)curPlanet + 1) % (int)Planet.COUNT);

        curTime += stepTime;
        yearText.text = "Year " + Mathf.Floor(curTime);
        planetText.text = "Looking at " + planetNames[curPlanet];
        speedText.text = "Speed = x" + speedMult.ToString("0.00");

    }
}

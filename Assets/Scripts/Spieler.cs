using UnityEngine;
using TMPro;
using System.Runtime.ExceptionServices;

public class Spieler : MonoBehaviour
{
    [Header("References")]
    public GameObject ziegelEinsPrefab;
    public GameObject ziegelZweiPrefab;
    public GameObject ziegelDreiPrefab;
    public GameObject ball;
    public Ball ballKlasse;

    [Header("Ball Settings")]
    Rigidbody2D ballRB;
    public bool ballUnterwegs = false;
    readonly float eingabeFaktor = 10;

    [Header("UI References")]
    public TextMeshProUGUI punkteAnzeige;
    public TextMeshProUGUI lebenAnzeige;
    public TextMeshProUGUI infoAnzeige;
    public TextMeshProUGUI zeitAnzeige;
    public TextMeshProUGUI zeitAltAnzeige;

    [Header("General Data")]
    public bool spielGestartet = false;
    public float spielZeitStart;

    void Start()
    {
        ballRB = ball.GetComponent<Rigidbody2D>();
        ZiegelErzeugen();
        ZeitAltLaden();
    }

    void ZiegelErzeugen()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Ziegel");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        for (int x = 1; x <= 10; x++)
        {
            for (int y = 1; y <= 5; y++)
            {
                int zufall = Random.Range(1, 4);
                GameObject ziegel = zufall switch
                {
                    1 => ziegelEinsPrefab,
                    2 => ziegelZweiPrefab,
                    _ => ziegelDreiPrefab
                };
                Instantiate(ziegel, new Vector3(x * 1.2f - 6.6f, y * 0.75f - 0.25f, 0), Quaternion.identity);

            }
        }
    }

    void ZeitAltLaden()
    {
        float zeitAlt = 0;
        if (PlayerPrefs.HasKey("zeitAlt")) zeitAlt = PlayerPrefs.GetFloat("zeitAlt");
        zeitAltAnzeige.text = string.Format("Alt: {0,6:0.0} sec.", zeitAlt);
    }

    void Update()
    {
        float xEingabe = Input.GetAxis("Horizontal");
        float yEingabe = Input.GetAxis("Vertical");

        if (!ballUnterwegs && yEingabe > 0)
        {
            Debug.Log("A");
            ballRB.AddForce(new Vector2(240, 160));
            ballUnterwegs = true;

            if (!spielGestartet)
            {
                spielGestartet = true;
                spielZeitStart = Time.time;
            }
            infoAnzeige.text = "";
        }

        if (ballUnterwegs)
        {
            float xNeu = transform.position.x + xEingabe * eingabeFaktor * Time.deltaTime;
            if (xNeu < -6) xNeu = -6;
            if (xNeu > 6) xNeu = 6;
            transform.position = new Vector3(xNeu, transform.position.y, 0);
        }

        if (spielGestartet)
        {
            zeitAnzeige.text = string.Format("Zeit: {0,6:0.0} sec.", Time.time - spielZeitStart);
        }
    }

    public void SpielNeuButton_Click()
    {
        if (spielGestartet) return;

        ballKlasse.anzahlPunkte = 0;
        ballKlasse.anzahlLeben = 5;

        punkteAnzeige.text = "Punkte: 0";
        lebenAnzeige.text = "Leben: 5";
        zeitAnzeige.text = "Zeit: 0.0 sec.";
        infoAnzeige.text = @"Schiesse den Ball mit Pfeil-Aufwärts ab. 
Bewege den schwarzen Spieler mit Pfeil-Links und Pfeil-Rechts.
Zerstöre Ziegel und meide den roten Boden.";

        ZeitAltLaden();
        ZiegelErzeugen();
        AufStartpunkt();
    }

    public void AufStartpunkt()
    {
        ballUnterwegs = false;
        gameObject.SetActive(true);
        transform.position = new Vector3(0, -4.55f, 0);

        ball.SetActive(true);
        ball.transform.position = new Vector3(0, -4.1f, 0);
    }

    public void AnwendungBeendenButton_Click()
    {
        if (!spielGestartet) Application.Quit();
    }
}

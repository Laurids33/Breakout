using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour
{
    public GameObject spieler;
    public AudioClip kollisionZiegelAudio;
    public int anzahlPunkte = 0;
    Rigidbody2D rb;
    public Spieler spielerKlasse;
    public int anzahlLeben = 5;

    [Header("UI References")]
    public TextMeshProUGUI punkteAnzeige;
    public TextMeshProUGUI lebenAnzeige;
    public TextMeshProUGUI infoAnzeige;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        GameObject anderesObjekt = coll.gameObject;

        if (anderesObjekt.CompareTag("Ziegel"))
        {
            AudioSource.PlayClipAtPoint(kollisionZiegelAudio, transform.position);
            anzahlPunkte++;
            punkteAnzeige.text = $"Punkte: {anzahlPunkte}";

            if (anzahlPunkte < 50)
            {
                Destroy(anderesObjekt, 0.1f);
                if (anzahlPunkte % 10 == 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x * 1.1f, rb.linearVelocity.y * 1.1f);
                    Debug.Log($"Punkte: {anzahlPunkte} Geschwindigkeit: {rb.linearVelocity.magnitude}");
                }
                
            }
            else
            {
                Destroy(anderesObjekt);
                spieler.SetActive(false);
                gameObject.SetActive(false);
                Debug.Log("Du hast gewonnen.");
                
                float spielZeitAktuell = Time.time - spielerKlasse.spielZeitStart;
                infoAnzeige.text = string.Format("Du hast gewonnen, in {0,6:0.0} sec.", spielZeitAktuell);
                PlayerPrefs.SetFloat("zeitAlt", spielZeitAktuell);
                PlayerPrefs.Save();

                spielerKlasse.spielGestartet = false;
                
            }
        }
        else if (anderesObjekt.CompareTag("BodenMitte"))
        {
            anzahlLeben--;
            lebenAnzeige.text = $"Leben: {anzahlLeben}";

            spieler.SetActive(false);
            gameObject.SetActive(false);
            spielerKlasse.ballUnterwegs = false;

            if (anzahlLeben >= 1)
            {
                Invoke(nameof(NaechstesLeben), 1);
                Debug.Log($"Leben: {anzahlLeben}");
                infoAnzeige.text = $"Du hast nur noch {anzahlLeben} Leben";
            }
            else
            {
                Debug.Log("Du hast verloren.");
                infoAnzeige.text = "Du hast verloren!";

                spielerKlasse.spielGestartet = false;
            }
        }
    }

    void NaechstesLeben()
    {
        infoAnzeige.text = "";
        spielerKlasse.AufStartpunkt();
    }
}

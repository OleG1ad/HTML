using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SiteProgressLoading : MonoBehaviour
{
    [SerializeField] private Image loadingImg;
    [SerializeField] private Image loadingImg_2;
    [SerializeField] private Image loadingImg_3;

    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text loadingText;

    private CustomSlider customSlider;
    [SerializeField] private GameObject MenuPanel;
    [SerializeField] private GameObject TumblerImage;

    private bool isTwoCircle;

    // animate the game object from -1 to +1 and back
    private float minimum = 0.0F;
    private float maximum = 1.0F;

    // starting value for the Lerp
    private static float progress = 0.0f;

    public float interpolater = 0.05f;

    public bool isAnimateProgressCircle;
    public bool isSmoothClosing;

    private void Start()
    {
        customSlider = TumblerImage.GetComponent<CustomSlider>();
        isSmoothClosing = false;
    }

    void Update()
    {
        if (isAnimateProgressCircle)
        {
            loadingImg.fillAmount = Mathf.Lerp(minimum, maximum, progress / 1f);            
            loadingImg_2.fillAmount = Mathf.Lerp(minimum, maximum, progress / 2f);            

            if (progress >= 0.75f)
            {
                loadingImg.CrossFadeAlpha(0, 0.5f, false); //плавное исчезновение
            }

            if (progress > 0f && progress < 0.25f)
            {
                loadingImg.CrossFadeAlpha(1, 0.5f, false); //плавное исчезновение
            }            

            loadingText.text = "Загрузка...";
            //progressText.text = string.Format("{0:0}%", progress * 100);

            // .. and increase the t interpolater
            progress += interpolater * Time.deltaTime;

            // now check if the interpolator has reached 1.0
            // and swap maximum and minimum so game object moves
            // in the opposite direction.

            if (progress >= 1.0f)
            {
                if (isTwoCircle)
                {
                    if (isSmoothClosing)
                    {
                        StartCoroutine(SmoothClosing());
                    }
                }
                isTwoCircle = true;
                progress = 0.0f;
            }
        }
    }

    public void ActiveProgressMenu()
    {
        progress = 0.0f;
        interpolater = 0.2f;
        isAnimateProgressCircle = true;

        TumblerImage.SetActive(true);
        customSlider = TumblerImage.GetComponent<CustomSlider>();
        MenuPanel.SetActive(true);
        customSlider.Open();
    }
    
    public void SkipProgressMenuPanel()
    {
        StartCoroutine(SmoothClosing());
    }

    private IEnumerator SmoothClosing()
    {
        isAnimateProgressCircle = false;
        customSlider.Close();
        yield return new WaitForSeconds(1f);
        TumblerImage.SetActive(false);
        MenuPanel.SetActive(false);
    }
}

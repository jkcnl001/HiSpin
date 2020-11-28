using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(CanvasGroup))]
public class Setting : MonoBehaviour, IUIBase
{
    public RectTransform top_baseRect;
    [Space(15)]
    public Image bgImage;
    public RectTransform panelRect;
    public Image head_iconImage;
    public Image lv_progressImage;
    public GameObject head_redpointGo;
    public Text ticket_multipleText;
    public Text lvText;
    public Text nameText;
    [Space(15)]
    public Button bgButton;
    public Button meButton;
    public Button withdrawButton;
    public Button tasksButton;
    public Button rulesButton;
    public Button soundButton;
    public Button musicButton;
    public Button emailButton;

    public new AnimationCurve animation;
    public GameObject task_rpGo;
    CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        panelStartPos = new Vector3(-1080 / 2 - panelRect.sizeDelta.x, 0);
        panelEndPos = new Vector3(-1080 / 2, 0);
        if (Master.IsBigScreen)
        {
            top_baseRect.sizeDelta += new Vector2(0, Master.TopMoveDownOffset);
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        bgImage.color = bgStartColor;
        panelRect.localPosition = panelStartPos;
        bgButton.AddClickEvent(OnBgClick);
        meButton.AddClickEvent(OnMeButtonClick);
        withdrawButton.AddClickEvent(OnWithdrawClick);
        tasksButton.AddClickEvent(OnTasksClick);
        rulesButton.AddClickEvent(OnRulesClick);
        soundButton.AddClickEvent(OnSoundClick);
        musicButton.AddClickEvent(OnMusicClick);
        emailButton.AddClickEvent(OnEmailClick);
        withdrawButton.gameObject.SetActive(Save.data.isPackB);
    }
    public void OnTaskFinishChange(bool hasFinish)
    {
        task_rpGo.SetActive(hasFinish);
    }
    private void OnBgClick()
    {
        UI.ClosePopPanel(this);
    }
    private void OnMeButtonClick()
    {
        UI.ClosePopPanel(this);
        UI.ShowBasePanel(BasePanel.Me);
    }
    private void OnWithdrawClick()
    {
        UI.ClosePopPanel(this);
        UI.ShowBasePanel(BasePanel.Cashout);
    }
    private void OnTasksClick()
    {
        UI.ClosePopPanel(this);
        UI.ShowBasePanel(BasePanel.Task);
    }
    private void OnRulesClick()
    {
        Application.OpenURL("http://luckyclub.vip/hispin-termofuse/");
    }
    private void OnSoundClick()
    {
        Save.data.sound_on = !Save.data.sound_on;
        Audio.SetSoundState(Save.data.sound_on);
        soundButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Setting, "sound_" + (Save.data.sound_on ? "on" : "off"));
    }
    private void OnMusicClick()
    {
        Save.data.music_on = !Save.data.music_on;
        Audio.SetMusicState(Save.data.music_on);
        musicButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Setting, "music_" + (Save.data.music_on ? "on" : "off"));
    }
    private void OnEmailClick()
    {
        SendEmail();
    }
    public static void SendEmail()
    {
        string userid;
        if (Save.data == null || Save.data.allData == null || Save.data.allData.user_panel == null || string.IsNullOrEmpty(Save.data.allData.user_panel.user_id))
            userid = "-";
        else
            userid = Save.data.allData.user_panel.user_id;
        string uuid;
        if (Save.data == null || string.IsNullOrEmpty(Save.data.uuid))
            uuid = "-";
        else
            uuid = Save.data.uuid;
        string adid;
        if (Save.data == null || string.IsNullOrEmpty(Save.data.adid))
            adid = "-";
        else
            adid = Save.data.adid;
        AndroidJavaClass javaClass = new AndroidJavaClass("com.Gradle.AndroidUtil");
        int androidVersion = javaClass.CallStatic<int>("GetAndroidVersion");
        string email = "hispin.support@luckyclub.vip";
        string subject = MyEscapeURL("Question from ID " +userid);
        string body = MyEscapeURL(string.Format("\n\n----------------------------------\nID:{0}\nUUID:{4}\nAdID:{5}\nVersion:{3}\nModel:{1}({2})\n----------------------------------\n", userid, SystemInfo.deviceModel, androidVersion.ToString(), Master.Version, uuid, adid));
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    static string MyEscapeURL(string url)
    {
        //%20是空格在url中的编码，这个方法将url中非法的字符转换成%20格式
        return WWW.EscapeURL(url).Replace("+", "%20");
    }
    #region state change
    Color bgStartColor = new Color32(0, 0, 0, 0);
    Color bgEndColor = new Color32(0, 0, 0, 204);
    Vector3 panelStartPos = Vector3.zero;
    Vector3 panelEndPos = Vector3.zero;
    public IEnumerator Show(params int[] args)
    {
        soundButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Setting, "sound_" + (Save.data.sound_on ? "on" : "off"));
        musicButton.image.sprite = Sprites.GetSprite(SpriteAtlas_Name.Setting, "music_" + (Save.data.music_on ? "on" : "off"));
        head_iconImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.HeadIcon, "head_" + Save.data.allData.user_panel.user_title);
        nameText.text = Save.data.allData.user_panel.user_name;
        ticket_multipleText.text = string.Format("Ticker <color=#fff000>x {0}</color>\nMultiplier ", Save.data.allData.user_panel.user_double.GetTicketMultipleString());
        lvText.text = "Lv." + Save.data.allData.user_panel.user_level;
        lv_progressImage.fillAmount = (float)Save.data.allData.user_panel.user_exp / Save.data.allData.user_panel.level_exp;
        head_redpointGo.SetActive(false);//检测是否有新头像
        int lv = Save.data.allData.user_panel.user_level;
        List<int> avatar_level_list = Save.data.allData.user_panel.title_level;
        List<bool> avatar_check = Save.data.head_icon_hasCheck;
        int count = avatar_level_list.Count;
        for (int i = 0; i < count; i++)
        {
            if (lv >= avatar_level_list[i] && !avatar_check[i])
            {
                head_redpointGo.SetActive(true);
                break;
            }
        }
        task_rpGo.SetActive(false);
        foreach (var task in Save.data.allData.lucky_schedule.user_task)
        {
            if (task.taskTargetId == PlayerTaskTarget.InviteAFriend)
                continue;
            if (task.task_cur >= task.task_tar && !task.task_receive)
            {
                task_rpGo.SetActive(true);
                break;
            }
        }

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        float startTime = animation[0].time;
        float endTime = animation[animation.length - 1].time;
        float timer = startTime;
        while (timer < endTime)
        {
            timer += Time.deltaTime;
            float value = animation.Evaluate(timer);
            panelRect.localPosition = Vector3.Lerp(panelStartPos, panelEndPos, value);
            bgImage.color = Color.Lerp(bgStartColor, bgEndColor, value);
            yield return null;
        }
    }
    public IEnumerator Close()
    {
        canvasGroup.interactable = false;
        float startTime = animation[0].time;
        float endTime = animation[animation.length - 1].time;
        float timer = endTime;
        while (timer > startTime)
        {
            timer -= Time.deltaTime;
            float value = animation.Evaluate(timer);
            panelRect.localPosition = Vector3.Lerp(panelStartPos, panelEndPos, value);
            bgImage.color = Color.Lerp(bgStartColor, bgEndColor, value);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void Pause()
    {
    }

    public void Resume()
    {
    }
    public void OnChangePackB()
    {
        withdrawButton.gameObject.SetActive(true);
    }

    #endregion
}

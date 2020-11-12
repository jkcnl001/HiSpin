using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Me : BaseUI
{
    public Image head_iconImage;
    public Image exp_progress_fillImage;
    public Text lvText;
    public InputField nameInputfield;
    public Text ticket_multipleText;
    public Button helpButton;
    [Space(15)]
    public Text current_levelText;
    public Text next_levelText;
    public Image lv_progress_fillImage;
    public Text lv_progress_desText;
    public Text current_ticket_multipleText;
    public Text next_ticket_multipleText;
    public Text level_up_reward_ticketText;
    protected override void Awake()
    {
        base.Awake();
        nameInputfield.onValueChanged.AddListener(OnInputNameValueChanged);
        nameInputfield.onEndEdit.AddListener(OnInputNameEnd);
        helpButton.AddClickEvent(OnHelpButtonClick);
    }
    private void OnInputNameValueChanged(string value)
    {
        nameInputfield.SetTextWithoutNotify(value.CheckName());
    }
    private void OnInputNameEnd(string value)
    {

    }
    private void OnHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.MyInfo);
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
    }
}

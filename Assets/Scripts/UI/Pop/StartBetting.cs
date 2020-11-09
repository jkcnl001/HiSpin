using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartBetting : PopUI
{
    public Text yesterday_ticket_numText;
    public RectTransform lightRect;
    public Text tipText;
    public RectTransform all_card_rootRect;
    public List<RectTransform> all_fly_cards;
    public CardItem single_card_item;
    public RectTransform fly_targetRect;
    public Button getButton;
    public Text get_button_contentText;
    protected override void Awake()
    {
        base.Awake();
        getButton.AddClickEvent(OnGetButtonClick);
        endPos = new Vector2(-473.27f-3, all_card_rootRect.localPosition.y);
    }
    private void OnGetButtonClick()
    {
        UI.ClosePopPanel(this);
    }
    Vector3 endPosOffset = new Vector2(-473.27f, 0);
    Vector3 endPos = new Vector2(-473.27f, 0);
    IEnumerator AutoSpinCards()
    {
        int yesterdayTicket = Save.data.betting_data.ysterday_tickets;
        yield return new WaitForSeconds(1);
        if (yesterdayTicket >= Betting.JoinNeedTicketNum)
        {
            Vector3 flyEndPos = fly_targetRect.position;
            Vector3 startPos = yesterday_ticket_numText.transform.position;
            Vector3 normalizeLength = (flyEndPos - startPos).normalized;
            int cardCount = all_fly_cards.Count;
            int startCardIndex = 0;
            int endCardIndex = 0;
            float flyInterval = 0.3f;
            float flyTimer = 0;
            float nextFlyTime = 0;
            while (endCardIndex < cardCount)
            {
                flyTimer += Time.deltaTime;
                yesterday_ticket_numText.text = ((int)(yesterdayTicket * Mathf.Clamp(((1.8f - flyTimer) / 1.8f), 0, 1))).GetTokenShowString();
                if (flyTimer >= nextFlyTime)
                {
                    startCardIndex++;
                    if (startCardIndex > cardCount)
                        startCardIndex = cardCount;
                    nextFlyTime += flyInterval;
                }
                for(int i = endCardIndex; i < startCardIndex; i++)
                {
                    all_fly_cards[i].Rotate(new Vector3(0, 0, Time.deltaTime * 100));
                    all_fly_cards[i].transform.position += normalizeLength * Time.deltaTime*1800;
                    if (Mathf.Abs(all_fly_cards[i].transform.position.x - flyEndPos.x) < 5f)
                    {
                        all_fly_cards[i].gameObject.SetActive(false);
                        Audio.PlayOneShot(AudioPlayArea.FlyOver);
                        endCardIndex++;
                    }
                }
                yield return null;
            }
        }
        float turn = 10;
        float speed = 0;
        float maxSpeed = 3000;
        all_card_rootRect.localPosition = new Vector3(0, all_card_rootRect.localPosition.y);
        while (turn > 0)
        {
            yield return null;
            speed += 50;
            if (speed > maxSpeed)
                speed = maxSpeed;
            if (turn > 1)
                all_card_rootRect.localPosition += new Vector3(-Time.deltaTime * speed, 0);
            else
            {
                all_card_rootRect.localPosition = Vector3.Lerp(all_card_rootRect.localPosition, endPos, Time.deltaTime);
                if (Vector3.Distance(all_card_rootRect.localPosition, endPos) < 10)
                {
                    break;
                }
            }
            if (all_card_rootRect.localPosition.x <= endPosOffset.x)
            {
                turn--;
                all_card_rootRect.localPosition -= 2 * endPosOffset;
            }
        }
        tipText.text = "More tickets you have Higher chance to win!";
        get_button_contentText.text = "TRY YOUR LUCK";
        List<BettingWinnerInfo> bettingWinners = Save.data.betting_data.ranking;
        string selfId = Save.data.mainData.user_id;
        BettingWinnerInfo willShow = bettingWinners[0];
        foreach(var winner in bettingWinners)
        {
            if (winner.user_id.Equals(selfId))
            {
                willShow = winner;
                tipText.text = "Congratulations on winning the prize!!";
                get_button_contentText.text = "TAKE YOUR MONEY!";
                break;
            }
        }
        single_card_item.Init(willShow.user_title, willShow.user_id, willShow.user_num);
        getButton.gameObject.SetActive(true);
    }
    IEnumerator AutoRatateLight()
    {
        while (true)
        {
            lightRect.Rotate(new Vector3(0, 0, Time.deltaTime * 10));
            yield return null;
        }
    }
    protected override void BeforeShowAnimation(params int[] args)
    {
        single_card_item.SetOff();
        int yesterdayTicket = Save.data.betting_data.ysterday_tickets;
        yesterday_ticket_numText.text = yesterdayTicket.GetTokenShowString();
        if (yesterdayTicket >= Betting.JoinNeedTicketNum)
            tipText.text = string.Format("You bet {0} tickets Good luck to you!", yesterdayTicket);
        else
            tipText.text = "More tickets you have Higher chance to win!";
        getButton.gameObject.SetActive(false);
        StartCoroutine(AutoRatateLight());
    }
    protected override void AfterShowAnimation(params int[] args)
    {
        StartCoroutine(AutoSpinCards());
    }
}

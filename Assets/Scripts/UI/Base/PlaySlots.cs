using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySlots : BaseUI
{
    public Image titleImage;
    public Button spinButton;
    public Text left_timeText;
    public Text cash_numText;
    public Image left_rewardImage;
    public Image mid_rewardImage;
    public Image right_rewardImage;
    [Space(15)]
    public GameObject exp_addGo;
    public Text expText;

    static readonly Dictionary<Reward, List<float>> type_offset_dic = new Dictionary<Reward, List<float>>()
    {
        {Reward.Gold,new List<float>(){ 0.31f, 0.43f, 0.55f, 0.69f, 0.82f} },
        {Reward.Ticket,new List<float>(){ 0.06f} },
        {Reward.Cash,new List<float>(){ 0.186f} },
    };

    static readonly SlotsRandomData[] emptyData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Null,weight=90,mustGetRange=null,numRnage=null,blackbox=null},
        new SlotsRandomData(){type=Reward.Null,weight=0,  mustGetRange=null,numRnage=null,blackbox=null}
    };
    static readonly SlotsRandomData[] goldData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Gold,weight=90,mustGetRange=new Range(1,2),numRnage=new Range(200,320),blackbox=null},
        new SlotsRandomData(){type=Reward.Gold,weight=90,mustGetRange=null,                   numRnage=new Range(100,300),blackbox=null}
    };
    static readonly SlotsRandomData[] ticketData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Ticket,weight=20,mustGetRange=new Range(0,1),numRnage=new Range(10,10),blackbox=new List<int>(){1,2,4,5 } },
        new SlotsRandomData(){type=Reward.Ticket,weight=10,mustGetRange=new Range(1,2),numRnage=new Range(10,10)}
    };
    static readonly SlotsRandomData[] cashData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Cash,weight=5,mustGetRange=new Range(0,1),numRnage=new Range(10,20),blackbox=new List<int>(){1,3,6,10 } },
        new SlotsRandomData(){type=Reward.Cash,weight=0,mustGetRange=null,numRnage=null}
    };

    protected override void Awake()
    {
        base.Awake();
        spinButton.AddClickEvent(OnSpinButtonClick);
    }
    bool isSpining = false;
    private void OnSpinButtonClick()
    {
        if (isSpining) return;
        isSpining = true;
        exp_addGo.SetActive(true);
        Master.Instance.AddLocalExp(exp_once);
        rewardType = RandomSlotsReward(out rewardNum);
        if (spinTime == 3)
        {
            Ads._instance.ShowInterstialAd(OnIVCallback, "老虎机第三次");
        }
        else
            OnIVCallback();
    }
    private void OnIVCallback()
    {
        StartCoroutine(StartSpinSlots());
    }
    private IEnumerator StartSpinSlots()
    {
        float endOffset_L, endOffset_M, endOffset_R;
        if (rewardType == Reward.Gold)
        {
            endOffset_L = endOffset_R = endOffset_M = type_offset_dic[Reward.Gold][Random.Range(0, type_offset_dic[Reward.Gold].Count)];
        }
        else if (rewardType == Reward.Ticket)
        {
            endOffset_L = endOffset_R = endOffset_M = type_offset_dic[Reward.Ticket][0];
        }
        else if (rewardType == Reward.Cash)
        {
            endOffset_L = endOffset_R = endOffset_M = type_offset_dic[Reward.Cash][0];
        }
        else
        {
            int Lindex = Random.Range(1, type_offset_dic[Reward.Gold].Count - 1);
            endOffset_L = type_offset_dic[Reward.Gold][Lindex];
            endOffset_R = type_offset_dic[Reward.Gold][Lindex+1];
            endOffset_M = type_offset_dic[Reward.Gold][Lindex-1];
        }
        #region newSpin
        float spinTime_L = 2;
        float spinTime_M = 3f;
        float spinTime_R = 4;
        float timer = 0;
        float spinSpeed;
        float backSpeed_L = 0.005f;
        float backSpeed_M = 0.005f;
        float backSpeed_R = 0.005f;
        float backTimer_L = 0;
        float backTimer_M = 0;
        float backTimer_R = 0;
        float startOffsetY_L = left_rewardImage.material.GetTextureOffset(MaterialOffsetProperty).y;
        float startOffsetY_M = mid_rewardImage.material.GetTextureOffset(MaterialOffsetProperty).y;
        float startOffsetY_R = right_rewardImage.material.GetTextureOffset(MaterialOffsetProperty).y;
        bool stop_L = false;
        bool back_L = false;
        bool stop_M = false;
        bool back_M = false;
        bool stop_R = false;
        bool back_R = false;
        AudioSource as_Spin = Audio.PlayLoop(AudioPlayArea.Spin);
        while (!stop_R || !stop_M || !stop_L)
        {
            yield return null;
            timer += Time.deltaTime * 2;
            spinSpeed = Time.deltaTime * 2.6f;
            startOffsetY_L += spinSpeed;
            startOffsetY_M += spinSpeed;
            startOffsetY_R += spinSpeed;
            if (!stop_L)
                if (timer < spinTime_L)
                    left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, startOffsetY_L));
                else
                {
                    if (!back_L)
                    {
                        backSpeed_L -= 0.0005f;
                        backTimer_L += backSpeed_L;
                        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_L + backTimer_L));
                        if (backSpeed_L <= 0)
                            back_L = true;
                    }
                    else
                    {
                        backSpeed_L -= 0.002f;
                        backTimer_L += backSpeed_L;
                        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_L + backTimer_L));
                        if (backTimer_L <= 0)
                        {
                            left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_L));
                            stop_L = true;
                        }
                    }
                }
            if (!stop_M)
                if (timer < spinTime_M)
                    mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, startOffsetY_M));
                else
                {
                    if (!back_M)
                    {
                        backSpeed_M -= 0.0005f;
                        backTimer_M += backSpeed_M;
                        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_M + backTimer_M));
                        if (backSpeed_M <= 0)
                            back_M = true;
                    }
                    else
                    {
                        backSpeed_M -= 0.002f;
                        backTimer_M += backSpeed_M;
                        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_M + backTimer_M));
                        if (backTimer_M <= 0)
                        {
                            mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_M));
                            stop_M = true;
                        }
                    }
                }
            if (!stop_R)
                if (timer < spinTime_R)
                    right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, startOffsetY_R));
                else
                {
                    if (!back_R)
                    {
                        backSpeed_R -= 0.0005f;
                        backTimer_R += backSpeed_R;
                        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_R + backTimer_R));
                        if (backSpeed_R <= 0)
                            back_R = true;
                    }
                    else
                    {
                        backSpeed_R -= 0.002f;
                        backTimer_R += backSpeed_R;
                        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_R + backTimer_R));
                        if (backTimer_R <= 0)
                        {
                            right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_R));
                            stop_R = true;
                        }
                    }
                }
        }
        as_Spin.Stop();
        as_Spin = null;
        #endregion
        yield return new WaitForSeconds(0.5f);
        switch (rewardType)
        {
            case Reward.Gold:
            case Reward.Ticket:
                UI.ShowPopPanel(PopPanel.GetReward, (int)rewardType, rewardNum, (int)GetRewardArea.PlaySlots);
                break;
            case Reward.Cash:
                UI.ShowPopPanel(PopPanel.GetCash, (int)GetCashArea.PlaySlots, rewardNum);
                break;
            case Reward.Null:
            default:
                if (spinTime >= MaxSpinTime)
                    UI.CloseCurrentBasePanel();
                break;
        }
        isSpining = false;
        exp_addGo.SetActive(false);
    }
    bool isPause = false;
    public override void Resume()
    {
        if (!isPause) return;
        isPause = false;
        if (spinTime >= MaxSpinTime)
            UI.CloseCurrentBasePanel();
    }
    public override void Pause()
    {
        isPause = true;
    }
    protected override void AfterCloseAnimation()
    {
        Master.Instance.SetBgDefault();
    }
    const string MaterialOffsetProperty = "_MainTex";
    const int MaxSpinTime = 5;
    static int isAd = 0;
    static int spinTime = 0;
    static int goldMustGetTime = -1;
    static int ticketMustGetTime = -1;
    static int cashMustGetTime = -1;
    static Reward rewardType = Reward.Null;
    static int rewardNum = 0;
    static int exp_once = 0;
    protected override void BeforeShowAnimation(params int[] args)
    {
        left_timeText.text = MaxSpinTime + "/" + MaxSpinTime;
        titleImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Slots, "title_" + args[0]);
        Master.Instance.ChangeBg(args[0]);
        isAd = args[1];
        cash_numText.text = "3 x     =     " + args[2].ToString();
        int totalExp = args[3];
        exp_once = totalExp / 5;
        expText.text = "+ Exp " + exp_once;

        spinTime = 0;
        int enterSlotsTotalTimes = Save.data.allData.user_panel.lucky_count + 1;
        List<int> goldBlackbox = goldData[isAd].blackbox;
        bool hasSetGoldBlackbox = false;
        if(goldBlackbox!=null)
            foreach (var time in goldBlackbox)
                if (time == enterSlotsTotalTimes)
                {
                    goldMustGetTime = goldData[isAd].mustGetRange.Max;
                    hasSetGoldBlackbox = true;
                    break;
                }
        if (!hasSetGoldBlackbox)
            goldMustGetTime = goldData[isAd].mustGetRange == null ? -1 : goldData[isAd].mustGetRange.RandomIncludeMax();

        List<int> ticketBlackbox = ticketData[0].blackbox;
        bool hasSetTicketBlackbox = false;
        if(ticketBlackbox!=null)
            foreach(var time in ticketBlackbox)
            {
                ticketMustGetTime = ticketData[isAd].mustGetRange.Max;
                hasSetTicketBlackbox = true;
                break;
            }
        if (!hasSetTicketBlackbox)
            ticketMustGetTime = ticketData[isAd].mustGetRange == null ? -1 : ticketData[isAd].mustGetRange.RandomIncludeMax();

        List<int> cashBlackbox = cashData[0].blackbox;
        bool hasSetCashblackbox = false;
        if(cashBlackbox!=null)
            foreach(var time in cashBlackbox)
            {
                cashMustGetTime = cashData[0].mustGetRange.Max;
                hasSetCashblackbox = true;
                break;
            }
        if (!hasSetCashblackbox)
            cashMustGetTime = cashData[isAd].mustGetRange == null ? -1 : cashData[isAd].mustGetRange.RandomIncludeMax();
        if (Save.data.allData.user_panel.lucky_total_cash >= 200)
            cashMustGetTime = 0;
        if (Save.data.allData.user_panel.user_gold_live >= Cashout.GoldMaxNum)
            goldMustGetTime = 0;
        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, type_offset_dic[Reward.Cash][0]));
        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, type_offset_dic[Reward.Cash][0]));
        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, type_offset_dic[Reward.Cash][0]));
    }
    struct RandomData
    {
        public bool hasThis;
        public Reward type;
        public int startWeight;
        public int endWeight;
    }
    private static List<RandomData> randomDatas = new List<RandomData>();
    private Reward RandomSlotsReward(out int rewardNum)
    {
        SlotsRandomData empty = emptyData[isAd];
        SlotsRandomData gold = goldData[isAd];
        SlotsRandomData ticket = ticketData[ticketMustGetTime <= -1 ? isAd : 0];
        SlotsRandomData cash = cashData[cashMustGetTime <= -1 ? isAd : 0];

        Reward RewardTicket(out int num)
        {
            num = ticket.numRnage.RandomIncludeMax();
            ticketMustGetTime--;
            spinTime++;
            left_timeText.text = (MaxSpinTime - spinTime) + "/" + MaxSpinTime;
            return Reward.Ticket;
        }
        Reward RewardGold(out int num)
        {
            num = gold.numRnage.RandomIncludeMax();
            goldMustGetTime--;
            spinTime++;
            left_timeText.text = (MaxSpinTime - spinTime) + "/" + MaxSpinTime;
            return Reward.Gold;
        }
        Reward RewardNull(out int num)
        {
            num = 0;
            spinTime++;
            left_timeText.text = (MaxSpinTime - spinTime) + "/" + MaxSpinTime;
            return Reward.Null;
        }
        Reward RewardCash(out int num)
        {
            num = cash.numRnage.RandomIncludeMax();
            cashMustGetTime--;
            spinTime++;
            left_timeText.text = (MaxSpinTime - spinTime) + "/" + MaxSpinTime;
            return Reward.Cash;
        }

        randomDatas.Clear();
        int total = 0;
        RandomData cashRandomData = new RandomData() { hasThis = false, type = Reward.Cash };
        if (cashMustGetTime > 0)
            cashRandomData.hasThis = true;
        else if (cashMustGetTime <= -1)
            cashRandomData.hasThis = MaxSpinTime - spinTime > (goldMustGetTime <= -1 ? 0 : goldMustGetTime) + (ticketMustGetTime <= -1 ? 0 : ticketMustGetTime);
        if (cashRandomData.hasThis)
        {
            cashRandomData.startWeight = total;
            total += cash.weight;
            cashRandomData.endWeight = total;
        }

        RandomData ticketRandomData = new RandomData() { hasThis = false, type = Reward.Ticket };
        if (ticketMustGetTime > 0)
            ticketRandomData.hasThis = true;
        else if(ticketMustGetTime<=-1)
            ticketRandomData.hasThis=MaxSpinTime-spinTime> (goldMustGetTime <= -1 ? 0 : goldMustGetTime) + (cashMustGetTime <= -1 ? 0 : cashMustGetTime);
        if (ticketRandomData.hasThis)
        {
            ticketRandomData.startWeight = total;
            total += ticket.weight;
            ticketRandomData.endWeight = total;
        }

        RandomData goldRandomData = new RandomData() { hasThis = false, type = Reward.Gold };
        if (goldMustGetTime > 0)
            goldRandomData.hasThis = true;
        else if (goldMustGetTime <= -1)
            goldRandomData.hasThis = MaxSpinTime - spinTime > (ticketMustGetTime <= -1 ? 0 : ticketMustGetTime) + (cashMustGetTime <= -1 ? 0 : cashMustGetTime);
        if (goldRandomData.hasThis)
        {
            goldRandomData.startWeight = total;
            total += gold.weight;
            goldRandomData.endWeight = total;
        }

        RandomData emptyRandomData = new RandomData() { hasThis = false, type = Reward.Null };
        emptyRandomData.hasThis = MaxSpinTime - spinTime > (ticketMustGetTime <= -1 ? 0 : ticketMustGetTime) + (cashMustGetTime <= -1 ? 0 : cashMustGetTime) + (goldMustGetTime <= -1 ? 0 : goldMustGetTime);
        if (emptyRandomData.hasThis)
        {
            emptyRandomData.startWeight = total;
            total += empty.weight;
            emptyRandomData.endWeight = total;
        }

        randomDatas.Add(cashRandomData);
        randomDatas.Add(ticketRandomData);
        randomDatas.Add(goldRandomData);
        randomDatas.Add(emptyRandomData);

        int result = Random.Range(0, total);
        foreach(var data in randomDatas)
        {
            if (!data.hasThis) continue;
            if (result >= data.startWeight && result < data.endWeight)
            {
                switch (data.type)
                {
                    case Reward.Null:
                        return RewardNull(out rewardNum);
                    case Reward.Gold:
                        return RewardGold(out rewardNum);
                    case Reward.Cash:
                        return RewardCash(out rewardNum);
                    case Reward.Ticket:
                        return RewardTicket(out rewardNum);
                    default:
                        return RewardNull(out rewardNum);
                }
            }
        }
        return RewardNull(out rewardNum);
    }
    private struct SlotsRandomData
    {
        public Reward type;
        public int weight;
        public Range mustGetRange;
        public Range numRnage;
        public List<int> blackbox;
    }
}

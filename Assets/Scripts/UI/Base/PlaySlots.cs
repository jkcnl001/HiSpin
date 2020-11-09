using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySlots : BaseUI
{
    public Image titleImage;
    public Button spinButton;
    public Button helpButton;
    public Text left_timeText;
    public Image left_rewardImage;
    public Image mid_rewardImage;
    public Image right_rewardImage;
    static readonly Dictionary<Reward, List<float>> type_offset_dic = new Dictionary<Reward, List<float>>()
    {
        {Reward.Gold,new List<float>(){ 0.335f,0.45f,0.58f,0.71f,0.85f} },
        {Reward.Ticket,new List<float>(){ 0.08f} },
        {Reward.Cash,new List<float>(){ 0.22f} },
    };

    static readonly SlotsRandomData[] emptyData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Null,weight=90,mustGetRange=null,numRnage=null},
        new SlotsRandomData(){type=Reward.Null,weight=0,  mustGetRange=null,numRnage=null}
    };
    static readonly SlotsRandomData[] goldData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Gold,weight=90,mustGetRange=new Range(1,2),numRnage=new Range(200,320)},
        new SlotsRandomData(){type=Reward.Gold,weight=90,mustGetRange=null,                   numRnage=new Range(100,300)}
    };
    static readonly SlotsRandomData[] ticketData = new SlotsRandomData[2]
    {
        new SlotsRandomData(){type=Reward.Ticket,weight=20,mustGetRange=new Range(0,1),numRnage=new Range(10,10)},
        new SlotsRandomData(){type=Reward.Ticket,weight=10,mustGetRange=new Range(1,2),numRnage=new Range(10,10)}
    };

    protected override void Awake()
    {
        base.Awake();
        spinButton.AddClickEvent(OnSpinButtonClick);
        helpButton.AddClickEvent(OnHelpButtonClick);
    }
    bool isSpining = false;
    private void OnSpinButtonClick()
    {
        if (isSpining) return;
        isSpining = true;
        rewardType = RandomSlotsReward(out rewardNum);
        StartCoroutine(StartSpinSlots());
    }
    private void OnHelpButtonClick()
    {
        UI.ShowPopPanel(PopPanel.Rules, (int)RuleArea.PlaySlots);
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
        else
        {
            int Lindex = Random.Range(1, type_offset_dic[Reward.Gold].Count - 1);
            endOffset_L = type_offset_dic[Reward.Gold][Lindex];
            endOffset_R = type_offset_dic[Reward.Gold][Lindex+1];
            endOffset_M = type_offset_dic[Reward.Gold][Lindex-1];
        }
        AudioSource tempAs = Audio.PlayLoop(AudioPlayArea.Spin);
        float spinRotateTime = 1;
        Vector2 leftOffset = left_rewardImage.material.GetTextureOffset(MaterialOffsetProperty);
        Vector2 rightOffset = right_rewardImage.material.GetTextureOffset(MaterialOffsetProperty);
        Vector2 midOffset = mid_rewardImage.material.GetTextureOffset(MaterialOffsetProperty);
        while (spinRotateTime > 0)
        {
            spinRotateTime -= Time.deltaTime;
            Vector2 o = new Vector2(0, Time.deltaTime*4);
            leftOffset += o;
            rightOffset += o;
            midOffset += o;
            left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, leftOffset);
            right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, rightOffset);
            mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, midOffset);
            yield return null;
        }
        tempAs.Stop();
        tempAs = null;
        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_L));
        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_R));
        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_M));
        yield return null;
        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_L + 0.02f));
        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_R + 0.02f));
        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_M + 0.02f));
        yield return new WaitForSeconds(0.04f);
        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_L));
        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_R));
        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, endOffset_M));
        yield return new WaitForSeconds(0.5f);
        switch (rewardType)
        {
            case Reward.Gold:
            case Reward.Ticket:
                UI.ShowPopPanel(PopPanel.GetReward, (int)rewardType, rewardNum, (int)GetRewardArea.PlaySlots);
                break;
            case Reward.Null:
            default:
                if (spinTime == MaxSpinTime)
                    UI.CloseCurrentBasePanel();
                break;
        }
        isSpining = false;
    }
    bool isPause = false;
    public override void Resume()
    {
        if (!isPause) return;
        isPause = false;
        if (spinTime == MaxSpinTime)
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
    static Reward rewardType = Reward.Null;
    static int rewardNum = 0;
    protected override void BeforeShowAnimation(params int[] args)
    {
        left_timeText.text = MaxSpinTime + "/" + MaxSpinTime;
        titleImage.sprite = Sprites.GetSprite(SpriteAtlas_Name.Slots, "title_" + args[0]);
        Master.Instance.ChangeBg(args[0]);
        isAd = args[1];
        spinTime = 0;
        goldMustGetTime = goldData[isAd].mustGetRange == null ? -1 : goldData[isAd].mustGetRange.RandomIncludeMax();
        ticketMustGetTime = ticketData[isAd].mustGetRange == null ? -1 : ticketData[isAd].mustGetRange.RandomIncludeMax();
        left_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, type_offset_dic[Reward.Cash][0]));
        right_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, type_offset_dic[Reward.Cash][0]));
        mid_rewardImage.material.SetTextureOffset(MaterialOffsetProperty, new Vector2(0, type_offset_dic[Reward.Cash][0]));
    }
    private Reward RandomSlotsReward(out int rewardNum)
    {
        SlotsRandomData empty = emptyData[isAd];
        SlotsRandomData gold = goldData[isAd];
        SlotsRandomData ticket = ticketData[isAd];

        bool hasEmpty = true;
        bool hasGold = true;
        bool hasTicket = true;

        if (goldMustGetTime != -1 && ticketMustGetTime != -1)
        {
            hasEmpty = MaxSpinTime - spinTime > goldMustGetTime + ticketMustGetTime;
            hasGold = goldMustGetTime > 0;
            hasTicket = ticketMustGetTime > 0;
        }
        else if (goldMustGetTime == -1 && ticketMustGetTime != -1)
        {
            hasEmpty = MaxSpinTime - spinTime > ticketMustGetTime;
            hasGold = hasEmpty;
            hasTicket = ticketMustGetTime > 0;
        }
        else if (goldMustGetTime != -1 && ticketMustGetTime == -1)
        {
            hasEmpty = MaxSpinTime - spinTime > goldMustGetTime;
            hasTicket = hasEmpty;
            hasGold = goldMustGetTime > 0;
        }

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

        if (hasTicket && !hasGold && !hasEmpty)
        {
            return RewardTicket(out rewardNum);
        }
        else if (hasGold && !hasTicket && !hasEmpty)
        {
            return RewardGold(out rewardNum);
        }
        else if (hasEmpty && !hasGold && !hasTicket)
        {
            return RewardNull(out rewardNum);
        }

        if (hasTicket && hasGold && !hasEmpty)
        {
            return Random.Range(0, ticket.weight + gold.weight) < ticket.weight ? RewardTicket(out rewardNum) : RewardGold(out rewardNum);
        }
        else if (hasTicket && hasEmpty && !hasGold)
        {
            return Random.Range(0, ticket.weight + empty.weight) < ticket.weight ? RewardTicket(out rewardNum) : RewardNull(out rewardNum);
        }
        else if (hasGold && hasEmpty && !hasTicket)
        {
            return Random.Range(0, gold.weight + empty.weight) < gold.weight ? RewardGold(out rewardNum) : RewardNull(out rewardNum);
        }

        int total = empty.weight + gold.weight + ticket.weight;
        int result = Random.Range(0, total);
        if (result < empty.weight)
        {
            return RewardNull(out rewardNum);
        }
        else if (result < empty.weight + gold.weight)
        {
            return RewardGold(out rewardNum);
        }
        else
        {
            return RewardTicket(out rewardNum);
        }
    }
    private struct SlotsRandomData
    {
        public Reward type;
        public int weight;
        public Range mustGetRange;
        public Range numRnage;
    }
}

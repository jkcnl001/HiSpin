﻿using System.Collections;
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
    static readonly Dictionary<Reward, List<float>> type_offset_dic = new Dictionary<Reward, List<float>>()
    {
        {Reward.Gold,new List<float>(){ 0.31f, 0.43f, 0.55f, 0.69f, 0.82f} },
        {Reward.Ticket,new List<float>(){ 0.06f} },
        {Reward.Cash,new List<float>(){ 0.186f} },
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
    }
    bool isSpining = false;
    private void OnSpinButtonClick()
    {
        if (isSpining) return;
        isSpining = true;
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
        cash_numText.text = "3 x     =     " + args[2].ToString();
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettingsScript : MonoBehaviour
{

    public static GameSettingsScript instance;
    #region singleton
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    #endregion
    public static GameSettingsScript createNew(GameObject where, GameSettingsScript inst = null)
    {
        GameSettingsScript temp = instance;
        instance = null;
        where.AddComponent<GameSettingsScript>();
        if (inst != null) instance.setAll(inst);
        else instance.setAll(temp);
        return instance;
    }
    public void setAll(GameSettingsScript ALL)
    {
        if (ALL == null) return;
        tickDelayCombat = ALL.tickDelayCombat;
        noTimer = ALL.noTimer;
        backTicks = ALL.backTicks;
        tickMode = ALL.tickMode;
        TimeLimitSnake = ALL.TimeLimitSnake;
        spacingSnake = ALL.spacingSnake;  // space between cubes
        tickDelaySnake = ALL.tickDelaySnake;
        easyModeSnake = ALL.easyModeSnake;
        easyModeEXP = ALL.easyModeEXP;

        endCombatNow = ALL.endCombatNow;
        clickCombat = ALL.clickCombat;
        deselectCombat = ALL.deselectCombat;
        tickNowCombat = ALL.tickNowCombat;
        backTickCombat = ALL.backTickCombat;
        selectorRangeCombat = ALL.selectorRangeCombat;
    }

    public void setTickDelayCombat(float tdelay) { tickDelayCombat = tdelay; }
    public void setNoTimer(bool yes) { noTimer = yes; }
    public void setBackTicks(int BT) { backTicks = BT; }
    public void setTickMode(int TM)
    {
        switch (TM)
        {
            case (int)TickManager.TickMode.Chaos:
                tickMode = TickManager.TickMode.Chaos;
                break;
            //case (int)TickManager.TickMode.Team:
            //    tickMode = TickManager.TickMode.Team;
            //    break;
            case (int)TickManager.TickMode.Initiative:
                tickMode = TickManager.TickMode.Initiative;
                break;
            //case (int)TickManager.TickMode.Initiative2:
            //    tickMode = TickManager.TickMode.Initiative2
            //    break;
        }
    }
    public void setTimeLimitSnake(float tlimit) { TimeLimitSnake = tlimit; }
    public void setSpacingSnake(float spacing) { spacingSnake = spacing; }
    public void setTickDelaySnake(float tdelay) { tickDelaySnake = tdelay; }
    public void seteasyModeSnake(bool yes) { easyModeSnake = yes; }
    public void setEasyModeEXP(bool yes) { easyModeEXP = yes; }
    public void setSelectorRangeCombat(float range) { selectorRangeCombat = range; }

    [Range(0.25f, 15f)] public float tickDelayCombat = 3f;
    public bool noTimer = false;
    public int backTicks = 5;
    public TickManager.TickMode tickMode = TickManager.TickMode.Chaos;
    [Range(5f, 60f)] public float TimeLimitSnake = 20f;
    public float spacingSnake = .25f;  // space between cubes
    public float tickDelaySnake = 1f;
    public bool easyModeSnake = false;
    public bool easyModeEXP = false;

    public KeyCode endCombatNow = KeyCode.Colon;
    public KeyCode clickCombat = KeyCode.Mouse0;
    public KeyCode deselectCombat = KeyCode.F;
    public KeyCode tickNowCombat = KeyCode.Space;
    public KeyCode backTickCombat = KeyCode.LeftShift;
    public float selectorRangeCombat = 10f;

    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}

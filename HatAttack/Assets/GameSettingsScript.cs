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

    [Range(0.25f, 15f)] public float tickDelayCombat = 3f;
    [SerializeField] public bool noTimer = false;
    [SerializeField] public int backTicks = 5;
    [SerializeField] public TickManager.TickMode tickMode = TickManager.TickMode.Chaos;
    [Range(5f, 60f)] public float TimeLimitSnake = 20f;
    public float spacingSnake = .25f;  // space between cubes
    public float tickDelaySnake = 3f;
    public bool easyModeSnake = false;
    public bool easyModeEXP = false;

    [SerializeField] public KeyCode endCombatNow = KeyCode.Colon;
    [SerializeField] public KeyCode clickCombat = KeyCode.Mouse0;
    [SerializeField] public KeyCode deselectCombat = KeyCode.F;
    [SerializeField] public KeyCode tickNowCombat = KeyCode.Space;
    [SerializeField] public KeyCode backTickCombat = KeyCode.LeftShift;
    [SerializeField] public float selectorRangeCombat = 100f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

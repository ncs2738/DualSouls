using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    static AudioSource audioSource;

    //================ GENERAL ========================
    [SerializeField] static AudioClip ButtonClick;

    //================ ATTACKING ========================
    [SerializeField] static AudioClip AttackStart;
    [SerializeField] static AudioClip AttackDraw;

    [SerializeField] static AudioClip AttackFire;
    [SerializeField] static AudioClip AttackGrass;
    [SerializeField] static AudioClip AttackWater;

    //================ GAME SFX ========================
    [SerializeField] static AudioClip CaptureFortress;
    [SerializeField] static AudioClip WinGame;

    //================ UNITS ========================
    [SerializeField] static AudioClip UnitTeleport;
    [SerializeField] static AudioClip UnitMove;
    [SerializeField] static AudioClip UnitGetKey;

    //================ DRAGON ========================
    [SerializeField] static AudioClip DragonSelect;
    [SerializeField] static AudioClip DragonUnselect;
    [SerializeField] static AudioClip DragonSummon;

    //================ THIEF ========================
    [SerializeField] static AudioClip ThiefSelect;
    [SerializeField] static AudioClip ThiefUnselect;
    [SerializeField] static AudioClip ThiefSummon;

    //================ WARRIOR ========================
    [SerializeField] static AudioClip WarriorSelect;
    [SerializeField] static AudioClip WarriorUnselect;
    [SerializeField] static AudioClip WarriorSummon;

    //================ WIZARD ========================
    [SerializeField] static AudioClip WizardSelect;
    [SerializeField] static AudioClip WizardUnselect;
    [SerializeField] static AudioClip WizardSummon;

    //================ MUSIC ========================
    [SerializeField] static AudioClip GameTheme1;
    [SerializeField] static AudioClip GameTheme2;
    [SerializeField] static AudioClip GameTheme3;
    [SerializeField] static AudioClip GameTheme4;
    [SerializeField] static AudioClip GameTheme5;
    [SerializeField] static AudioClip GameTheme6;
    [SerializeField] static AudioClip GameTheme7;
    [SerializeField] static AudioClip MenuTheme;

    AudioClip[] GameSongs;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    audioSource = GetComponent<AudioSource>();

    ButtonClick = Resources.Load<AudioClip>("Audio/ClickAnyButton");

    //================ ATTACKING ========================
    AttackStart = Resources.Load<AudioClip>("Audio/AttackStart");
    AttackDraw = Resources.Load<AudioClip>("Audio/AttackDraw");

    AttackFire = Resources.Load<AudioClip>("Audio/AttackFire");
    AttackGrass = Resources.Load<AudioClip>("Audio/AttackGrass");
    AttackWater = Resources.Load<AudioClip>("Audio/AttackWater");

    //================ GAME SFX ========================
    CaptureFortress = Resources.Load<AudioClip>("Audio/CaptureFortress");
    WinGame = Resources.Load<AudioClip>("Audio/WinGame");

    //================ UNITS ========================
    UnitTeleport = Resources.Load<AudioClip>("Audio/UnitTeleport");
    UnitMove = Resources.Load<AudioClip>("Audio/UnitMove");
    UnitGetKey = Resources.Load<AudioClip>("Audio/key2_pickup");

    //================ DRAGON ========================
    DragonSelect = Resources.Load<AudioClip>("Audio/UnitDragonSelect");
    DragonUnselect = Resources.Load<AudioClip>("Audio/UnitDragonUnselect");
    DragonSummon = Resources.Load<AudioClip>("Audio/UnitDragonSummon");

    //================ THIEF ========================
    ThiefSelect = Resources.Load<AudioClip>("Audio/UnitThiefSelect");
    ThiefUnselect = Resources.Load<AudioClip>("Audio/UnitThiefUnselect");
    ThiefSummon = Resources.Load<AudioClip>("Audio/UnitThiefSummon");

    //================ WARRIOR ========================
    WarriorSelect = Resources.Load<AudioClip>("Audio/UnitWarriorSelect");
    WarriorUnselect = Resources.Load<AudioClip>("Audio/UnitWarriorUnselect");
    WarriorSummon = Resources.Load<AudioClip>("Audio/UnitWarriorSummon");

    //================ WIZARD ========================
    WizardSelect = Resources.Load<AudioClip>("Audio/UnitWizardSelect");
    WizardUnselect = Resources.Load<AudioClip>("Audio/UnitWizardUnselect");
    WizardSummon = Resources.Load<AudioClip>("Audio/UnitWizardSummon");

    //================ MUSIC ========================
    GameTheme1 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Into-Battle");
    GameTheme2 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Pirates");
    GameTheme3 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Ancient-Crusades");
    GameTheme4 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Ancient-Troops-Amassing");
    GameTheme5 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Defending-the-Princess-Haunted");
    GameTheme6 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Fantasy-Forest-Battle");
    GameTheme7 = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Forward-Assault");
    MenuTheme = Resources.Load<AudioClip>("Audio/BKGMUS_NL_Tower-Defense");


    GameSongs = new AudioClip[] { GameTheme1, GameTheme2, GameTheme3, GameTheme4, GameTheme5, GameTheme6, GameTheme7 };
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
        audioSource.PlayOneShot(GameSongs[Random.Range(0, GameSongs.Length)]);
        }
    }

    //================ ATTACKING ========================

    public void PlayAttackStartSound()
    {
        audioSource.PlayOneShot(AttackStart);
    }

    public void PlayAttackDrawSound()
    {
        audioSource.PlayOneShot(AttackDraw);
    }

    public void PlayAttackFireSound()
    {
        audioSource.PlayOneShot(AttackFire);
    }   

    public void PlayAttackWaterSound()
    {
        audioSource.PlayOneShot(AttackWater);
    }

    public void PlayAttackGrassSound()
    {
        audioSource.PlayOneShot(AttackGrass);
    }


    //================ GAME SFX ========================

    public void PlayCaptureFortressSound()
    {
        audioSource.PlayOneShot(CaptureFortress);
    }

    public void PlayWinGame()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(WinGame);   
    }

    //================ UNITS ========================

    public void PlayUnitTeleportSound()
    {
        audioSource.PlayOneShot(UnitTeleport);
    }

    public void PlayUnitMoveSound()
    {
        audioSource.PlayOneShot(UnitMove);
    }

    public void PlayUnitGetKeySound()
    {
        audioSource.PlayOneShot(UnitGetKey);
    }

    //================ DRAGON ========================
    public void PlayDragonSelectSound()
    {
        audioSource.PlayOneShot(DragonSelect);
    }

    public void PlayDragonUnselectSound()
    {
        audioSource.PlayOneShot(DragonUnselect);
    }

    public void PlayDragonSummonSound()
    {
        audioSource.PlayOneShot(DragonSummon);
    }

    //================ THIEF ========================
    public void PlayThiefSelectSound()
    {
        audioSource.PlayOneShot(ThiefSelect);
    }

    public void PlayThiefUnselectSound()
    {
        audioSource.PlayOneShot(ThiefUnselect);
    }

    public void PlayThiefSummonSound()
    {
        audioSource.PlayOneShot(ThiefSummon);
    }

    //================ WARRIOR ========================
    public void PlayWarriorSelectSound()
    {
        audioSource.PlayOneShot(WarriorSelect);
    }

    public void PlayWarriorUnselectSound()
    {
        audioSource.PlayOneShot(WarriorUnselect);
    }

    public void PlayWarriorSummonSound()
    {
        audioSource.PlayOneShot(WarriorSummon);
    }

    //================ WIZARD ========================
    public void PlayWizardSelectSound()
    {
        audioSource.PlayOneShot(WizardSelect);
    }   

    public void PlayWizardUnselectSound()
    {
        audioSource.PlayOneShot(WizardUnselect);
    }

    public void PlayWizardSummonSound()
    {
        audioSource.PlayOneShot(WizardSummon);
    }

    //================ MUSIC ========================
    public void PlayGameTheme()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.PlayOneShot(GameSongs[Random.Range(0, GameSongs.Length)]);
        audioSource.loop = false;
        audioSource.volume = .25f;
    }

    public void PlayMenuTheme()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        audioSource.PlayOneShot(MenuTheme);
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = .25f;
    }
}
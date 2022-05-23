using OWML.ModHelper;
using OWML.Common;
using UnityEngine;

namespace HatchlingOutfit
{
    public class HatchlingOutfit : ModBehaviour
    {
        // Config vars
        string bodySetting, armsSetting, headSetting, jetpackSetting, suitOneArm;
        bool missingBody, missingHead, missingLArm, missingRArm;

        // Mod vars
        public static HatchlingOutfit Instance;
        PlayerCharacterController characterController;
        PlayerAnimController animController;
        GameObject suitlessModel, suitlessBody, suitlessHead, suitlessLArm, suitlessRArm, suitlessHeadShader, suitlessRArmShader,
                   suitModel, suitBody, suitHead, suitLArm, suitRArm, suitHeadShader, suitRArmShader, suitJetpack, suitJetpackFX;

        public override void Configure(IModConfig config)
        {
            base.Configure(config);
            bodySetting = config.GetSettingsValue<string>("Body");
            armsSetting = config.GetSettingsValue<string>("Arms");
            headSetting = config.GetSettingsValue<string>("Head");
            jetpackSetting = config.GetSettingsValue<string>("Jetpack");
            suitOneArm = config.GetSettingsValue<string>("Only Suit One Arm");
            missingBody = config.GetSettingsValue<bool>("Missing Body");
            missingHead = config.GetSettingsValue<bool>("Missing Head");
            missingLArm = config.GetSettingsValue<bool>("Missing Left Arm");
            missingRArm = config.GetSettingsValue<bool>("Missing Right Arm");
            ChangeOutfit();
        }

        private void Awake()
        {
            // Static reference to mod so it can be used in patches
            Instance = this;
        }

        void Start()
        {
            // Add the patches
            ModHelper.HarmonyHelper.AddPostfix<PlayerAnimController>(
                "Start", typeof(Patches), nameof(Patches.PlayerAnimControllerAwake));
            ModHelper.HarmonyHelper.AddPostfix<PlayerCharacterController>(
                "OnSuitUp", typeof(Patches), nameof(Patches.SuitChanged));
            ModHelper.HarmonyHelper.AddPostfix<PlayerCharacterController>(
                "OnRemoveSuit", typeof(Patches), nameof(Patches.SuitChanged));
            ModHelper.Console.WriteLine($"Hatchling is ready to dazzle({nameof(HatchlingOutfit)} is ready)", MessageType.Success);
        }

        public void SetVars()
        {
            // Make sure that the scene is the SS or Eye
            if (WrongScene()) return;
            // Cancel otherwise

            // Temporary vars
            OWRigidbody playerBody = Locator.GetPlayerBody();
            GameObject playerModel = playerBody.transform.Find("Traveller_HEA_Player_v2").gameObject;

            // Get permanent vars and separately grab the suitless and suited models
            characterController = Locator.GetPlayerController();
            animController = characterController.GetComponent<PlayerAnimController>();
            suitlessModel = playerModel.transform.Find("player_mesh_noSuit:Traveller_HEA_Player").gameObject;
            suitModel = playerModel.transform.Find("Traveller_Mesh_v01:Traveller_Geo").gameObject;
            suitJetpackFX = playerBody.transform.Find("PlayerVFX").gameObject;

            // Get all individual parts for suitless
            string suitlessPrefix = "player_mesh_noSuit:Player_";
            suitlessBody = suitlessModel.transform.Find(suitlessPrefix + "Clothes").gameObject;
            suitlessHead = suitlessModel.transform.Find(suitlessPrefix + "Head").gameObject;
            suitlessLArm = suitlessModel.transform.Find(suitlessPrefix + "LeftArm").gameObject;
            suitlessRArm = suitlessModel.transform.Find(suitlessPrefix + "RightArm").gameObject;
            suitlessHeadShader = suitlessModel.transform.Find(suitlessPrefix + "Head_ShadowCaster").gameObject;
            suitlessRArmShader = suitlessModel.transform.Find(suitlessPrefix + "RightArm_ShadowCaster").gameObject;

            // Get all individual parts for suited
            string suitedPrefix = "Traveller_Mesh_v01:";
            suitBody = suitModel.transform.Find(suitedPrefix + "PlayerSuit_Body").gameObject;
            suitHead = suitModel.transform.Find(suitedPrefix + "PlayerSuit_Helmet").gameObject;
            suitLArm = suitModel.transform.Find(suitedPrefix + "PlayerSuit_LeftArm").gameObject;
            suitRArm = suitModel.transform.Find(suitedPrefix + "PlayerSuit_RightArm").gameObject;
            suitHeadShader = suitModel.transform.Find(suitedPrefix + "PlayerSuit_Helmet_ShadowCaster").gameObject;
            suitRArmShader = suitModel.transform.Find(suitedPrefix + "PlayerSuit_RightArm_ShadowCaster").gameObject;
            suitJetpack = suitModel.transform.Find(suitedPrefix + "Props_HEA_Jetpack").gameObject;

            // Now that all vars are set, make the actual ingame changes
            ChangeOutfit();
        }

        public void ChangeOutfit()
        {
            // Make sure that the scene is the SS or Eye
            if (WrongScene()) return;
            // Cancel otherwise

            bool isSuited = characterController._isWearingSuit;

            // Jacket
            switch (bodySetting)
            {
                case "Always Suitless":
                    suitBody.SetActive(false);
                    break;
                case "Default":
                    suitBody.SetActive(isSuited);
                    break;
                case "Always Suited":
                    suitBody.SetActive(true);
                    break;
                case "Opposite":
                    suitBody.SetActive(!isSuited);
                    break;
            }

            // Arms
            switch (armsSetting)
            {
                case "Always Suitless":
                    suitLArm.SetActive(false);
                    suitRArm.SetActive(false);
                    break;
                case "Default":
                    suitLArm.SetActive(isSuited);
                    suitRArm.SetActive(isSuited);
                    break;
                case "Always Suited":
                    suitLArm.SetActive(true);
                    suitRArm.SetActive(true);
                    break;
                case "Opposite":
                    suitLArm.SetActive(!isSuited);
                    suitRArm.SetActive(!isSuited);
                    break;
            }

            // Helmet
            switch (headSetting)
            {
                case "Always Suitless":
                    suitHead.SetActive(false);
                    break;
                case "Default":
                    suitHead.SetActive(isSuited);
                    break;
                case "Always Suited":
                    suitHead.SetActive(true);
                    break;
                case "Opposite":
                    suitHead.SetActive(!isSuited);
                    break;
            }

            // Jetpack
            switch (jetpackSetting)
            {
                case "Always Off":
                    suitJetpack.SetActive(false);
                    suitJetpackFX.SetActive(false);
                    ChangeAnimGroup("Suitless");
                    break;
                case "Default":
                    suitJetpack.SetActive(isSuited);
                    suitJetpackFX.SetActive(isSuited);
                    if (!isSuited) ChangeAnimGroup("Suitless");
                    else ChangeAnimGroup("Suited");
                    break;
                case "Always On":
                    suitJetpack.SetActive(true);
                    suitJetpackFX.SetActive(true);
                    ChangeAnimGroup("Suited");
                    break;
                case "Opposite":
                    suitJetpack.SetActive(!isSuited);
                    suitJetpackFX.SetActive(!isSuited);
                    if (isSuited) ChangeAnimGroup("Suitless");
                    else ChangeAnimGroup("Suited");
                    break;
            }


            // Set both suitless and suited whole models as visible
            suitlessModel.SetActive(true);
            suitModel.SetActive(true);

            // Enable suitless body part if the cooresponding suited part is inactive
            suitlessBody.SetActive(!suitBody.activeSelf);
            suitlessHead.SetActive(!suitHead.activeSelf);
            suitlessLArm.SetActive(!suitLArm.activeSelf);
            suitlessRArm.SetActive(!suitRArm.activeSelf);

            // If player chose to suit only one arm, unsuit the other
            switch (suitOneArm)
            {
                case "Off":
                    break;

                case "Left":
                    suitlessRArm.SetActive(true);
                    suitRArm.SetActive(false);
                    break;

                case "Right":
                    suitlessLArm.SetActive(true);
                    suitLArm.SetActive(false);
                    break;
            }

            // Remove chosen body parts
            if (missingBody)
            {
                suitlessBody.SetActive(false);
                suitBody.SetActive(false);
            }
            if (missingHead)
            {
                suitlessHead.SetActive(false);
                suitHead.SetActive(false);
            }
            if (missingLArm)
            {
                suitlessLArm.SetActive(false);
                suitLArm.SetActive(false);
            }
            if (missingRArm)
            {
                suitlessRArm.SetActive(false);
                suitRArm.SetActive(false);
            }

            // Enable shaders for visible parts that have them
            suitlessHeadShader.SetActive(suitHeadShader.activeSelf);
            suitlessRArmShader.SetActive(suitRArmShader.activeSelf);
            suitHeadShader.SetActive(suitHead.activeSelf);
            suitRArmShader.SetActive(suitRArm.activeSelf);
        }

        void ChangeAnimGroup(string animGroup)
        {
            // There are two anim groups for player: one for the suitless and one for suited. It looks weird
            // if the hatchling uses suit anims when the jetpack is off because they hold their left arm in
            // the air weirdly, so I switch anims, using suitless if no jetpack and suited if jetpack.
            switch (animGroup)
            {
                case "Suitless":
                    animController._animator.runtimeAnimatorController = animController._unsuitedAnimOverride;
                    animController._unsuitedGroup.SetActive(!PlayerState.InMapView());
                    animController._suitedGroup.SetActive(false);
                    break;
                case "Suited":
                    animController._animator.runtimeAnimatorController = animController._baseAnimController;
                    animController._unsuitedGroup.SetActive(false);
                    animController._suitedGroup.SetActive(!PlayerState.InMapView());
                    break;
            }
        }

        bool WrongScene()
        {
            OWScene scene = LoadManager.s_currentScene;
            return !(scene == OWScene.SolarSystem || scene == OWScene.EyeOfTheUniverse);
        }
    }

    public static class Patches
    {
        public static void PlayerAnimControllerAwake()
        {
            // If the anim controller just woke up then the scene was just loaded and the vars need to be
            // reset before any outfit can be changed, so go to SetVars.
            HatchlingOutfit.Instance.SetVars();
        }

        public static void SuitChanged()
        {
            // If hatchling changes suits then all the vars are already loaded, so go straight to ChangeOutfit
            HatchlingOutfit.Instance.ChangeOutfit();
        }
    }
}

/*      
 *      == 2022/5/22 ==
 *      
 *      Oh, hi. Didn't see you there ;)
 *  
 *      I made this mod while I really should have been studying for finals and finishing my makeup work for
 *      school.
 *  
 *      But I didn't, because funny weird suit hatchling is clearly more important than my academic career!
 *      
 */  
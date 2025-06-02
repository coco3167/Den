/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID GAMESYNCS = 170047161U;
        static const AkUniqueID PLAY_DEN_AMB = 1394160082U;
        static const AkUniqueID PLAY_DEN_BUSHENTER = 1088048986U;
        static const AkUniqueID PLAY_DEN_CREA_ANGERSTEPS = 2814436364U;
        static const AkUniqueID PLAY_DEN_CREA_CURIOUSSTEPS = 3724550301U;
        static const AkUniqueID PLAY_DEN_CREA_FEARSTEPS = 3029527017U;
        static const AkUniqueID PLAY_DEN_CREA_NEUTRALBARK = 1250462805U;
        static const AkUniqueID PLAY_DEN_CREA_RANDOMBARKS = 3621090860U;
        static const AkUniqueID PLAY_DEN_CREA_RB_ANGER = 3903286686U;
        static const AkUniqueID PLAY_DEN_CREA_RB_CURIOUS = 1141265601U;
        static const AkUniqueID PLAY_DEN_CREA_RB_FEAR = 231948563U;
        static const AkUniqueID PLAY_DEN_CREA_SCREAMBARKS = 4165947622U;
        static const AkUniqueID PLAY_DEN_MUSIC_END = 2532886009U;
        static const AkUniqueID PLAY_DEN_MUSIC_MENU = 334277355U;
        static const AkUniqueID PLAY_DEN_MUSIC_START = 486039834U;
        static const AkUniqueID PLAY_DEN_MUSIC_TEST = 1174350178U;
        static const AkUniqueID PLAY_DEN_STARTGAME = 3010429372U;
        static const AkUniqueID PLAY_DEN_STI_MOOD_ANGER = 3155287650U;
        static const AkUniqueID PLAY_DEN_STI_MOOD_CURIOUS = 2374769269U;
        static const AkUniqueID PLAY_DEN_STI_MOOD_FEAR = 2375292999U;
        static const AkUniqueID PLAY_DEN_STINGER = 198938308U;
        static const AkUniqueID PLAY_DEN_TOD = 892655767U;
        static const AkUniqueID PLAY_DEN_UI_CURSOR = 773185301U;
        static const AkUniqueID SET_DEN_AMB_ANGER1 = 1566837199U;
        static const AkUniqueID SET_DEN_AMB_ANGER2 = 1566837196U;
        static const AkUniqueID SET_DEN_AMB_ANGER3 = 1566837197U;
        static const AkUniqueID SET_DEN_AMB_CURIOUS1 = 1268512806U;
        static const AkUniqueID SET_DEN_AMB_CURIOUS2 = 1268512805U;
        static const AkUniqueID SET_DEN_AMB_CURIOUS3 = 1268512804U;
        static const AkUniqueID SET_DEN_AMB_FEAR1 = 3699036620U;
        static const AkUniqueID SET_DEN_AMB_FEAR2 = 3699036623U;
        static const AkUniqueID SET_DEN_AMB_FEAR3 = 3699036622U;
        static const AkUniqueID SET_DEN_AMB_RESET = 4204191990U;
        static const AkUniqueID SET_DEN_TOD_DAY = 4020875500U;
        static const AkUniqueID SET_DEN_TOD_EVENING = 2386744970U;
        static const AkUniqueID SET_DEN_TOD_MORNING = 1585709882U;
        static const AkUniqueID SET_DEN_TOD_NIGHT = 282213896U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace DEN_STATE_AMBIENCE
        {
            static const AkUniqueID GROUP = 1560129083U;

            namespace STATE
            {
                static const AkUniqueID DEN_ST_AMB_PLAYING = 3026507386U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DEN_STATE_AMBIENCE

        namespace DEN_STATE_AUDIO_MIX
        {
            static const AkUniqueID GROUP = 1852625376U;

            namespace STATE
            {
                static const AkUniqueID DEN_ST_AUDIO_MIX_MONO = 2408252910U;
                static const AkUniqueID DEN_ST_AUDIO_MIX_STEREOHP = 1541385743U;
                static const AkUniqueID DEN_ST_AUDIO_MIX_STEREOSP = 1960826282U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DEN_STATE_AUDIO_MIX

        namespace DEN_STATE_MOOD
        {
            static const AkUniqueID GROUP = 3079765046U;

            namespace STATE
            {
                static const AkUniqueID DEN_ST_MOOD_ANGER = 999913598U;
                static const AkUniqueID DEN_ST_MOOD_CURIOUSITY = 2378539543U;
                static const AkUniqueID DEN_ST_MOOD_FEAR = 1511253875U;
                static const AkUniqueID DEN_ST_MOOD_NEUTRAL = 3523856042U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DEN_STATE_MOOD

        namespace DEN_STATE_TOD
        {
            static const AkUniqueID GROUP = 2248328396U;

            namespace STATE
            {
                static const AkUniqueID DEN_ST_TOD_DAY = 354475675U;
                static const AkUniqueID DEN_ST_TOD_EVENING = 3179366297U;
                static const AkUniqueID DEN_ST_TOD_MORNING = 1758083905U;
                static const AkUniqueID DEN_ST_TOD_NIGHT = 1822764695U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace DEN_STATE_TOD

    } // namespace STATES

    namespace SWITCHES
    {
        namespace DEN_SW_ANGERSTEPS
        {
            static const AkUniqueID GROUP = 3664495308U;

            namespace SWITCH
            {
                static const AkUniqueID DEN_SW_AS_0 = 3467041815U;
                static const AkUniqueID DEN_SW_AS_1 = 3467041814U;
                static const AkUniqueID DEN_SW_AS_2 = 3467041813U;
                static const AkUniqueID DEN_SW_AS_3 = 3467041812U;
            } // namespace SWITCH
        } // namespace DEN_SW_ANGERSTEPS

        namespace DEN_SW_CURIOSITYSTEPS
        {
            static const AkUniqueID GROUP = 3152036582U;

            namespace SWITCH
            {
                static const AkUniqueID DEN_SW_CS_0 = 4081255473U;
                static const AkUniqueID DEN_SW_CS_1 = 4081255472U;
                static const AkUniqueID DEN_SW_CS_2 = 4081255475U;
                static const AkUniqueID DEN_SW_CS_3 = 4081255474U;
            } // namespace SWITCH
        } // namespace DEN_SW_CURIOSITYSTEPS

        namespace DEN_SW_FEARSTEPS
        {
            static const AkUniqueID GROUP = 1947237033U;

            namespace SWITCH
            {
                static const AkUniqueID DEN_SW_FS_0 = 3562904914U;
                static const AkUniqueID DEN_SW_FS_1 = 3562904915U;
                static const AkUniqueID DEN_SW_FS_2 = 3562904912U;
                static const AkUniqueID DEN_SW_FS_3 = 3562904913U;
            } // namespace SWITCH
        } // namespace DEN_SW_FEARSTEPS

        namespace DEN_SW_FOOTSTEPS
        {
            static const AkUniqueID GROUP = 2461695847U;

            namespace SWITCH
            {
                static const AkUniqueID DEN_SW_FS_RUNNING = 57715291U;
                static const AkUniqueID DEN_SW_FS_WALKING = 3564956079U;
            } // namespace SWITCH
        } // namespace DEN_SW_FOOTSTEPS

        namespace DEN_SW_NEUTRALSTEPS
        {
            static const AkUniqueID GROUP = 3572877288U;

            namespace SWITCH
            {
                static const AkUniqueID DEN_SW_NS_0 = 2067990490U;
                static const AkUniqueID DEN_SW_NS_1 = 2067990491U;
                static const AkUniqueID DEN_SW_NS_2 = 2067990488U;
                static const AkUniqueID DEN_SW_NS_3 = 2067990489U;
            } // namespace SWITCH
        } // namespace DEN_SW_NEUTRALSTEPS

        namespace DEN_SW_REACTIONMOOD
        {
            static const AkUniqueID GROUP = 2393674518U;

            namespace SWITCH
            {
                static const AkUniqueID DEN_SW_RM_ANGER = 550223305U;
                static const AkUniqueID DEN_SW_RM_CURIOUS = 798839706U;
                static const AkUniqueID DEN_SW_RM_FEAR = 4041152746U;
            } // namespace SWITCH
        } // namespace DEN_SW_REACTIONMOOD

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID DEN_GP_ANGERVALUE = 722514783U;
        static const AkUniqueID DEN_GP_CURIOSITYVALUE = 2578001617U;
        static const AkUniqueID DEN_GP_CURSORSPEED = 4061733226U;
        static const AkUniqueID DEN_GP_CURSORVOLUME = 2230858223U;
        static const AkUniqueID DEN_GP_FEARVALUE = 4122714024U;
        static const AkUniqueID DEN_GP_INTENSITYVALUE = 973587621U;
        static const AkUniqueID DEN_GP_MONO = 1386681402U;
        static const AkUniqueID DEN_GP_NEUTRALVALUE = 1734226615U;
        static const AkUniqueID DEN_GP_TENSIONVALUE = 3189323882U;
        static const AkUniqueID DEN_GP_UIV_AMBIENCE = 2280064220U;
        static const AkUniqueID DEN_GP_UIV_MASTER = 2421685046U;
        static const AkUniqueID DEN_GP_UIV_MUSIC = 3976517781U;
        static const AkUniqueID DEN_GP_UIV_SFX = 307573345U;
        static const AkUniqueID DEN_GP_UIV_UI = 1763700666U;
        static const AkUniqueID DEN_GP_VOLUMEMOMO = 3482592633U;
    } // namespace GAME_PARAMETERS

    namespace TRIGGERS
    {
        static const AkUniqueID DEN_TR_ANGERSTEP_01 = 2123502977U;
        static const AkUniqueID DEN_TR_ANGERSTEP_02 = 2123502978U;
        static const AkUniqueID DEN_TR_ANGERSTEP_03 = 2123502979U;
        static const AkUniqueID DEN_TR_CURIOUSSTEP_01 = 102155032U;
        static const AkUniqueID DEN_TR_CURIOUSSTEP_02 = 102155035U;
        static const AkUniqueID DEN_TR_CURIOUSSTEP_03 = 102155034U;
        static const AkUniqueID DEN_TR_FEARSTEP_01 = 2550257292U;
        static const AkUniqueID DEN_TR_FEARSTEP_02 = 2550257295U;
        static const AkUniqueID DEN_TR_FEARSTEP_03 = 2550257294U;
    } // namespace TRIGGERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN_SOUNDBANK = 2228651116U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBIENCE = 85412153U;
        static const AkUniqueID FX = 1802970371U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID SFX = 393239870U;
        static const AkUniqueID UI = 1551306167U;
    } // namespace BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__

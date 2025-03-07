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
        static const AkUniqueID AMBIENCE_BASS = 1720356053U;
        static const AkUniqueID AMBIENCE_RAIN = 710389270U;
        static const AkUniqueID BOOST_RUSH = 4043448287U;
        static const AkUniqueID BOOST_START = 2188867727U;
        static const AkUniqueID BOOST_STOP = 1751967765U;
        static const AkUniqueID CAMERA_MOVING = 3353320293U;
        static const AkUniqueID CAMERA_STOP = 1515494175U;
        static const AkUniqueID CLOAK_START = 638098506U;
        static const AkUniqueID CLOAK_STOP = 1518276258U;
        static const AkUniqueID ENEMY_ALERTED = 359728807U;
        static const AkUniqueID GUARD_FOOTSTEP = 3149531453U;
        static const AkUniqueID IN_VIEW_CONE = 2249657186U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PLAYER_FOOTSTEP = 2453392179U;
        static const AkUniqueID PLAYER_JUMP = 1305133589U;
        static const AkUniqueID PLAYER_LAND = 3629196698U;
        static const AkUniqueID PLAYER_SLIDE = 2609528332U;
        static const AkUniqueID PLAYER_WALL_JUMP = 4245309516U;
        static const AkUniqueID POWER_GENERATOR = 2283979374U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace AMBIENCE
        {
            static const AkUniqueID GROUP = 85412153U;

            namespace STATE
            {
                static const AkUniqueID INSIDE = 3553349781U;
                static const AkUniqueID NOAMBIENCE = 2065151404U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OUTSIDE = 438105790U;
            } // namespace STATE
        } // namespace AMBIENCE

        namespace MUSIC
        {
            static const AkUniqueID GROUP = 3991942870U;

            namespace STATE
            {
                static const AkUniqueID ALARM_HIGH = 3599981451U;
                static const AkUniqueID ALARM_LOW = 4009708623U;
                static const AkUniqueID ALARM_MIDDLE = 122271732U;
                static const AkUniqueID HIDDEN = 3621873013U;
                static const AkUniqueID NOMUSIC = 1862135557U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace MUSIC

        namespace REVERB
        {
            static const AkUniqueID GROUP = 348963605U;

            namespace STATE
            {
                static const AkUniqueID LARGE = 4284352190U;
                static const AkUniqueID MEDIUM = 2849147824U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OUTSIDE = 438105790U;
                static const AkUniqueID SMALL = 1846755610U;
            } // namespace STATE
        } // namespace REVERB

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GUARD_FOOTSTEP_MATERIAL
        {
            static const AkUniqueID GROUP = 1980337323U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID RUBBER = 437659151U;
            } // namespace SWITCH
        } // namespace GUARD_FOOTSTEP_MATERIAL

        namespace PLAYER_FOOTSTEP_MATERIAL
        {
            static const AkUniqueID GROUP = 4145461805U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID RUBBER = 437659151U;
            } // namespace SWITCH
        } // namespace PLAYER_FOOTSTEP_MATERIAL

        namespace PLAYER_JUMP_MATERIAL
        {
            static const AkUniqueID GROUP = 1085835795U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID RUBBER = 437659151U;
            } // namespace SWITCH
        } // namespace PLAYER_JUMP_MATERIAL

        namespace PLAYER_LAND_MATERIAL
        {
            static const AkUniqueID GROUP = 1536569246U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID RUBBER = 437659151U;
            } // namespace SWITCH
        } // namespace PLAYER_LAND_MATERIAL

        namespace PLAYER_SLIDE_MATERIAL
        {
            static const AkUniqueID GROUP = 4130691716U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID METAL = 2473969246U;
                static const AkUniqueID RUBBER = 437659151U;
            } // namespace SWITCH
        } // namespace PLAYER_SLIDE_MATERIAL

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID HORIZONTALVELOCITY = 2816179192U;
        static const AkUniqueID SUSPICION = 2268126698U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
        static const AkUniqueID MUSIC = 3991942870U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID LARGEROOMREVERB = 1409684747U;
        static const AkUniqueID MEDIUMROOMREVERB = 2347912553U;
        static const AkUniqueID OUTSIDEREVERB = 3370640258U;
        static const AkUniqueID SMALLROOMREVERB = 2227188135U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__

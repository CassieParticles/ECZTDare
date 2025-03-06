using NUnit.Framework;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AlarmMusicHandler : MonoBehaviour
{
    public AK.Wwise.Event music;
    public AK.Wwise.Event rain;

    AlarmSystem alarm;

    private ArrayList guardsChasing = new ArrayList();

    readonly byte alarmOnBitMask = 0b00000001;
    readonly byte chasedBitMask =  0b00000010;

    private byte currentStates=0;
    private byte prevStates=0;


    private void setBit(ref byte bytes, byte mask)
    {
        bytes |= mask;
    }

    private void resetBit(ref byte bytes, byte mask)
    {
        bytes &= (byte)~mask;
    }

    private bool readBit(byte bytes, byte mask)
    {
        return (bytes & mask) != 0;
    }

    private bool readBitIndex(byte bytes, int index)
    {
        return readBit(bytes, (byte)(1 << index));
    }

    private void AlarmOn(Vector3 playerPosition)
    {
        setBit(ref currentStates, alarmOnBitMask);
        checkMusic();
    }

    private void AlarmOff()
    {
        //Sets the "Music" State Group's active State to "Hidden"
        resetBit(ref currentStates, alarmOnBitMask);
        checkMusic();
    }

    public static AlarmMusicHandler GetMusicHandler()
    {
        return GameObject.Find("MusicSystem").GetComponent<AlarmMusicHandler>();
    }

    public void BeginChase(GuardBehaviour guard)
    {
        guardsChasing.Add(guard);
        setBit(ref currentStates, chasedBitMask);
        checkMusic();
    }

    public void EndChase(GuardBehaviour guard)
    {
        guardsChasing.Remove(guard);
        if(guardsChasing.Count == 0 )
        {
            resetBit(ref currentStates, chasedBitMask);
        }
        checkMusic();
    }

    private void checkMusic()
    {
        //Music should not change
        if(currentStates == prevStates)
        {
            return;
        }
        
        if(!readBit(currentStates,alarmOnBitMask) && !readBit(currentStates, chasedBitMask))
        {
            //Sets the "Music" State Group's active State to "Hidden"
            AkSoundEngine.SetState("Music", "Hidden");
        }     //Alarm off, not chased

        if(!readBit(currentStates,alarmOnBitMask) && readBit(currentStates, chasedBitMask))
        {
            //Sets the "Music" State Group's active State to "Alarm_Middle"
            AkSoundEngine.SetState("Music", "Alarm_Middle");
        }     //Alarm off, chased

        if (readBit(currentStates, alarmOnBitMask) && !readBit(currentStates, chasedBitMask))
        {
            //Sets the "Music" State Group's active State to "Alarm_Low"
            AkSoundEngine.SetState("Music", "Alarm_Low");
        }     //Alarm on, not chased

        if(readBit(currentStates, alarmOnBitMask) && readBit(currentStates, chasedBitMask))
        {
            //Sets the "Music" State Group's active State to "Alarm_High"
            AkSoundEngine.SetState("Music", "Alarm_High");
        }     //Alarm on, chased

        prevStates = currentStates;
    }

    private void Awake()
    {
        alarm = GameObject.Find("AlarmObject").GetComponent<AlarmSystem>();
    }

    private void Start()
    {
        alarm.AddAlarmEnableFunc(AlarmOn);
        alarm.AddAlarmDisableFunc(AlarmOff);

        music.Post(gameObject);
        rain.Post(gameObject);
        //Sets the "Music" State Group's active State to "Hidden"
        AkSoundEngine.SetState("Music", "Hidden");
        //Sets the "Ambience" State Group's active State to "Outside"
        AkSoundEngine.SetState("Ambience", "Outside");
    }
}

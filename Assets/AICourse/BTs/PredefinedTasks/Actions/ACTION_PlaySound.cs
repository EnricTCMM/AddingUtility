
using UnityEngine;
using BTs;

public class ACTION_PlaySound : Action
{

    public string keyClip;
    public string keyVolume;
    public string keyWait;

    public ACTION_PlaySound() { }
    public ACTION_PlaySound(string keyClip, string keyVolume = "1.0", string keyWait = "true") 
    { 
        this.keyClip = keyClip; 
        this.keyVolume = keyVolume; 
        this.keyWait = keyWait; 
    }
   

    private AudioSource audioSource;
    private bool mustWait;

    public override void OnInitialize()
    {
        // find the thing that lets you play sounds... 
        audioSource =  GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.Log("Null audiosource in " + gameObject.name);
            return;
        }

        audioSource.volume = blackboard.Get<float>(keyVolume);
        audioSource.clip = blackboard.Get<AudioClip>(keyClip);
        mustWait = blackboard.Get<bool>(keyWait);
        audioSource.Play();
        
    }

    public override Status OnTick()
    {
        if (audioSource == null) return Status.FAILED;
        if (!mustWait) return Status.SUCCEEDED;
        if (audioSource.isPlaying) return Status.RUNNING;
        else return Status.SUCCEEDED;
    }

    public override void OnAbort()
    {
        base.OnAbort();
        audioSource.Stop();
    }
}

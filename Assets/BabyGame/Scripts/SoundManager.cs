using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioSource audioSourceFX;
    public AudioSource audioSourceBG;
    public AudioSource audioRun;
    public AudioClip clipClick, clipAnKeo, clipAnCauHoi, clipTraLoiDung, clipTraLoiSai;
    public AudioClip clipDamPhan, clipVapDa, clipWin, clipLose, clipHienDich;

    public void PlayClick() {
        audioSourceFX.PlayOneShot(clipClick);
    }
    public void PlayAnKeo() {
        audioSourceFX.PlayOneShot(clipAnKeo);
    }
    public void PlayAnCauHoi() {
        audioSourceFX.PlayOneShot(clipAnCauHoi);
    }
    public void PlayTraLoiDung() {
        audioSourceFX.PlayOneShot(clipTraLoiDung);
    }
    public void PlayTraLoiSai() {
        audioSourceFX.PlayOneShot(clipTraLoiSai);
    }
    public void PlayDamPhan() {
        audioSourceFX.PlayOneShot(clipDamPhan);
    }
    public void PlayVapDa() {
        audioSourceFX.PlayOneShot(clipVapDa);
    }
    public void PlayHienDich() {
        audioSourceFX.PlayOneShot(clipHienDich);
    }
    public void PlayWin() {
        audioSourceFX.PlayOneShot(clipWin);
    }
    public void PlayLose() {
        audioSourceFX.PlayOneShot(clipLose);
    }
    public void PlayBG() {
        audioSourceBG.Play();
    }
    public void StopBG() {
        audioSourceBG.Stop();
    }
    public void PlayRun() {
        audioRun.Play();
    }
    public void StopRun() {
        audioRun.Stop();
    }
}


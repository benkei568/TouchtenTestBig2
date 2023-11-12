using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class AudioSystem : MonoBehaviour
    {
        private static Dictionary<AudioEventEnum, AudioClip> audioDictionary;

        private static AudioDatabase audioDB;
        
        public delegate void AudioLoadFinish();
        public static event AudioLoadFinish onAudioLoadFinished;

        public static IEnumerator LoadAllAudio()
        {
            audioDictionary = new Dictionary<AudioEventEnum, AudioClip>();
            audioDB = Resources.Load<AudioDatabase>("AudioDB");

            var dictKeys = audioDB.audioPathDictionary.Keys;
            foreach (var key in dictKeys)
            {
                ResourceRequest req = Resources.LoadAsync<AudioClip>(audioDB.audioPathDictionary[key]);
                yield return req;
                AudioClip loadedAudio = req.asset as AudioClip;
                if(loadedAudio != null)
                {
                    audioDictionary.Add(key,loadedAudio);
                }
                else
                {
                    Debug.LogError("error loading audio: " + audioDB.audioPathDictionary[key]);
                }
            }
            onAudioLoadFinished?.Invoke();
        }

        public static void PlayAudioOneShot(AudioEventEnum audioEvent)
        {
            AudioSource defaultSource = Camera.main.GetComponent<AudioSource>();
            if(defaultSource == null)
            {
                defaultSource = Camera.main.gameObject.AddComponent<AudioSource>();
            }
            defaultSource.PlayOneShot(audioDictionary[audioEvent]);
        }

        public static void PlayAudioOneShot(GameObject audioSource, AudioEventEnum audioEvent)
        {
            AudioSource source = audioSource.GetComponent<AudioSource>();
            if (source == null)
            {
                source = audioSource.AddComponent<AudioSource>();
            }
            source.PlayOneShot(audioDictionary[audioEvent]);

        }

        public static void PlayAudioLoop(AudioEventEnum audioEvent)
        {
            AudioSource defaultSource = Camera.main.GetComponent<AudioSource>();
            if (defaultSource == null)
            {
                defaultSource = Camera.main.gameObject.AddComponent<AudioSource>();
            }
            defaultSource.loop = true;
            defaultSource.clip = audioDictionary[audioEvent];
            defaultSource.Play();
        }

        public static void PlayAudioLoop(GameObject audioSource, AudioEventEnum audioEvent)
        {
            AudioSource source = audioSource.GetComponent<AudioSource>();
            if (source == null)
            {
                source = audioSource.AddComponent<AudioSource>();
            }
            source.loop = true;
            source.clip = audioDictionary[audioEvent];
            source.Play();
        }
    }
}
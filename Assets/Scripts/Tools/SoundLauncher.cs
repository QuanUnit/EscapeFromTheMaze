using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class SoundLauncher : MonoBehaviour
{
    [SerializeField] private float _minPitch;
    [SerializeField] private float _maxPitch;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
        _audioSource.Play();
    }
}

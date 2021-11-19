using UnityEngine;
using Random = UnityEngine.Random;

namespace MazeGame.Tools
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundLauncher : MonoBehaviour
    {
        [SerializeField] private float _minPitch;
        [SerializeField] private float _maxPitch;
        [SerializeField] private bool _destroyAfterClip;

        private AudioSource _audioSource;


        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound()
        {
            _audioSource.pitch = Random.Range(_minPitch, _maxPitch);
            _audioSource.Play();

            float clipLenght = _audioSource.clip.length;

            if (_destroyAfterClip) Tools.Invoke(this, clipLenght, Destroy);
        }

        private void Destroy()
        {
            if (CanParentDestroy()) Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }

        private bool CanParentDestroy()
        {
            int childrenCount = transform.parent.transform.childCount;
            return childrenCount == 1;
        }
    }
}
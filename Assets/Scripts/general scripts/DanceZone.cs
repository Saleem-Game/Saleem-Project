using UnityEngine;

public class DanceZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform saleem; // Drag Saleem object here
    private AudioSource danceMusic;
    private Animator saleemAnim;

    [Header("Distance Settings")]
    [SerializeField] private float maxDistance = 20f;  // Music starts at 0% volume here
    [SerializeField] private float minDistance = 5f;   // Music reaches 100% volume here
    [SerializeField] private float danceDistance = 8f; // Saleem starts dancing here

    private void Awake()
    {
        danceMusic = GetComponent<AudioSource>();
        danceMusic.loop = true;
        danceMusic.playOnAwake = false;
        danceMusic.spatialBlend = 1.0f; // Force 3D mode for spatial fading

        if (saleem != null)
            saleemAnim = saleem.GetComponent<Animator>();
    }

    void Update()
    {
        if (saleem == null) return;

        // Calculate how far Saleem is from this kid
        float distance = Vector3.Distance(transform.position, saleem.position);

        // --- 1. HANDLE AUDIO FADING ---
        if (distance < maxDistance)
        {
            if (!danceMusic.isPlaying) danceMusic.Play();

            // Calculates volume: 0 at 20m, 1 at 5m
            float targetVolume = Mathf.InverseLerp(maxDistance, minDistance, distance);
            danceMusic.volume = targetVolume;
        }
        else
        {
            if (danceMusic.isPlaying)
            {
                danceMusic.volume = 0;
                danceMusic.Stop();
            }
        }

        // --- 2. HANDLE DANCING ANIMATION ---
        if (saleemAnim != null)
        {
            bool shouldDance = distance < danceDistance;

            // Update animator only if the state changes
            if (saleemAnim.GetBool("isDancing") != shouldDance)
            {
                saleemAnim.SetBool("isDancing", shouldDance);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.ParticleSystem;

public class EchoEffect : MonoBehaviour, IPoolable<EchoEffect>
{
    private ParticleSystem echoParticle;
    private IObjectPool<EchoEffect> pool;
    private bool isStarted = false;

    public void SetPool(IObjectPool<EchoEffect> pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        if(isStarted && !echoParticle.isPlaying)
        {
            isStarted = false;
            pool.Release(this);
        }
    }

    public void Init()
    {
        echoParticle = GetComponent<ParticleSystem>();
    }

    public void PlayEffect()
    {
        echoParticle.Play();
        isStarted = true;
    }

    public void SetEffectDuration(float duration)
    {
        var mainModule = echoParticle.main;
        mainModule.duration = duration;
    }

    public void SetEffectSizeOverLifetime(float startSize, float endSize)
    {
        var sizeModule = echoParticle.sizeOverLifetime;
        sizeModule.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, startSize);
        curve.AddKey(1.0f, endSize);
        sizeModule.size = new ParticleSystem.MinMaxCurve(1.0f, curve);
    }

    public void SetEffectColor(Color color)
    {
        ParticleSystem ps = echoParticle;

        var colorModule = ps.colorOverLifetime;
        colorModule.enabled = true;

        Gradient gradient = new Gradient();

        // 색상 키 설정
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(color, 0.0f);
        colorKeys[1] = new GradientColorKey(color, 1.0f);

        // 투명도 키 설정
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
        alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphaKeys[1] = new GradientAlphaKey(1.0f, 0.7f);
        alphaKeys[2] = new GradientAlphaKey(0.0f, 1.0f);

        gradient.SetKeys(colorKeys, alphaKeys);
        colorModule.color = new ParticleSystem.MinMaxGradient(gradient);
    }
}

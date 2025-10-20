using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class MoleUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image moleImage; 
    [SerializeField] private float riseDistance = 120f;
    [SerializeField] private float riseDuration = 0.25f;

    [HideInInspector] public int spriteIndex = -1;

    public Action<MoleUI> OnHit;
    public Action<MoleUI> OnHidden;

    RectTransform rt;
    Vector2 hiddenPos;
    Vector2 visiblePos;
    Coroutine anim;

    enum State { Hidden, Rising, Visible, Falling, Hit }
    State state = State.Hidden;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        hiddenPos = rt.anchoredPosition;
        visiblePos = hiddenPos + Vector2.up * riseDistance;
    }

    public void Setup(Sprite sprite, int spriteIdx, float riseDist, float riseDur)
    {
        spriteIndex = spriteIdx;
        if (moleImage != null) moleImage.sprite = sprite;
        riseDistance = riseDist;
        riseDuration = riseDur;
        hiddenPos = rt.anchoredPosition;
        visiblePos = hiddenPos + Vector2.up * riseDistance;
        state = State.Hidden;
        gameObject.SetActive(true);
    }

    public void PopUp(float visibleTime)
    {
        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(PopRoutine(visibleTime));
    }

    IEnumerator PopRoutine(float visibleTime)
    {
        state = State.Rising;
        float t = 0f;
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / riseDuration);
            rt.anchoredPosition = Vector2.Lerp(hiddenPos, visiblePos, p);
            yield return null;
        }
        rt.anchoredPosition = visiblePos;
        state = State.Visible;

        float timer = 0f;
        while (timer < visibleTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        state = State.Falling;
        t = 0f;
        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0, 1, t / riseDuration);
            rt.anchoredPosition = Vector2.Lerp(visiblePos, hiddenPos, p);
            yield return null;
        }
        rt.anchoredPosition = hiddenPos;
        state = State.Hidden;
        OnHidden?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (state == State.Visible)
        {
            Hit();
        }
    }

    void Hit()
    {
        if (anim != null) StopCoroutine(anim);
        state = State.Hit;
        rt.anchoredPosition = hiddenPos;
        state = State.Hidden;
        OnHit?.Invoke(this);
    }

    public bool IsVisible()
    {
        return state == State.Visible || state == State.Rising;
    }
}

